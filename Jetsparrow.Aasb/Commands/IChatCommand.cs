namespace Jetsparrow.Aasb.Commands;
public interface IChatCommand
{
    bool Authorize { get; }
    string Execute(CommandContext cmd);
}
