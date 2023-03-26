using System.ComponentModel.DataAnnotations;

namespace Jetsparrow.Aasb;

public class UnbleeperSettings
{
    public string LegalWordsRegex { get; set; }
    public string BleepedSwearsRegex { get; set; }
    public int MinAmbiguousWordLength { get; set; }
    public int MinWordLength { get; set; }
}

public class SearchDictionarySettings
{
    public string DictionaryPath { get; set; }

    [Range(typeof(TimeSpan), "00:00:10", "01:00:00")]
    public TimeSpan AutosavePeriod { get; set; }
}

public class TelegramSettings
{
    public string ApiKey { get; set; }
}

public class AccessSettings
{
    public string[] AllowedChats { get; set; }
}