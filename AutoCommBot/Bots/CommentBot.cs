using Telegram.Bot;
using Telegram.Bot.Polling;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoCommBot.Bots
{
    public class CommentBot
    {
        private ITelegramBotClient? _bot;
        private CancellationTokenSource? _cancellationToken;
        private static readonly ILogger<CommentBot> _logger;
        private bool isConnected = true;

        public CommentBot()
        {
        }

        public void InitializeBot()
        {
            try
            {
                _bot = new TelegramBotClient("6229570999:AAEhVHyTh4l8s03rvDzrZbVMk3g7BZ7caDs");
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
                    "Хорошо",
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
