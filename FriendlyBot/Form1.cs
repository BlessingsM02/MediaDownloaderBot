using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Message = Telegram.Bot.Types.Message;

namespace FriendlyBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TelegramBotClient botClient;

        private void Form1_Load(object sender, EventArgs e)
        {
            botClient = new TelegramBotClient("6562800145:AAGakN1UqYeodeJ0MU7NDBbtbxRe2Vtn8-4");
            StartReceiver();
        }

        public async Task StartReceiver()
        {
            var token = new CancellationTokenSource();
            var cancelToken = token.Token;
            var ReOpt = new ReceiverOptions { AllowedUpdates = { } };
            await botClient.ReceiveAsync(OnMessage, ErrorMessage, ReOpt, cancelToken);
        }

        public async Task OnMessage(ITelegramBotClient botclient, Update update, CancellationToken cancellation)
        {
            if (update.Message is Message message)
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Hello there");
                if (message.Entities != null)
                {
                    foreach (var entity in message.Entities)
                    {
                        if (entity.Type == MessageEntityType.Url)
                        {
                            string url = message.Text.Substring(entity.Offset, entity.Length);

                            
                            if (IsMediaUrl(url))
                            {
                                
                                byte[] mediaBytes = await DownloadMediaFromUrl(url);
                                Console.WriteLine("Downlaoding");

                                
                                using (var stream = new MemoryStream(mediaBytes))
                                {
                                    
                                    InputOnlineFile mediaFile = new InputOnlineFile(stream);
                                    await botClient.SendDocumentAsync(message.Chat.Id, mediaFile, "Downloaded Media");
                                    
                                }
                            }
                        }
                    }
                }

            }

        }

        private bool IsMediaUrl(string url)
        {
            return true;
        }

        private async Task<byte[]> DownloadMediaFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsByteArrayAsync();
                }

                
                return null;
            }
        }

        public async  Task ErrorMessage(ITelegramBotClient botClient, Exception e, CancellationToken cancellation)
        {
            if(e is ApiRequestException requestException)
            {
                await botClient.SendTextMessageAsync(" ", e.Message.ToString());
            }
        }

    }
}