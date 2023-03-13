using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Jetsparrow.Aasb.Commands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jetsparrow.Aasb;

public class Aasb : IHostedService
{
    SearchDictionary Dict { get; }
    Unbleeper Unbleeper { get; }
    ILogger Log { get; }
    public bool Started { get; private set; } = false;

    public Aasb(ILogger<Aasb> log, IOptions<TelegramSettings> tgCfg, Unbleeper unbp)
    {
        Log = log;
        TelegramSettings = tgCfg.Value;
        Unbleeper = unbp;
    }

    TelegramSettings TelegramSettings { get; }
    TelegramBotClient TelegramBot { get; set; }
    ChatCommandRouter Router { get; set; }
    public User Me { get; private set; }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(TelegramSettings.ApiKey))
            return;

        TelegramBot = new TelegramBotClient(TelegramSettings.ApiKey);

        Me = await TelegramBot.GetMeAsync();
        Log.LogInformation("Connected to Telegram as @{Username}", Me.Username);
        Router = new ChatCommandRouter(Me.Username);
        Router.Add(new LearnCommand(Dict), "learn");
        Router.Add(new UnlearnCommand(Dict), "unlearn");

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
        await TelegramBot.CloseAsync();
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
            string commandResponse = null;

            try { commandResponse = Router.Execute(sender, update); }
            catch { }

            if (commandResponse != null)
            {
                await TelegramBot.SendTextMessageAsync(
                    msg.Chat.Id,
                    commandResponse,
                    replyToMessageId: msg.MessageId,
                    disableNotification: true);
            }
            else
            {
                var unbleepResponse = Unbleeper.UnbleepSwears(msg.Text);
                if (unbleepResponse != null)
                    await TelegramBot.SendTextMessageAsync(
                        msg.Chat.Id,
                        unbleepResponse,
                        replyToMessageId: msg.MessageId,
                        disableNotification: true);
            }
        }
        catch (Exception e)
        {
            Log.LogError(e, "Exception while handling message {0}", msg);
        }
    }
}
