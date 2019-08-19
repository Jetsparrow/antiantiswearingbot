using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace AntiAntiSwearingBot.Tests
{
    public class DetectTests
    {
        Unbleeper ubl { get; }
        Config cfg { get; }
        SearchDictionary dict { get; }

        public DetectTests()
        {
            cfg = Config.Load<Config>("aasb.cfg.json");
            dict = new SearchDictionary(cfg);
            ubl = new Unbleeper(dict, cfg.Unbleeper);
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
        [InlineData("Сергей опять вы**нулся своим знанием тонкостей русского языка; в окно еб*шил стылый ноябрьский ветер. ", "*выебнулся\n**ебашил")]
        public void DetectWordsWithPunctuation(string text, string expected)
        {
            var unbleep = ubl.UnbleepSwears(text).Replace("\r\n", "\n").Trim();
            Assert.Equal(expected, unbleep);
        }
    }
}
