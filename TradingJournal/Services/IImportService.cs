using TradingJournal.Api.DTO;

namespace TradingJournal.Api.Services;

public interface IImportService
{
    Task<ImportResultDto> ImportHistoryAsync();
}