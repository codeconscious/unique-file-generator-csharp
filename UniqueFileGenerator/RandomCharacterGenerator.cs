namespace UniqueFileGenerator;

public sealed class RandomCharacterGenerator
{
    private const string AlphanumericBank = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string NumericBank = "0123456789";

    private static Random Random { get; } = new();

    public string GetChars(uint count, bool numbersOnly)
    {
        if (count == 0)
            return string.Empty;

        var bank = numbersOnly ? NumericBank : AlphanumericBank;

        var outputChars = new char[count];

        for (int i = 0; i < outputChars.Length; i++)
        {
            outputChars[i] = bank[Random.Next(bank.Length)];
        }

        return new string(outputChars);
    }
}