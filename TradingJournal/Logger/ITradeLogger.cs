using TradingJournal.Api.Bybit.DTO;

    public interface ITradeLogger
    {
        void Info(string message);

        void Warning(string message);

        void Error(string message);

        void Position(PositionState position);

        void Execution(ExecutionDto execution);

        void Trade(Trade trade);
    }

