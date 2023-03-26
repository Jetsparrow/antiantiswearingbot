namespace Jetsparrow.Aasb.Commands;
public class IdCommand : IChatCommand
{
    public bool Authorize => true;
    public string Execute(CommandContext cmd)
    {
        if (cmd.ChatId == cmd.SenderId)
            return $"userid: `{cmd.SenderId}`";
        return $"chatid: `{cmd.ChatId}`";
    } 
}
