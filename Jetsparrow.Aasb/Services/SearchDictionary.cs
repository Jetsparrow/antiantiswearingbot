using System.IO;
using System.Text.RegularExpressions;

namespace Jetsparrow.Aasb.Services;

public record struct WordMatch(string Word, int Distance);

public class SearchDictionary
{
    SearchDictionarySettings Cfg { get; }
    UnbleeperSettings UnbleeperCfg { get; }
    Regex LegalWordsRegex { get; }

    public SearchDictionary(
        IOptions<SearchDictionarySettings> searchDictionaryCfg,
        IOptions<UnbleeperSettings> unbleeperCfg,
        IHostApplicationLifetime lifetime)
    {
        Cfg = searchDictionaryCfg.Value;
        UnbleeperCfg = unbleeperCfg.Value;
        Loaded = Load();
        Autosave = AutosaveLoop(lifetime.ApplicationStopping);
        LegalWordsRegex = new Regex(UnbleeperCfg.LegalWordsRegex, RegexOptions.Compiled);
    }


    public async Task<WordMatch> Match(string pattern)
    {
        await Loaded;
        var matches = TopMatches(pattern);
        return matches[Random.Shared.Next(0, matches.Count)];
    }

    IList<WordMatch> TopMatches(string pattern)
    {
        pattern = pattern.ToLowerInvariant();
        List<WordMatch> matches;
        using (var guard = DictLock.GetReadLockToken())
        {
            matches = words
                .Select((w, i) => new WordMatch(w, StringEx.LevenshteinDistance(pattern, w)))
                .ToList();
        };
        var minDist = matches.Min(w => w.Distance);
        return matches.Where(w => w.Distance == minDist).ToList();
    }

    public enum LearnResult { Illegal, Added, Known }
    public LearnResult Learn(string word)
    {
        using var guard = DictLock.GetWriteLockToken();
        if (words.Contains(word))
            return LearnResult.Known;

        if (!LegalWordsRegex.IsMatch(word))
            return LearnResult.Illegal;

        words.Add(word);
        Changed = true;
        return LearnResult.Added;
    }

    public bool Unlearn(string word)
    {
        using var guard = DictLock.GetWriteLockToken();
        var res = words.Remove(word);
        Changed |= res;
        return res;
    }

    ReaderWriterLockSlim DictLock = new();
    List<string> words;

    #region load/save
    Task Loaded, Autosave;
    bool Changed;
    async Task Load()
    {
        words = new List<string>(await File.ReadAllLinesAsync(Cfg.DictionaryPath));
    }

    async Task AutosaveLoop(CancellationToken stopToken)
    {
        try
        {
            while (!stopToken.IsCancellationRequested)
            {
                await Task.Delay(Cfg.AutosavePeriod, stopToken);
                DoSave();
            }
        }
        catch (TaskCanceledException) { }
        DoSave();
    }
    void DoSave()
    {
        if (!Changed) return;
        using (var guard = DictLock.GetWriteLockToken())
        {
            if (!Changed) return;
            Changed = false;
            var tmppath = Cfg.DictionaryPath + ".tmp";
            File.WriteAllLines(tmppath, words);
            File.Move(tmppath, Cfg.DictionaryPath, overwrite: true);
        }
    }

    #endregion
}
