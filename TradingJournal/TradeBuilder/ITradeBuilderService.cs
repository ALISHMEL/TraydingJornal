using TradingJournal.Api.Bybit.DTO;

namespace TradingJournal.Api.TradeBuilder;

public interface ITradeBuilderService
{
    TradeBuilderResult Build(List<ExecutionDto> executions);
}