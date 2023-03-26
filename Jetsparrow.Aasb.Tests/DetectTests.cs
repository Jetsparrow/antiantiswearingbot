namespace Jetsparrow.Aasb.Tests;

public class DetectTests : BleepTestsBase
{
    [Theory]
    [InlineData("бл**ь", "*блядь")]
    [InlineData("ж**а", "*жопа")]
    public async Task UnbleepSimpleSwears(string word, string expected)
    {
        var unbleep = (await ubl.UnbleepSwears(word)).TrimEnd(Environment.NewLine.ToCharArray());
        Assert.Equal(expected, unbleep);
    }

    [Theory]
    [InlineData("Просто пи**ец, как хочется кушать.", "*пиздец")]
    [InlineData("Ужас на*уй!", "*нахуй")]
    [InlineData("еб*ть-колотить", "*ебать")]
    [InlineData("еб*ть—колотить", "*ебать")]
    [InlineData("Получилась полная х**ня: даже не знаю, что и сказать, б**.", "*херня\n**бля")]
    [InlineData("Сергей опять вы**нулся своим знанием тонкостей русского языка; в окно еб*шил стылый ноябрьский ветер. ", "*выебнулся\n**ебашил")]
    public async void DetectWordsWithPunctuation(string text, string expected)
    {
        var unbleep = (await ubl.UnbleepSwears(text)).Replace("\r\n", "\n").Trim();
        Assert.Equal(expected, unbleep);
    }
}
