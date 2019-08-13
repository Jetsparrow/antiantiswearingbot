using System.Linq;
using System.Text.RegularExpressions;
using Telegram.Bot.Args;

namespace AntiAntiSwearingBot.Commands
{
    public class UnlearnCommand : IChatCommand
    {
        SearchDictionary Dict { get; }

        public UnlearnCommand(SearchDictionary dict)
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
                var res = Dict.Unlearn(word);

            switch (res)
            {
                case SearchDictionary.UnlearnResult.Demoted:
                    return $"Понизил слово \"{word}\"";
                case SearchDictionary.UnlearnResult.Removed:
                    return $"Удалил слово \"{word}\"";
                case SearchDictionary.UnlearnResult.NotFound:
                default:
                    return $"Не нашел слово \"{word}\"";
            }
        }
    }
}
