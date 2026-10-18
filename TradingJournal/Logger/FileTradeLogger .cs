using TradingJournal.Api.Bybit.DTO;

public class FileTradeLogger : ITradeLogger
{
    private readonly string _filePath;

    public FileTradeLogger()
    {
        var folder = Path.Combine(AppContext.BaseDirectory, "Logs");

        Directory.CreateDirectory(folder);

        _filePath = Path.Combine(
            folder,
            $"tradebuilder_{DateTime.Now:yyyyMMdd}.log");
    }

    public void Info(string message)
        => Write("INFO", message);

    public void Warning(string message)
        => Write("WARN", message);

    public void Error(string message)
        => Write("ERROR", message);

    public void Execution(ExecutionDto execution)
    {
        Write(
            "EXEC",
            $"{execution.Time:yyyy-MM-dd HH:mm:ss.fff} | " +
            $"{execution.Symbol} | " +
            $"{execution.Side} | " +
            $"Qty={execution.Quantity} | " +
            $"Price={execution.Price}");
    }

    public void Position(PositionState position)
    {
        Write(
            "POSITION",
            $"{position.Symbol} | " +
            $"Qty={position.Quantity} | " +
            $"{(position.IsLong ? "Long" : "Short")}");
    }

    public void Trade(Trade trade)
    {
        Write(
            "TRADE",
            $"{trade.Symbol} | " +
            $"{trade.Side} | " +
            $"{trade.EntryPrice} -> {trade.ExitPrice} | " +
            $"PnL={trade.Pnl}");
    }

    private void Write(string level, string message)
    {
        File.AppendAllText(
            _filePath,
            $"[{DateTime.Now:HH:mm:ss.fff}] [{level}] {message}{Environment.NewLine}");
    }
}