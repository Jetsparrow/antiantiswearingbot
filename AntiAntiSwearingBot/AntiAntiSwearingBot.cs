using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using AntiAntiSwearingBot.Commands;

namespace AntiAntiSwearingBot;

public class AntiAntiSwearingBot
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

    TelegramBotClient TelegramBot { get; set; }
    ChatCommandRouter Router { get; set; }
    public User Me { get; private set; }

    public async Task Init()
    {
        if (string.IsNullOrWhiteSpace(Config.ApiKey))
            return;

        TelegramBot = new TelegramBotClient(Config.ApiKey);

        Me = await TelegramBot.GetMeAsync();
            
            
        Router = new ChatCommandRouter(Me.Username);
        Router.Add(new LearnCommand(Dict), "learn");
        Router.Add(new UnlearnCommand(Dict), "unlearn");

        var receiverOptions = new ReceiverOptions { AllowedUpdates = new[] { UpdateType.Message } };
        TelegramBot.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions);
    }

    Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };
        Console.WriteLine(ErrorMessage);
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
                    replyToMessageId: msg.MessageId);
            }
            else
            {
                var unbleepResponse = Unbleeper.UnbleepSwears(msg.Text);
                if (unbleepResponse != null)
                    await TelegramBot.SendTextMessageAsync(
                        msg.Chat.Id,
                        unbleepResponse,
                        replyToMessageId: msg.MessageId);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
