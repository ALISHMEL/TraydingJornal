namespace TradingJournal.Api.DTO;

public class ImportResultDto
{
    public int Received { get; set; }

    public int Imported { get; set; }

    public int Skipped { get; set; }
}