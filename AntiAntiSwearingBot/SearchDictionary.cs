﻿using System;
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

            learnInitialRating = Math.Clamp(s.LearnInitialRating, 0,1);
            learnNudgeFactor = Math.Clamp(s.LearnNudgeFactor, 0, 1);
            unlearnNudgeFactor = Math.Clamp(s.UnlearnNudgeFactor, 0, 1);
            minUnlearnNudge = Math.Max(s.MinUnlearnNudge, 0);

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
                    int newIndex = (int)(index * learnNudgeFactor);
                    words.Move(index, newIndex);
                    return false;
                }
                else
                {
                    words.Insert((int)(words.Count * learnInitialRating), word);
                    return true;
                }
            }
        }

        public enum UnlearnResult { NotFound, Demoted, Removed }
        public UnlearnResult Unlearn(string word)
        {
            lock (SyncRoot)
            {
                int index = words.IndexOf(word);
                if (index < 0)
                    return UnlearnResult.NotFound;

                int indexFromEnd = words.Count - 1 - index;
                int change = Math.Max(minUnlearnNudge, (int)(indexFromEnd * unlearnNudgeFactor ));
                int newIndex = index + change;
                if (newIndex > words.Count)
                {
                    words.RemoveAt(index);
                    return UnlearnResult.Removed;
                }
                else
                {
                    words.Move(index, newIndex);
                    return UnlearnResult.Demoted;
                }
            }
        }

        #region service

        readonly string path, tmppath;

        double learnInitialRating = 0.75;
        double learnNudgeFactor = 0.5;
        double unlearnNudgeFactor = 0.66;
        int minUnlearnNudge = 5;

        object SyncRoot = new object();
        List<string> words;
        
        #endregion
    }
}
