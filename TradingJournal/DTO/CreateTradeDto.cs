namespace TradingJournal.DTO;

public class CreateTradeDto
{
    public string Symbol { get; set; } = "";

    public string Side { get; set; } = "";

    public decimal EntryPrice { get; set; }

    public decimal ExitPrice { get; set; }

    public decimal Quantity { get; set; }

    public decimal Pnl { get; set; }

    public decimal Fee { get; set; }

    public int Leverage { get; set; }

    public DateTime OpenTime { get; set; }

    public DateTime CloseTime { get; set; }

    public string? Note { get; set; }
}