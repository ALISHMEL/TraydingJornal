using Bybit.Net.Clients;
using Bybit.Net.Interfaces.Clients;
using CryptoExchange.Net.Authentication;
using Microsoft.EntityFrameworkCore;
using TradingJournal.Api.Data;
using TradingJournal.Api.Services;
using TradingJournal.Bybit.Auth;
using Bybit.Net;
using CryptoExchange.Net.Authentication;
using TradingJournal.Bybit;
using TradingJournal.Api.TradeBuilder;
var builder = WebApplication.CreateBuilder(args);

// Подключение к БД
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ITradeService, TradeService>();
builder.Services.AddScoped<IImportService, ImportService>();
builder.Services.AddScoped<ITradeBuilderService, TradeBuilderService>();
builder.Services.AddSingleton<ITradeLogger, FileTradeLogger>();
// Конфигурация настроек Bybit
var bybitSection = builder.Configuration.GetSection("TradingJournalBybit");
builder.Services.Configure<TradingJournalBybitOptions>(bybitSection);

Console.WriteLine(builder.Configuration["TradingJournalBybit:ApiKey"]);
Console.WriteLine(builder.Configuration["TradingJournalBybit:ApiSecret"]);
// РЕГИСТРАЦИЯ КЛИЕНТА BYBIT
builder.Services.AddBybit(options =>
{
    options.ApiCredentials = new ApiCredentials(
        builder.Configuration["TradingJournalBybit:ApiKey"]!,
        builder.Configuration["TradingJournalBybit:ApiSecret"]!);
    //options.Environment = BybitEnvironment.DemoTrading;
});

// Регистрация вашего сервиса
builder.Services.AddScoped<IBybitService, BybitService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();