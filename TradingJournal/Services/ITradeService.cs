using TradingJournal.DTO;

namespace TradingJournal.Api.Services;

public interface ITradeService
{
    Task<List<Trade>> GetAllAsync();

    Task<Trade?> GetByIdAsync(int id);

    Task<Trade> CreateAsync(CreateTradeDto dto);

    Task<Trade?> UpdateAsync(int id, CreateTradeDto dto);

    Task<bool> DeleteAsync(int id);
}