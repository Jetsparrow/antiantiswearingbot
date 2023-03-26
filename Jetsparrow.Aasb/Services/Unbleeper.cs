using System.Text.RegularExpressions;

namespace Jetsparrow.Aasb.Services;
public class Unbleeper
{
    SearchDictionary Dict { get; }
    UnbleeperSettings Cfg { get; }

    public Unbleeper(SearchDictionary dict, IOptions<UnbleeperSettings> cfg)
    {
        Dict = dict;
        Cfg = cfg.Value;
        var toBleep = Cfg.BleepedSwearsRegex;

        if (!toBleep.StartsWith('^')) toBleep = "^" + toBleep;
        if (!toBleep.EndsWith('$')) toBleep = toBleep + "$";
        BleepedSwearsRegex = new Regex(toBleep, RegexOptions.Compiled);
    }

    Regex BleepedSwearsRegex { get; }

    static readonly char[] WORD_SEPARATORS = { ' ', '\t', '\r', '\n', '.', ',', '!', '?', ';', ':', '-', '—' };

    public async Task<string> UnbleepSwears(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        text = text.Trim();


        var words = text.Split(WORD_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
        var candidates = words
            .Where(w =>
                StringEx.HasNonWordChars(w)
                && (StringEx.HasWordChars(w) || w.Length >= Cfg.MinAmbiguousWordLength)
                && w.Length >= Cfg.MinWordLength
                && !StringEx.IsTelegramMention(w)
                && !StringEx.IsEmailPart(w)
                && !StringEx.IsHashTag(w)
                && BleepedSwearsRegex.IsMatch(w)
            )
            .ToList();

        if (!candidates.Any())
            return null;

        var response = new StringBuilder();
        for (int i = 0; i < candidates.Count; ++i)
        {
            var m = await Dict.Match(candidates[i]);
            response.AppendLine(new string('*', i + 1) + m.Word + new string('?', m.Distance));
        }
        return response.ToString();
    }
}
