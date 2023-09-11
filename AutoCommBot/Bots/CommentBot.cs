using Telegram.Bot;
using Telegram.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;
using AutoCommBot.Providers;

namespace AutoCommBot.Bots
{
    public class CommentBot
    {
        private ITelegramBotClient? _bot;
        private CancellationTokenSource? _cancellationToken;
        private static ILogger<CommentBot> _logger;
        private static ITelegramProvider _telegramProvider;
        private bool isConnected = true;

        public CommentBot(ILogger<CommentBot> logger, ITelegramProvider telegramProvider)
        {
            _logger = logger;
            _telegramProvider = telegramProvider;
        }

        public void InitializeBot()
        {
            try
            {
                _bot = new TelegramBotClient("api_key");
                _cancellationToken = new CancellationTokenSource();
                var cancellationToken = _cancellationToken.Token;
                var receiverOptions = new ReceiverOptions
                {
                    AllowedUpdates = { }, // receive all update types
                };
                _bot.StartReceiving(
                    HandleUpdateAsync,
                    HandleErrorAsync,
                    receiverOptions,
                    cancellationToken
                );
            }
            catch (Exception ex)
            {
                _logger.LogError($"Произошла ошибка при инициализации бота: {ex.Message}");
            }
        }

        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Получено обновление");
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                if(update.Message.Date > (DateTime.UtcNow - TimeSpan.FromMinutes(1)))
                await SendMessage(botClient, update);
            }
        }

        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApiRequestException)
            {
                _logger.LogError($"Ошибка при отправке запроса к Телеграм API. {exception.Message}");
            }
            else
            {
                _logger.LogError($"Неизвестная ошибка: {exception.Message}");
            }
        }

        public void StopBot()
        {
            _cancellationToken.Cancel();
        }

        private static async Task SendMessage(ITelegramBotClient botClient, Update update)
        {
            var message = update.Message.Text.ToLower();
            if (message == "/start")
            {
                var acceptListButtons = new List<string>
                {
                    "Авторизироваться",
                    "Отмена"
                };
                await botClient.SendTextMessageAsync(update.Message.Chat,
                    "Добро пожаловать! Это чат-бот для для автоматического комментинга постов в телеграм каналах! \n " +
                    "Для начала работы пожалуйста пройдите авторизацию.", replyMarkup: GetKeyboard(acceptListButtons), parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                return;
            }
            else if (message == "/clear")
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Тема диалога очищена, но вы можете задать мне другой вопрос!");
                return;
            }
            else if(message == "авторизироваться")
            {
                await botClient.SendTextMessageAsync(update.Message.Chat, "Введите ваш api_id и api_hash в одну строчку, через пробел, без запятых, в начале стоки дабавив знак #, в фомате: \n " +
                    "\"#api_id api_hash\" \n" +
                    "Если у вас ещё нет этих данных, то вы можете получить их перейдя по этой ссылке: https://my.telegram.org/apps");
                return;
            }
            else if (message.StartsWith("#"))
            {
                message = message.Substring(1);
                int position = message.IndexOf(" ");
                var api_id = message.Substring(0, position);
                var api_hash = message.Substring(position + 1);
                await _telegramProvider.Initialize(api_id, api_hash);
                await botClient.SendTextMessageAsync(update.Message.Chat, "Введите ваш номер телефона в формате: +71234567890");
                return;
            }
            else if (message.StartsWith("+"))
            {
                //message = message.Substring(1);
                var result = await _telegramProvider.LogIn(message);
                await botClient.SendTextMessageAsync(update.Message.Chat, result);
                return;
            }
            else if(message.StartsWith("$"))
            {
                message = message.Substring(1);
                var result = await _telegramProvider.LogIn(message);
                await botClient.SendTextMessageAsync(update.Message.Chat, result);
                return;
            }
            else if (message.StartsWith("-"))
            {
                message = message.Substring(1);
                var result = await _telegramProvider.LogIn(message);
                await botClient.SendTextMessageAsync(update.Message.Chat, result);
                return;
            }
            else if (message.StartsWith("!"))
            {
                message = message.Substring(1);
                var result = await _telegramProvider.LogIn(message);
                await botClient.SendTextMessageAsync(update.Message.Chat, result);
                return;
            }
            return;
        }

        private static ReplyKeyboardMarkup GetKeyboard(List<string> buttons)
        {
            return new ReplyKeyboardMarkup(buttons.Select(item => new[] { new KeyboardButton(item.ToString()) }).ToArray());
        }

        /// <summary>
        /// Проверка соединения с Телеграм API
        /// </summary>
        /// <returns></returns>
        public async Task CheckConnection()
        {
            try
            {
                await _bot.TestApiAsync();
                if (!isConnected)
                {
                    _logger.LogInformation("Соединение с Телеграм API восстановлено");
                    InitializeBot();
                    isConnected = true;
                }
            }
            catch (Exception ex)
            {
                if (isConnected)
                {
                    _logger.LogError($"{ex.Message}: Не удалось подключиться к Телеграм API, возможно отсутствует соединение с интернетом");
                    StopBot();
                    isConnected = false;
                }
            }
        }
    }
}
