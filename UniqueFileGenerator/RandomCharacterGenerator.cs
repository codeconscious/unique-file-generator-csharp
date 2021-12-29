namespace UniqueFileGenerator;

public sealed class RandomCharacterGenerator
{
    private const string AlphanumericBank = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    private const string NumericBank = "0123456789";

    private static Random Random { get; } = new();

    public IEnumerable<string> GetCharacterSet(int items, int length, bool numbersOnly)
    {
        if (items < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(items), "The number of items must be greater than zero.");
        }

        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length), "The length must be greater than zero.");
        }

        if (items == 0)
            return Enumerable.Empty<string>();

        var set = new HashSet<string>();

        while (set.Count < items)
        {
            set.Add(GetChars(length, numbersOnly));
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