using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;

namespace AntiAntiSwearingBot.Commands
{
    public class LearnCommand : IChatCommand
    {
        SearchDictionary Dict { get; }

        public LearnCommand(SearchDictionary dict)
        {
            Dict = dict;
        }

        public string Execute(CommandString cmd, MessageEventArgs args)
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
}
