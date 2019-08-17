using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AntiAntiSwearingBot.Commands;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AntiAntiSwearingBot
{
    public class AntiAntiSwearingBot : IDisposable
    {
        Config Config { get; }
        SearchDictionary Dict { get; }
        Unbleeper Unbleeper { get; }

        public AntiAntiSwearingBot(Config cfg, SearchDictionary dict)
        {
            Config = cfg;
            Dict = dict;
            Unbleeper = new Unbleeper(dict, cfg.Unbleeper);
        }

        TelegramBotClient Client { get; set; }
        ChatCommandRouter Router { get; set; }
        User Me { get; set; }

        public async Task Init()
        {
            var httpProxy = new WebProxy($"{Config.Proxy.Url}:{Config.Proxy.Port}")
            {
                Credentials = new NetworkCredential(Config.Proxy.Login, Config.Proxy.Password)
            };

            Client = new TelegramBotClient(Config.ApiKey, httpProxy);
            Me = await Client.GetMeAsync();
            Router = new ChatCommandRouter(Me.Username);
            Router.Add(new LearnCommand(Dict), "learn");
            Router.Add(new UnlearnCommand(Dict), "unlearn");

            Client.OnMessage += BotOnMessageReceived;
            Client.StartReceiving();
        }

        public async Task Stop()
        {
            Dispose();
        }

        #region service

        void BotOnMessageReceived(object sender, MessageEventArgs args)
        {
            var msg = args.Message;
            if (msg == null || msg.Type != MessageType.Text)
                return;

            string commandResponse = null;

            try { commandResponse = Router.Execute(sender, args); }
            catch { }

            if (commandResponse != null)
            {
                Client.SendTextMessageAsync(
                    args.Message.Chat.Id,
                    commandResponse,
                    replyToMessageId: args.Message.MessageId);
            }
            else
            {
                var unbleepResponse = Unbleeper.UnbleepSwears(msg.Text);
                if (unbleepResponse != null)
                    Client.SendTextMessageAsync(
                        args.Message.Chat.Id,
                        unbleepResponse,
                        replyToMessageId: args.Message.MessageId);
            }
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Client.StopReceiving();
        }
        
        #endregion
    }
}
