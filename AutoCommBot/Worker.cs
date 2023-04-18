using AutoCommBot.Bots;

namespace AutoCommBot
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly CommentBot _commentBot;

        public Worker(ILogger<Worker> logger, CommentBot commentBot)
        {
            _logger = logger;
            _commentBot = commentBot;
            DoWorkAsyncInfiniteLoop();
        }

        /// <summary>
        /// ������ ����������
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("�������������");
        }

        /// <summary>
        /// ����� ������
        /// </summary>
        /// <returns></returns>
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("����� ������...");
            _commentBot.InitializeBot();
        }

        /// <summary>
        /// ��������� ������
        /// </summary>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("��������� ������...");
            _commentBot.StopBot();
        }

        /// <summary>
        /// ������� �������
        /// </summary>
        /// <returns></returns>
        private async Task DoWorkAsyncInfiniteLoop()
        {
            const int pause = 60000;
            while (true)
            {
                await Task.Delay(pause);
                try
                {
                    _logger.LogInformation("�������� ����������...");
                    await _commentBot.CheckConnection();
                }
                catch (Exception exception)
                {
                    _logger.LogError(exception.Message);
                }
            }
        }
    }
}