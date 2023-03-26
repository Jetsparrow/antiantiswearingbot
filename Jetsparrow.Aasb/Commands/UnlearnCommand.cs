using Jetsparrow.Aasb.Services;

namespace Jetsparrow.Aasb.Commands;
public class UnlearnCommand : IChatCommand
{
    public bool Authorize => true;
    SearchDictionary Dict { get; }

    public UnlearnCommand(SearchDictionary dict)
    {
        Dict = dict;
    }

    public string Execute(CommandContext cmd)
    {
        var word = cmd.Parameters.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(word))
            return null;

        if (Dict.Unlearn(word))
            return $"Больше не буду";
        else
            return $"А я и не знаю что такое \"{word}\"";
    }
}
