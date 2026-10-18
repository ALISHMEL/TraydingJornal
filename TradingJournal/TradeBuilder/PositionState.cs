using TradingJournal.Api.Bybit.DTO;

public class PositionState
{
    public string Symbol { get; set; } = "";

    // >0 Long, <0 Short
    public decimal Quantity { get; set; }

    public DateTime OpenTime { get; set; }

    public bool IsLong { get; set; }

    public List<ExecutionDto> EntryExecutions { get; } = new();

    public List<ExecutionDto> ExitExecutions { get; } = new();
}