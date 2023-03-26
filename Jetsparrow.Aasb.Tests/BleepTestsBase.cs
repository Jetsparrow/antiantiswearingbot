using Jetsparrow.Aasb.Services;
using Jetsparrow.Aasb.Tests.Utils;

namespace Jetsparrow.Aasb.Tests;

public class BleepTestsBase
{
    public BleepTestsBase()
    {
        dictCfg = Options.Create(new SearchDictionarySettings
        {
            AutosavePeriod = TimeSpan.MaxValue,
            DictionaryPath = "dict/ObsceneDictionaryRu.txt"
        });

        ublCfg = Options.Create(new UnbleeperSettings
        {
            LegalWordsRegex = "[а-яА-ЯёЁ]+",
            BleepedSwearsRegex = "[а-яА-ЯёЁ@\\*#]+",
            MinWordLength = 3,
            MinAmbiguousWordLength = 5
        });

        lifetime = new FakeLifetime();

        dict = new SearchDictionary(dictCfg, ublCfg, lifetime);
        ubl = new Unbleeper(dict, ublCfg);
    }

    protected Unbleeper ubl { get; }
    protected SearchDictionary dict { get; }
    protected IOptions<SearchDictionarySettings> dictCfg { get; }
    protected IOptions<UnbleeperSettings> ublCfg { get; }
    protected IHostApplicationLifetime lifetime { get; }
}