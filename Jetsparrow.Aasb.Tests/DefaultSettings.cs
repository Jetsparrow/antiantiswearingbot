namespace AntiAntiSwearingBot.Tests;

public static class DefaultSettings
{
    public static SearchDictionarySettings SearchDictionary { get; }
    public static UnbleeperSettings Unbleeper { get; }

    static DefaultSettings()
    {
        Unbleeper = new UnbleeperSettings
        {
            BleepedSwearsRegex = @"[а-яА-ЯёЁ@\\*#]+",
            MinWordLength = 3,
            MinAmbiguousWordLength = 5
        };

        SearchDictionary = new SearchDictionarySettings
        {
            DictionaryPath = "dict/ObsceneDictionaryRu.txt"
        };
    }
}
