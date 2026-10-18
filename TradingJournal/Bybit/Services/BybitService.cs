using Bybit.Net.Interfaces.Clients;
using TradingJournal.Api.Bybit.DTO;
using Bybit.Net.Enums;
namespace TradingJournal.Bybit;

public class BybitService : IBybitService
{
    private readonly IBybitRestClient _client;

    public BybitService(IBybitRestClient client)
    {
        _client = client;
    }

    public async Task<DateTime> GetServerTimeAsync()
    {
        var result = await _client.V5Api.ExchangeData.GetServerTimeAsync();
        if (!result.Success)
            throw new Exception(result.Error?.Message ?? "Bybit API error");


        return result.Data.TimeSecond;
    }
    public async Task<List<ExecutionDto>> GetHistoryAsync()
    {
       
        var executions = new List<ExecutionDto>();


        string? cursor = null;

        do
        {
            var result = await _client.V5Api.Trading.GetUserTradesAsync(
                Category.Linear,
                limit: 100,
                cursor: cursor);

            if (!result.Success)
            {
                Console.WriteLine(
    $"Received {result.Data.List.Count()} executions");
                throw new Exception(
                    $"Code: {result.Error?.Code}\n" +
                    $"Message: {result.Error?.Message}");
            }
            Console.WriteLine("========== PAGE ==========");
            Console.WriteLine($"Count: {result.Data.List.Count()}");
            Console.WriteLine($"NextCursor: {result.Data.NextPageCursor}");

            foreach (var t in result.Data.List)
            {
                Console.WriteLine(
       $"{t.Symbol,-15} {t.Side,-5} {t.Timestamp:yyyy-MM-dd HH:mm:ss}");
            }
            executions.AddRange(result.Data.List.Select(x => new ExecutionDto
            {
                ExecutionId = x.TradeId,
                OrderId = x.OrderId,
                Symbol = x.Symbol,
                Side = x.Side.ToString(),
                Quantity = x.Quantity,
                Price = x.Price,
                Fee = x.Fee ?? 0,
                Time = x.Timestamp
            }));

            cursor = result.Data.NextPageCursor;

        } while (!string.IsNullOrEmpty(cursor));

        return executions
            .OrderBy(x => x.Time)
            .ToList();
    }
}