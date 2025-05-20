using Bogus.DataSets;

namespace Api.IntegrationTests.Infrastructure.Extensions;

internal static class InternetExtensions
{
    public static string PasswordCustom(this Internet internet, int minLength, int maxLength)
    {
        var r = internet.Random;

        var lowercase = r.Char('a', 'z').ToString();
        var uppercase = r.Char('A', 'Z').ToString();
        var number = r.Char('0', '9').ToString();
        var symbol = r.Char('!', '/').ToString();
        var padding = r.String2(minLength - 4);
        var padding2 = r.String2(r.Number(0, maxLength - minLength));

        var chars = (lowercase + uppercase + number + symbol + padding + padding2).ToArray();
        var shuffledChars = r.Shuffle(chars).ToArray();

        return new string(shuffledChars);
    }
}