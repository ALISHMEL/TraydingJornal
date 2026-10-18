using TradingJournal.Api.Bybit.DTO;

namespace TradingJournal.Api.TradeBuilder;

public class TradeBuilderService : ITradeBuilderService
{
    private readonly ITradeLogger _logger;
    public TradeBuilderService(ITradeLogger logger)
    {
        _logger = logger;
    }
    public TradeBuilderResult Build(List<ExecutionDto> executions)
    {
        executions = executions
            .OrderBy(x => x.Time)
            .ToList();

        var result = new TradeBuilderResult();

        var positions = new Dictionary<string, PositionState>();

        foreach (var execution in executions)
        {
            _logger.Execution(execution);
            if (!positions.TryGetValue(execution.Symbol, out var position))
            {
                positions.Add(execution.Symbol, OpenPosition(execution));
                continue;
            }

            ProcessExecution(position, execution, result, positions);
            _logger.Position(position);
        }
        return result;
    }
    private PositionState OpenPosition(ExecutionDto execution)
    {
     
        var qty = execution.Side == "Buy"
            ? execution.Quantity
            : -execution.Quantity;

        var position = new PositionState
        {
            Symbol = execution.Symbol,
            Quantity = qty,
            IsLong = qty > 0,
            OpenTime = execution.Time
        };

        position.EntryExecutions.Add(execution);
        _logger.Info(
    $"OPEN {position.Symbol} {(position.IsLong ? "LONG" : "SHORT")} " +
    $"Qty={position.Quantity} Price={execution.Price}");

        _logger.Position(position);

        return position;
    }
    private void ProcessExecution(
    PositionState position,
    ExecutionDto execution,
    TradeBuilderResult result,
    Dictionary<string, PositionState> positions)
    {
       
        var qty = execution.Side == "Buy"
            ? execution.Quantity
            : -execution.Quantity;

        bool sameDirection =
            Math.Sign(position.Quantity) ==
            Math.Sign(qty);

        if (sameDirection)
        {
            Average(position, execution, qty);
        }
        else
        {
            Close(position, execution, qty, result, positions);
        }
        _logger.Position(position);
    }
    private void Average(
    PositionState position,
    ExecutionDto execution,
    decimal qty)
    {
        _logger.Info(
      $"AVERAGE {position.Symbol} " +
      $"OldQty={position.Quantity} Add={qty}");
        position.Quantity += qty;

        position.EntryExecutions.Add(execution);
    }
    private void Close(
    PositionState position,
    ExecutionDto execution,
    decimal qty,
    TradeBuilderResult result,
    Dictionary<string, PositionState> positions)
    {
        var positionQty = Math.Abs(position.Quantity);
        var executionQty = Math.Abs(qty);


        if (executionQty < positionQty)
        {
            ClosePartial(position, execution, executionQty);
            return;
        }

        if (executionQty == positionQty)
        {
            CloseFull(position, execution, result, positions);
            return;
        }

        ReversePosition(position, execution, qty, result, positions);
        _logger.Position(position);
    }
    private void ClosePartial(
    PositionState position,
    ExecutionDto execution,
    decimal closingQty)
    {
        _logger.Info(
        $"PARTIAL CLOSE {position.Symbol} " +
        $"OldQty={position.Quantity} " +
        $"CloseQty={closingQty}");
        if (position.Quantity > 0)
            position.Quantity -= closingQty;
        else
            position.Quantity += closingQty;
        _logger.Position(position);
        position.ExitExecutions.Add(new ExecutionDto
        {
            ExecutionId = execution.ExecutionId,
            OrderId = execution.OrderId,
            Symbol = execution.Symbol,
            Side = execution.Side,
            Quantity = closingQty,
            Price = execution.Price,
            Fee = execution.Fee,
            Time = execution.Time
        });
        _logger.Position(position);
    }
    private void CloseFull(
    PositionState position,
    ExecutionDto execution,
    TradeBuilderResult result,
    Dictionary<string, PositionState> positions)
    {
        position.ExitExecutions.Add(execution);

        result.Trades.Add(
            CreateTrade(position, execution.Time));

        positions.Remove(position.Symbol);
        _logger.Position(position);
    }
    private Trade CreateTrade(PositionState position, DateTime closeTime)
    {
        var quantity = position.EntryExecutions.Sum(x => x.Quantity);

        var entryPrice =
            position.EntryExecutions.Sum(x => x.Price * x.Quantity) / quantity;

        var exitPrice =
            position.ExitExecutions.Sum(x => x.Price * x.Quantity) / quantity;

        var fee =
            position.EntryExecutions.Sum(x => x.Fee) +
            position.ExitExecutions.Sum(x => x.Fee);

        var pnl = position.IsLong
            ? (exitPrice - entryPrice) * quantity
            : (entryPrice - exitPrice) * quantity;
        _logger.Position(position);

        return new Trade
        {
            Symbol = position.Symbol,
            Side = position.IsLong ? "Long" : "Short",
            Quantity = quantity,
            EntryPrice = entryPrice,
            ExitPrice = exitPrice,
            Fee = fee,
            Pnl = pnl,
            OpenTime = position.OpenTime,
            CloseTime = closeTime,
            ImportedFromBybit = true
        };
        
    }
    private void ReversePosition(
    PositionState position,
    ExecutionDto execution,
    decimal qty,
    TradeBuilderResult result,
    Dictionary<string, PositionState> positions)
    {
    
        var closeQty = Math.Abs(position.Quantity);
        var openQty = Math.Abs(qty) - closeQty;

        // Исполнение, которым закрываем старую позицию
        position.ExitExecutions.Add(new ExecutionDto
        {
            ExecutionId = execution.ExecutionId,
            OrderId = execution.OrderId,
            Symbol = execution.Symbol,
            Side = execution.Side,
            Quantity = closeQty,
            Price = execution.Price,
            Fee = execution.Fee * (closeQty / execution.Quantity),
            Time = execution.Time
        });

        result.Trades.Add(CreateTrade(position, execution.Time));

        positions.Remove(position.Symbol);

        // Исполнение, которым открываем новую позицию
        var newExecution = new ExecutionDto
        {
            ExecutionId = execution.ExecutionId,
            OrderId = execution.OrderId,
            Symbol = execution.Symbol,
            Side = execution.Side,
            Quantity = openQty,
            Price = execution.Price,
            Fee = execution.Fee * (openQty / execution.Quantity),
            Time = execution.Time
        };

        positions.Add(
            execution.Symbol,
            OpenPosition(newExecution));
        _logger.Position(position);
    }
}