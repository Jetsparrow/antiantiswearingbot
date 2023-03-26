namespace Jetsparrow.Aasb.Commands;
public class ChatCommandRouter
{
    string BotUsername { get; }
    Dictionary<string, IChatCommand> Commands { get; }
    IOptionsMonitor<AccessSettings> Access { get; }

    public ChatCommandRouter(string username, IOptionsMonitor<AccessSettings> accessCfg)
    {
        BotUsername = username;
        Access = accessCfg;
        Commands = new Dictionary<string, IChatCommand>();
    }

    public void Register(IChatCommand c, params string[] cmds)
    {
        foreach (var cmd in cmds)
        {
            if (Commands.ContainsKey(cmd))
                throw new ArgumentException($"collision for {cmd}, commands {Commands[cmd].GetType()} and {c.GetType()}");
            Commands[cmd] = c;
        }
    }

    public string Execute(CommandContext cmd)
    {
        var allowed = Access.CurrentValue.AllowedChats;

        if (cmd.Recipient != null && cmd.Recipient != BotUsername)
            return null;

        if (!Commands.TryGetValue(cmd.Command, out var handler))
            return null;

        if (handler.Authorize)
        {
            if (!allowed.Contains(cmd.ChatId) && !allowed.Contains(cmd.SenderId))
                return null;
        }

        return handler.Execute(cmd);
    }
}
