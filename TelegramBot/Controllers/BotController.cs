using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBot.BusinessLogic.Interfaces;
using TelegramBot.Common.DTO;
using TelegramBot.Model.DatabaseModels;

namespace TelegramBot.Controllers
{
    public class BotController
    {
        const int TOP_AMOUNT_FILMS = 5;

        private readonly IFilmService filmService;
        private List<FilmDTO> _films { get; set; }

        public BotController(IFilmService movieService)
        {
            this.filmService = movieService;
            _films= new List<FilmDTO>();
        }

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
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

        public async Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, $"Приветики-пистолетики, {message.Chat.FirstName} 😋");
                await HelpString(message.Chat.Id, bot);
                return;
            }

            if (message.Text == "/top5")
            {
                await bot.SendTextMessageAsync(
                    message.Chat.Id,
                    $"А вот и ТОП-5 для {message.Chat.FirstName} 💜"
                );

                FilmDTO[] filmList = new FilmDTO[TOP_AMOUNT_FILMS];

                for (int i = 0; i < TOP_AMOUNT_FILMS; i++)
                {
                    filmList[i]= filmService.Get(i+1);
                    await PrintFilm(filmList[i], message.Chat.Id, bot);
                }
                await HelpString(message.Chat.Id, bot);
                return;
            }

            if (message.Text == "/choose_genre")
            {
                InlineKeyboardMarkup keyboard = new(new[]
                {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Драма", "drama"),
                InlineKeyboardButton.WithCallbackData("Боевик", "action"),
                InlineKeyboardButton.WithCallbackData("Детектив", "detective"),
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Биография", "biography"),
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
                    $"Рекомендации для drama queen 💔:"
                );

                await PrintFilm(filmService.GetRandomByGenre("драма"), callbackQuery.Message.Chat.Id, bot); 
                await HelpString(callbackQuery.Message.Chat.Id, bot);
                return;
            }
            if (callbackQuery.Data.StartsWith("action"))
            {
                await bot.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    $"Ищу боевики для диванных солнышек 🔫:"
                );
                await PrintFilm(filmService.GetRandomByGenre("боевик"), callbackQuery.Message.Chat.Id, bot);
                await HelpString(callbackQuery.Message.Chat.Id, bot);
                return;
            }
            if (callbackQuery.Data.StartsWith("biography"))
            {
               await bot.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    $"Насладись жизнью интересных людей 🕶:"
                );
                await PrintFilm(filmService.GetRandomByGenre("биографический"), callbackQuery.Message.Chat.Id, bot);
                await PrintFilm(_films[new Random().Next(_films.Count)], callbackQuery.Message.Chat.Id, bot);
                await HelpString(callbackQuery.Message.Chat.Id, bot);
                return;
            }
            if (callbackQuery.Data.StartsWith("detective"))
            {
                await bot.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    $"Самое время для загадок! ♟\n Ищу фильмы:"
                );
                _films = filmService.GetListByGenre("детектив").ToList();

                await HelpString(callbackQuery.Message.Chat.Id, bot);
                return;
            }
            if (callbackQuery.Data.StartsWith("comedy"))
            {
                await bot.SendTextMessageAsync(
                    callbackQuery.Message.Chat.Id,
                    $"Лучшая вещь после сложного дня - хорошая комедия!\nПодборка с наилучшими пожеланиями ✨:"
                );
                _films = filmService.GetListByGenre("комедия").ToList();

                await HelpString(callbackQuery.Message.Chat.Id, bot);
                return;
            }
            await bot.SendTextMessageAsync(
                callbackQuery.Message.Chat.Id,
                $"Вы выбрали: {callbackQuery.Data}"
                );
            return;
        }

        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
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

        public async Task PrintFilm(FilmDTO film, long id, ITelegramBotClient bot)
        {
            await bot.SendPhotoAsync(
                        chatId: id,
                        photo: $"{film.LinkPoster}",
                        $"{film.Name}" + Environment.NewLine +
                        $"Жанр: {film.Genre}" + Environment.NewLine +
                        $"Страна: {film.Country}" + Environment.NewLine+
                        $"Режиссер: {film.Producer}" + Environment.NewLine +
                        $"Cсылка на фильм: {film.Link}" + Environment.NewLine);
            return;
        }

        public async Task HelpString( long id, ITelegramBotClient bot)
        {
            await bot.SendTextMessageAsync(id, "Выбирайте команды:\n\n /choose_genre - выбрать жанр \n\n /top5 - ТОП-5 фильмов");
        }
    }
}
