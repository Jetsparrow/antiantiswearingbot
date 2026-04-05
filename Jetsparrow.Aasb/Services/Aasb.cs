using Jetsparrow.Aasb.Commands;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Jetsparrow.Aasb.Services;

public class AntiAntiSwearingBot : IHostedService
{
    SearchDictionary Dict { get; }
    Unbleeper Unbleeper { get; }
    IOptionsMonitor<AccessSettings> AccessCfg { get; }
    ILogger Log { get; }
    public bool Started { get; private set; } = false;

    public AntiAntiSwearingBot(
        ILogger<AntiAntiSwearingBot> log,
        IOptions<TelegramSettings> tgCfg,
        Unbleeper unbp,
        IOptionsMonitor<AccessSettings> accessCfg)
    {
        Log = log;
        TelegramSettings = tgCfg.Value;
        Unbleeper = unbp;
        AccessCfg = accessCfg;
    }

    TelegramSettings TelegramSettings { get; }
    TelegramBotClient TelegramBot { get; set; }
    ChatCommandRouter Router { get; set; }
    public User Me { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(TelegramSettings.ApiKey))
        {
            Log.LogCritical("No ApiKey found in config!");
            throw new Exception("No ApiKey found in config!");
        }

        var httpClientHandler = new HttpClientHandler();

        if (TelegramSettings.Proxy is string url)
        {
            httpClientHandler.Proxy = new WebProxy(url);
            httpClientHandler.UseProxy = true;
        }
        var httpClient = new HttpClient(httpClientHandler);

        TelegramBot = new TelegramBotClient(TelegramSettings.ApiKey, httpClient);
        
        Log.LogInformation("Connecting to Telegram...");
        Me = await TelegramBot.GetMe(cancellationToken);
        Log.LogInformation("Connected to Telegram as @{Username}", Me.Username);
        Router = new ChatCommandRouter(Me.Username, AccessCfg);
        Router.Register(new LearnCommand(Dict), "learn");
        Router.Register(new UnlearnCommand(Dict), "unlearn");
        Router.Register(new IdCommand(), "chatid");

        var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } };
        TelegramBot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions);

        Log.LogInformation("AntiAntiSwearBot started!");
        Started = true;
    }


    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await TelegramBot.Close();
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        Log.LogError(exception, "Exception while handling API message");
        return Task.CompletedTask;
    }

    async Task HandleUpdateAsync(ITelegramBotClient sender, Update update, CancellationToken cancellationToken)
    {
        if (update.Type != UpdateType.Message || update?.Message?.Type != MessageType.Text)
            return;
        var msg = update.Message!;

        try
        {
            if (TryParseCommand(update, out var cmd))
            {
                var cmdResponse = Router.Execute(cmd);

                if (cmdResponse != null)
                {
                    await TelegramBot.SendMessage(
                        msg.Chat,
                        cmdResponse,
                        replyParameters: new ReplyParameters { MessageId = msg.MessageId },
                        parseMode: ParseMode.MarkdownV2,
                        disableNotification: true,
                        cancellationToken: cancellationToken);
                }
            }
            else
            {
                var unbleepResponse = await Unbleeper.UnbleepSwears(msg.Text);
                if (unbleepResponse != null)
                    await TelegramBot.SendMessage(
                        msg.Chat,
                        unbleepResponse,
                        replyParameters: new ReplyParameters { MessageId = msg.MessageId },
                        disableNotification: true,
                        cancellationToken: cancellationToken);

            }
        }
        catch (Exception e)
        {
            Log.LogError(e, "Exception while handling message {0}", msg);
        }
    }

    const string NS_TELEGRAM = "telegram://";
    static readonly char[] WS_CHARS = new[] { ' ', '\r', '\n', '\n' };

    bool TryParseCommand(Update update, out CommandContext result)
    {
        var s = update.Message.Text;
        result = null;
        if (string.IsNullOrWhiteSpace(s) || s[0] != '/')
            return false;

        string[] words = s.Split(WS_CHARS, StringSplitOptions.RemoveEmptyEntries);

        var cmdRegex = new Regex(@"/(?<cmd>\w+)(@(?<name>\w+))?");
        var match = cmdRegex.Match(words.First());
        if (!match.Success)
            return false;

        string cmd = match.Groups["cmd"].Captures[0].Value;
        string username = match.Groups["name"].Captures.Count > 0 ? match.Groups["name"].Captures[0].Value : null;
        string[] parameters = words.Skip(1).ToArray();

        result = new CommandContext
        {
            Command = cmd,
            ChatId = NS_TELEGRAM + update.Message.Chat.Id,
            SenderId = NS_TELEGRAM + update.Message.From.Id,
            Recipient = username,
            Parameters = parameters
        };
        return true;
    }
}
