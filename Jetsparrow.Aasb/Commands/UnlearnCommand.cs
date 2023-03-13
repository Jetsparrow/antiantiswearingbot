using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace AntiAntiSwearingBot.Commands;
public class UnlearnCommand : IChatCommand
{
    SearchDictionary Dict { get; }

    public UnlearnCommand(SearchDictionary dict)
    {
        Dict = dict;
    }

    public string Execute(CommandString cmd, Update args)
    {
        var word = cmd.Parameters.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(word))
            return null;

        if (!Regex.IsMatch(word, @"[а-яА-Я]+"))
            return null;
        if (Dict.Unlearn(word))
            return $"Удалил слово \"{word}\"";
        else
            return $"Не нашел слово \"{word}\"";
    }
}
