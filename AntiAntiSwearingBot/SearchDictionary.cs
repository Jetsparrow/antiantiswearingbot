using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AntiAntiSwearingBot
{
    public class SearchDictionary
    {
        public SearchDictionary(Config cfg)
        {
            var s = cfg.SearchDictionary;
            path = s.DictionaryPath;
            tmppath = path + ".tmp";

            words = File.ReadAllLines(path).ToList();
        }

        public int Count => words.Count;

        public void Save()
        {
            if (File.Exists(tmppath))
                File.Delete(tmppath);
            File.WriteAllLines(tmppath, words);
            if (File.Exists(path))
                File.Delete(path);
            File.Move(tmppath, path);
        }

        public struct WordMatch
        {
            public string Word;
            public int Distance;
            public int Rating;
        }

        public WordMatch Match(string pattern)
            => AllMatches(pattern).First();

        public IEnumerable<WordMatch> AllMatches(string pattern)
        {
            lock (SyncRoot)
            {
                pattern = pattern.ToLowerInvariant();
                return words
                    .Select((w, i) => new WordMatch { Word = w, Distance = Language.LevenshteinDistance(pattern, w), Rating = i })
                    .OrderBy(m => m.Distance)
                    .ThenBy(m => m.Rating);
            }
        }

        public bool Learn(string word)
        {
            lock (SyncRoot)
            {
                int index = words.IndexOf(word);
                if (index > 0)
                {
                    words.Move(index, 0);
                    return false;
                }
                else
                {
                    words.Insert(0, word);
                    return true;
                }
            }
        }

        public bool Unlearn(string word)
        {
            lock (SyncRoot)
                return words.Remove(word);
        }

        #region service

        readonly string path, tmppath;

        object SyncRoot = new object();
        List<string> words;

        #endregion
    }
}
