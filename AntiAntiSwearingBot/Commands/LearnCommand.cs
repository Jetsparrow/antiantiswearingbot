using System.Text.RegularExpressions;
using Telegram.Bot.Types;

namespace AntiAntiSwearingBot.Commands;
public class LearnCommand : IChatCommand
{
    SearchDictionary Dict { get; }

    public LearnCommand(SearchDictionary dict)
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

        bool newWord = Dict.Learn(word);
        return newWord ? $"Принято слово \"{word}\"" : $"Поднял рейтинг слову \"{word}\"";
    }
}
