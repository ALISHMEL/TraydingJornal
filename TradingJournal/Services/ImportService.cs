using Microsoft.EntityFrameworkCore;
using TradingJournal.Api.Data;
using TradingJournal.Api.DTO;
using TradingJournal.Api.Models;
using TradingJournal.Api.TradeBuilder;
using TradingJournal.Api.Models;
using TradingJournal.Bybit;

namespace TradingJournal.Api.Services;

public class ImportService : IImportService
{
    private readonly ITradeBuilderService _tradeBuilder;
    private readonly IBybitService _bybitService;
    private readonly AppDbContext _context;

    public ImportService(
     IBybitService bybitService,
     AppDbContext context,
     ITradeBuilderService tradeBuilder)
    {
        _bybitService = bybitService;
        _context = context;
        _tradeBuilder = tradeBuilder;
    }

    public async Task<ImportResultDto> ImportHistoryAsync()
    {
        var executions = await _bybitService.GetHistoryAsync();
        var buildResult = _tradeBuilder.Build(executions);

        int imported = 0;
        int skipped = 0;
        foreach (var execution in executions)
        {
            bool exists = await _context.Trades.AnyAsync(x =>
                x.ExecutionId == execution.ExecutionId);

            if (exists)
                continue;

            _context.Trades.Add(new Trade
            {
                ExecutionId = execution.ExecutionId,
                OrderId = execution.OrderId,

                Symbol = execution.Symbol,
                Side = execution.Side,      // Buy / Sell

                EntryPrice = execution.Price,
                ExitPrice = execution.Price,

                Quantity = execution.Quantity,

                Fee = execution.Fee,

                OpenTime = execution.Time,
                CloseTime = execution.Time,

                ImportedFromBybit = true,

                Pnl = 0,
                Leverage = 0
            });
        }
        foreach (var trade in buildResult.Trades)
        {
            bool exists = await _context.Executions.AnyAsync(x =>
                x.Symbol == trade.Symbol &&
                x.Time == trade.OpenTime);

            if (exists)
            {
                skipped++;
                continue;
            }

            _context.Executions.Add(new Execution
            {
                ExecutionId = trade.ExecutionId ?? "",
                OrderId = trade.OrderId ?? "",

                Symbol = trade.Symbol,
                Side = trade.Side,          // Long / Short

                Price = trade.EntryPrice,   // пока сохраняем цену входа

                Quantity = trade.Quantity,

                Fee = trade.Fee,

                Time = trade.OpenTime
            });

            imported++;
        }
        foreach (var trade in buildResult.Trades)
        {
            Console.WriteLine(
                $"{trade.Symbol} {trade.Side} {trade.EntryPrice} -> {trade.ExitPrice}");
        }
        await _context.SaveChangesAsync();

        return new ImportResultDto
        {
            Received = executions.Count,
            Imported = imported,
            Skipped = skipped
        };
    }
}