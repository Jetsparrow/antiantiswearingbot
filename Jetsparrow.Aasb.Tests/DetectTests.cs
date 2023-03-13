using System;

using Microsoft.Extensions.Options;

using Xunit;

namespace Jetsparrow.Aasb.Tests;

public class DetectTests
{
    Unbleeper ubl { get; }
    SearchDictionary dict { get; }

    public DetectTests()
    {

        dict = new SearchDictionary(MockOptionsMonitor.Create(DefaultSettings.SearchDictionary));
        ubl = new Unbleeper(dict, Options.Create(DefaultSettings.Unbleeper));
    }

    [Theory]
    [InlineData("бл**ь", "*блядь")]
    [InlineData("ж**а", "*жопа")]
    public void UnbleepSimpleSwears(string word, string expected)
    {
        var unbleep = ubl.UnbleepSwears(word).TrimEnd(Environment.NewLine.ToCharArray());
        Assert.Equal(expected, unbleep);
    }

    [Theory]
    [InlineData("Просто пи**ец, как хочется кушать.", "*пиздец")]
    [InlineData("Ужас на*уй!", "*нахуй")]
    [InlineData("еб*ть-колотить", "*ебать")]
    [InlineData("еб*ть—колотить", "*ебать")]
    [InlineData("Получилась полная х**ня: даже не знаю, что и сказать, б**.", "*херня\n**бля")]
    [InlineData("Сергей опять вы**нулся своим знанием тонкостей русского языка; в окно еб*шил стылый ноябрьский ветер. ", "*выебнулся\n**ебашил")]
    public void DetectWordsWithPunctuation(string text, string expected)
    {
        var unbleep = ubl.UnbleepSwears(text).Replace("\r\n", "\n").Trim();
        Assert.Equal(expected, unbleep);
    }
}
