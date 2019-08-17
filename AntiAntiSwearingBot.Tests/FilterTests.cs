using System;
using Xunit;

namespace AntiAntiSwearingBot.Tests
{
    public class FilterTests
    {
        Unbleeper ubl { get; }
        public FilterTests()
        {
            var cfg = Config.Load<Config>("aasb.cfg.json", "aasb.cfg.secret.json");
            var dict = new SearchDictionary(cfg);
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
        [InlineData("*")]
        [InlineData("**#")]
        [InlineData("@**#")]
        public void IgnoreShortGrawlixes(string text) => Assert.Null(ubl.UnbleepSwears(text));

        [Theory]
        [InlineData("@pvkuznetsov https://github.com/jacksondunstan/UnityNativeScripting")]
        [InlineData("@JohnnyMnemonic")]
        [InlineData("@Artyom по поводу")]
        [InlineData("@Laima прошу блины!")]
        [InlineData("эй админ @harry0xfefecaca верни бота")]
        public void IgnoreMentions(string text) => Assert.Null(ubl.UnbleepSwears(text));

        [Theory]
        [InlineData("x - floor(abs(x)) * sign(x) -- вроде такая формула для frac(x)")]
        public void IgnoresWeirdShit(string text) => Assert.Null(ubl.UnbleepSwears(text));

        [Theory]
        [InlineData("/poll")]
        [InlineData("/roll 2d6")]
        [InlineData("/award medal")]
        [InlineData("/status@MinecraftServerBot")]
        [InlineData("/broadcast@MinecraftServerBot пи#*ец вы понастроили тут")]
        [InlineData("/ban@MinecraftServerBot @dirty_johnny86")]
        public void IgnoreCommands(string text) => Assert.Null(ubl.UnbleepSwears(text));

        [Theory]
        [InlineData("#UEeğitimKarazin")]
        [InlineData("#KöksalBabaCafeTrabzonda")]
        [InlineData("#ZehraHanımSüresizeKadro")]
        [InlineData("#define")]
        [InlineData("#ifndef")]
        [InlineData("#trashtag")]
        [InlineData("#MeToo")]
        [InlineData("#инстаграм")]
        [InlineData("#битваБлогеров")]
        [InlineData("#зенитахмат")]
        [InlineData("#HappyKWONJIYONGDay")]
        [InlineData("#MCITOT")]
        [InlineData("#ТамбовКраснодар")]
        [InlineData("#JRockконвент2019")]
        [InlineData("#DonaldTrumpAgain")]
        [InlineData("#ZodiacKillerStrikesAgain")]
        [InlineData("#ThanksObama")]
        [InlineData("#BalıkBurcuKızıylaEvlenmek")]
        public void IgnoreHashtags(string text) => Assert.Null(ubl.UnbleepSwears(text));

    }
}
