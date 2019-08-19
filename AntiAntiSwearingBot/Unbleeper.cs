using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AntiAntiSwearingBot
{
    public class Unbleeper
    {
        SearchDictionary Dict { get; }
        UnbleeperSettings Cfg { get; }

        public Unbleeper(SearchDictionary dict, UnbleeperSettings cfg)
        {
            Dict = dict;
            Cfg = cfg;
            BleepedSwearsRegex = new Regex("^" + Cfg.BleepedSwearsRegex + "$", RegexOptions.Compiled);
        }

        Regex BleepedSwearsRegex { get; }

        static readonly char[] WORD_SEPARATORS = { ' ', '\t', '\r', '\n', '.', ',', '!', '?', ';' };

        public string UnbleepSwears(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            text = text.Trim();

            if (text.StartsWith('/')) // is chat command
                return null;

            var words = text.Split(WORD_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            var candidates = words
                .Where(w =>
                    !Language.IsTelegramMention(w)
                    && !Language.IsEmailPart(w)
                    && Language.HasNonWordChars(w)
                    && !Language.IsHashTag(w)
                    && (Language.HasWordChars(w) || w.Length >= Cfg.MinAmbiguousWordLength)
                    && w.Length >= Cfg.MinWordLength
                    && BleepedSwearsRegex.IsMatch(w)
                    )
                .ToArray();

            if (candidates.Any())
            {
                var response = new StringBuilder();
                for (int i = 0; i < candidates.Length; ++i)
                {
                    var m = Dict.Match(candidates[i]);
                    response.AppendLine(new string('*', i + 1) + m.Word + new string('?', m.Distance));
                }
                return response.ToString();
            }
            else
                return null;
        }
    }
}
