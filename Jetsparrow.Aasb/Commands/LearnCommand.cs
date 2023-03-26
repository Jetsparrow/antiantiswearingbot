using Jetsparrow.Aasb.Services;

namespace Jetsparrow.Aasb.Commands;
public class LearnCommand : IChatCommand
{
    public bool Authorize => true;
    SearchDictionary Dict { get; }
    public LearnCommand(SearchDictionary dict)
    {
        Dict = dict;
    }

    public string Execute(CommandContext cmd)
    {
        var word = cmd.Parameters.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(word))
            return null;

        var learnRes = Dict.Learn(word);
        return learnRes switch
        {
            SearchDictionary.LearnResult.Known => $"Я знаю что такое \"{word}\"",
            SearchDictionary.LearnResult.Added => $"Понял принял, \"{word}\"",
            SearchDictionary.LearnResult.Illegal => "Я такое запоминать не буду",
            _ => "ась?"
        };
    }
}
