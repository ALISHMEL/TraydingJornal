using TradingJournal.Bybit.DTO;
using TradingJournal.Api.Bybit.DTO;

namespace TradingJournal.Bybit;

public interface IBybitService
{
    Task<DateTime> GetServerTimeAsync();
    Task<List<ExecutionDto>> GetHistoryAsync();
}