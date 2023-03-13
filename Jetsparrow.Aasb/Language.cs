using System.Text.RegularExpressions;

namespace Jetsparrow.Aasb;
public static class Language
{
    static int min(int a, int b, int c) { return Math.Min(Math.Min(a, b), c); }

    public static int HammingDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a))
        {
            if (string.IsNullOrEmpty(b))
                return 0;
            return b.Length;
        }

        int dist = 0;
        int len = Math.Min(a.Length, b.Length);
        int leftover = Math.Max(a.Length, b.Length) - len;
        for (int i = 0; i < len; ++i)
            if (!CharMatch(a[i], b[i]))
                ++dist;

        return leftover + dist;
    }

    public static int LevenshteinDistance(string a, string b)
    {
        int[] prevRow = new int[b.Length + 1];
        int[] thisRow = new int[b.Length + 1];

        // init thisRow as 
        for (int i = 0; i < prevRow.Length; i++) prevRow[i] = i;

        for (int i = 0; i < a.Length; i++)
        {
            thisRow[0] = i + 1;
            for (int j = 0; j < b.Length; j++)
            {
                var cost = CharMatch(a[i], b[j]) ? 0 : 1;
                thisRow[j + 1] = min(thisRow[j] + 1, prevRow[j + 1] + 1, prevRow[j] + cost);
            }

            var t = prevRow;
            prevRow = thisRow;
            thisRow = t;
        }
        return prevRow[b.Length];
    }

    public static bool CharMatch(char a, char b)
        => a == b || !char.IsLetterOrDigit(a) || !char.IsLetterOrDigit(b);

    static readonly Regex MentionRegex = new Regex("^@[a-zA-Z0-9_]+$", RegexOptions.Compiled);
    static readonly Regex EmailPartRegex = new Regex("^\\w+@\\w+$", RegexOptions.Compiled);

    static readonly Regex HashTagRegex = new Regex("^#\\w+$", RegexOptions.Compiled);

    public static bool IsTelegramMention(string word) => MentionRegex.IsMatch(word);

    public static bool IsEmailPart(string word) => EmailPartRegex.IsMatch(word);

    public static bool IsHashTag(string word) => HashTagRegex.IsMatch(word);

    public static bool HasNonWordChars(string arg) => arg.Any(c => !char.IsLetterOrDigit(c));

    public static bool HasWordChars(string arg) => arg.Any(char.IsLetter);

}
