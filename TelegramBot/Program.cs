using AutoMapper;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BusinessLogic.Implementation;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.Mapping;
using TelegtramBot.Model;

var bot = new TelegramBotClient(token: "5450399836:AAFif_Fl7GciUFf45Qus44isMf8CVc0yXXQ");
using var cts = new CancellationTokenSource();

var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MapperProfile()));
IMapper mapper = mappingConfig.CreateMapper();
ApplicationContext context = new ApplicationContext();
IFilmService filmService = new FilmService(context,mapper);

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = { }, 
};

bot.StartReceiving(
    HandleUpdateAsync,
    HandleErrorAsync,
    receiverOptions,
    cancellationToken: cts.Token);

var me = await bot.GetMeAsync();
Console.WriteLine($"Start @{me.Username}");
Console.ReadLine();

cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
{
    if (update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(bot, update.Message);
        return;
    }

    if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(bot, update.CallbackQuery);
    }
}

async Task HandleMessage(ITelegramBotClient bot, Message message)
{
    if (message.Text.ToLower() == "/start")
    {
        await bot.SendTextMessageAsync(message.Chat.Id, "Приветики-пистолетики!\nВыбирай команды:\n\n /choose_genre - выбрать жанр \n\n /top15 - ТОП-15 фильмов");
        return;
    }

    if (message.Text == "/top15")
    {
        string str = filmService.GetTop15ToString();
        await bot.SendTextMessageAsync(message.Chat.Id, "ТОП-15:"+str); //, replyMarkup: keyboard

        return;
    }

    if (message.Text == "/choose_genre")
    {
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Драма", "drama"),
                InlineKeyboardButton.WithCallbackData("Криминал", "criminal"),
                InlineKeyboardButton.WithCallbackData("Фантастика", "fiction"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Триллер", "triller"),
                InlineKeyboardButton.WithCallbackData("Комедия", "comedy"),
            },
        });
        await bot.SendTextMessageAsync(message.Chat.Id, "Выберите жанр:", replyMarkup: keyboard);
        return;
    }

    await bot.SendTextMessageAsync(message.Chat.Id, $"Очень интересно, но ничего не понятно:\n{message.Text}");
}

async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
{
    if (callbackQuery.Data.StartsWith("drama"))
    {
        await bot.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Рекомендации для drama queen"
        );
        //await GetListByGenre("драма").ToList;
        return;
    }
    if (callbackQuery.Data.StartsWith("criminal"))
    {
        await bot.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Рекомендации для криминального гения"
        );
        return;
    }
    if (callbackQuery.Data.StartsWith("fiction"))
    {
        await bot.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Время фантастики!"
        );
        return;
    }
    if (callbackQuery.Data.StartsWith("triller"))
    {
        await bot.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Самое время для триллеров"
        );
        return;
    }
    if (callbackQuery.Data.StartsWith("comedy"))
    {
        await bot.SendTextMessageAsync(
            callbackQuery.Message.Chat.Id,
            $"Лучшая вещь после сложного дня - хорошая комедия"
        );
        return;
    }
    await bot.SendTextMessageAsync(
        callbackQuery.Message.Chat.Id,
        $"Вы выбрали: {callbackQuery.Data}"
        );
    return;
}

Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Ошибка Telegram API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
        _ => exception.ToString()
    };
    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}