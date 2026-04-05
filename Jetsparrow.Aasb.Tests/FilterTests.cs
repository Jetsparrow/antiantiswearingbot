namespace Jetsparrow.Aasb.Tests;
public class FilterTests : BleepTestsBase
{
    [Theory]
    [InlineData("*")]
    [InlineData("**#")]
    [InlineData("@*#")]
    public async Task IgnoreShortGrawlixesWithoutLetters(string text)
    {
        if (text.Length < DefaultSettings.Unbleeper.MinAmbiguousWordLength)
            Assert.Null(await ubl.UnbleepSwears(text));
    }

    [Theory]
    [InlineData("*")]
    [InlineData("*б")]
    [InlineData("х#")]
    public async Task IgnoreShortWords(string text)
    {
        if (text.Length < DefaultSettings.Unbleeper.MinWordLength)
            Assert.Null(await ubl.UnbleepSwears(text));
    }

    [Theory]
    [InlineData("@pvkuznetsov https://github.com/jacksondunstan/UnityNativeScripting")]
    [InlineData("@JohnnyMnemonic")]
    [InlineData("@Artyom по поводу")]
    [InlineData("@Laima прошу блины!")]
    [InlineData("эй админ @harry0xfefecaca верни бота")]
    public async Task IgnoreMentions(string text) => Assert.Null(await ubl.UnbleepSwears(text));

    [Theory]
    [InlineData("x - floor(abs(x)) * sign(x) -- вроде такая формула для frac(x)")]
    public async Task IgnoresWeirdShit(string text) => Assert.Null(await ubl.UnbleepSwears(text));

    [Theory]
    [InlineData("/poll")]
    [InlineData("/roll 2d6")]
    [InlineData("/award medal")]
    [InlineData("/status@MinecraftServerBot")]
    [InlineData("/broadcast@MinecraftServerBot пи#*ец вы понастроили тут")]
    [InlineData("/ban@MinecraftServerBot @dirty_johnny86")]
    public async Task IgnoreCommands(string text) => Assert.Null(await ubl.UnbleepSwears(text));

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
    public async Task IgnoreHashtags(string text) => Assert.Null(await ubl.UnbleepSwears(text));

    [Theory]
    [InlineData("ipetrov@mail.ru")]
    [InlineData("ipetrov@русская.mail.ru")]
    [InlineData("ипетров@почта.рф")]
    [InlineData("admin@local")]
    [InlineData("админ@local")]
    public async Task IgnoreEmails(string text) => Assert.Null(await ubl.UnbleepSwears(text));

}
