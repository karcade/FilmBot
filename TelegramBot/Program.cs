using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using TelegramBot.BusinessLogic.Implementation;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.Mapping;
using TelegtramBot.Model;
using TelegramBot.Controllers;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
            .AddLogging()
            .AddSingleton<IFilmService, FilmService>()
            .BuildServiceProvider();
var mapperConfiguration = new MapperConfiguration(x =>
{
    x.AddProfile<MapperProfile>();
});
mapperConfiguration.AssertConfigurationIsValid();
IMapper mapper = mapperConfiguration.CreateMapper();

ApplicationContext context = new ApplicationContext();
IFilmService filmService = new FilmService(context, mapper);

var botController = new BotController(filmService);

var bot = new TelegramBotClient(token: "5450399836:AAFif_Fl7GciUFf45Qus44isMf8CVc0yXXQ");
using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, 
};

bot.StartReceiving(
    botController.HandleUpdateAsync,
    botController.HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await bot.GetMeAsync();
Console.WriteLine($"Start @{me.Username}");
Console.ReadLine();

cts.Cancel();

