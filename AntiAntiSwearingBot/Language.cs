using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AntiAntiSwearingBot
{
    static class Language
    {
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

        static int min(int a, int b, int c) { return Math.Min(Math.Min(a, b), c); }

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

        /// <summary>
        /// Compute the distance between two strings.
        /// </summary>
        public static int Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;
            int[,] d = new int[n + 1, m + 1];

            if (n == 0)
                return m;
            if (m == 0)
                return n;

            // Step 2
            for (int i = 0; i <= n; d[i, 0] = i++)
            {
            }

            for (int j = 0; j <= m; d[0, j] = j++)
            {
            }

            // Step 3
            for (int i = 1; i <= n; i++)
            {
                //Step 4
                for (int j = 1; j <= m; j++)
                {
                    // Step 5
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                    // Step 6
                    d[i, j] = Math.Min(
                        Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            // Step 7
            return d[n, m];
        }
    }
}
