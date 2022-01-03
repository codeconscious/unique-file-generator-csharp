namespace UniqueFileGenerator;

public sealed class RandomCharacterGenerator
{
    private const string AlphanumericBank = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string NumericBank = "0123456789";

    private static Random Random { get; } = new();

    public IEnumerable<string> GetCharacterCollection(int stringCount, int stringLength, bool numbersOnly)
    {
        if (stringCount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stringCount), "The count of desired strings must be greater than zero.");
        }

        if (stringLength < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(stringLength), "The length of each string must be greater than zero.");
        }

        if (stringCount == 0)
            return Enumerable.Empty<string>();

        var set = new HashSet<string>();

        while (set.Count < stringCount)
        {
            set.Add(GetChars(stringLength, numbersOnly));
        }

        return set;
    }

    private string GetChars(int length, bool numbersOnly)
    {
        if (length == 0)
            return string.Empty;

        var bank = numbersOnly ? NumericBank : AlphanumericBank;

        var outputChars = new char[length];

        for (int i = 0; i < outputChars.Length; i++)
        {
            outputChars[i] = bank[Random.Next(bank.Length)];
        }

        return new string(outputChars);
    }
}