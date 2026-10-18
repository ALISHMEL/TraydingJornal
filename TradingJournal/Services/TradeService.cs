using Microsoft.EntityFrameworkCore;
using TradingJournal.Api.Data;
using TradingJournal.DTO;

namespace TradingJournal.Api.Services;

public class TradeService : ITradeService
{
    private readonly AppDbContext _context;

    public TradeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Trade>> GetAllAsync()
    {
        return await _context.Trades
            .OrderByDescending(x => x.CloseTime)
            .ToListAsync();
    }

    public async Task<Trade?> GetByIdAsync(int id)
    {
        var trade = await _context.Trades
            .FirstOrDefaultAsync(x => x.Id == id);

        if (trade != null)
        {
            Console.WriteLine(trade.GetType().FullName);
            Console.WriteLine($"Symbol = {trade.Symbol}");
            Console.WriteLine($"Side = {trade.Side}");
            Console.WriteLine($"Qty = {trade.Quantity}");
            Console.WriteLine($"Entry = {trade.EntryPrice}");
            Console.WriteLine($"Exit = {trade.ExitPrice}");
        }

        return trade;
    }

    public async Task<Trade> CreateAsync(CreateTradeDto dto)
    {
        var trade = new Trade
        {
            Symbol = dto.Symbol,
            Side = dto.Side,
            EntryPrice = dto.EntryPrice,
            ExitPrice = dto.ExitPrice,
            Quantity = dto.Quantity,
            Pnl = dto.Pnl,
            Fee = dto.Fee,
            Leverage = dto.Leverage,
            OpenTime = dto.OpenTime,
            CloseTime = dto.CloseTime,
            Note = dto.Note
        };

        _context.Trades.Add(trade);

        await _context.SaveChangesAsync();
        return trade;
    }

    public async Task<Trade?> UpdateAsync(int id, CreateTradeDto dto)
    {
        var trade = await _context.Trades.FindAsync(id);

        if (trade == null)
            return null;

        trade.Symbol = dto.Symbol;
        trade.Side = dto.Side;
        trade.EntryPrice = dto.EntryPrice;
        trade.ExitPrice = dto.ExitPrice;
        trade.Quantity = dto.Quantity;
        trade.Pnl = dto.Pnl;
        trade.Fee = dto.Fee;
        trade.Leverage = dto.Leverage;
        trade.OpenTime = dto.OpenTime;
        trade.CloseTime = dto.CloseTime;
        trade.Note = dto.Note;

        await _context.SaveChangesAsync();
        return trade;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var trade = await _context.Trades.FindAsync(id);

        if (trade == null)
            return false;

        _context.Trades.Remove(trade);

        await _context.SaveChangesAsync();

        return true;
    }
}