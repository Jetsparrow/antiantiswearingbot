namespace Jetsparrow.Aasb.Commands;
public class CommandContext
{
    public string Command { get; set; }
    public string Recipient { get; set; }
    public string ChatId { get; set; }
    public string SenderId { get; set; }
    public string[] Parameters { get; set; }
}
