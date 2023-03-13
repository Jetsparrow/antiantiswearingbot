using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

namespace Jetsparrow.Aasb;
public class SearchDictionary : BackgroundService
{
    public SearchDictionary(IOptionsMonitor<SearchDictionarySettings> cfg)
    {
        Cfg = cfg;
        var path = cfg.CurrentValue.DictionaryPath;
        words = File.ReadAllLines(path).ToList();
    }

    IOptionsMonitor<SearchDictionarySettings> Cfg { get; }
    ReaderWriterLockSlim DictLock = new();
    List<string> words;
    bool Changed = false;

    public int Count => words.Count;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                if (Changed) Save();
            }
        }
        finally
        {
            Save();
        }
    }

    public void Save()
    {
        using var guard = DictLock.GetWriteLockToken();
        Changed = false;

        var path = Cfg.CurrentValue.DictionaryPath;
        var tmppath = path + ".tmp";

        File.WriteAllLines(tmppath, words);
        File.Move(tmppath, path, overwrite: true);
    }

    public record struct WordMatch (string Word, int Distance, int Rating);

    public WordMatch Match(string pattern)
        => AllMatches(pattern).First();

    public IEnumerable<WordMatch> AllMatches(string pattern)
    {
        using var guard = DictLock.GetReadLockToken();
        return words
            .Select((w, i) => new WordMatch(w, Language.LevenshteinDistance(pattern.ToLowerInvariant(), w), i))
            .OrderBy(m => m.Distance)
            .ThenBy(m => m.Rating);
    }

    public bool Learn(string word)
    {
        using var guard = DictLock.GetWriteLockToken();
        Changed = true;

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

    public bool Unlearn(string word)
    {
        using var guard = DictLock.GetWriteLockToken();
        Changed = true;
        return words.Remove(word);
    }

}
