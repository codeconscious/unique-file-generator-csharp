namespace UniqueFileGenerator;

/// <summary>
/// Handles all operations for generating random strings.
/// </summary>
public sealed class RandomStringFactory
{
    private readonly IReadOnlyDictionary<CharacterType, string> CharactersByCategory =
        new Dictionary<CharacterType, string>()
        {
            { CharacterType.UpperCaseLetter, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { CharacterType.LowerCaseLetter, "abcdefghijklmnopqrstuvwxyz" },
            { CharacterType.Number,          "0123456789" }
        };

    /// <summary>
    /// All characters that can be randomly selected by this class's methods.
    /// </summary>
    private string CharacterBank { get; }
    private Random Random { get; } = new();

    /// <summary>
    /// Constructor that populates the character bank.
    /// </summary>
    public RandomStringFactory(CharacterType charTypes)
    {
        ArgumentNullException.ThrowIfNull(nameof(charTypes));

        var sb = new System.Text.StringBuilder();

        // Iterate through the character types, appending only relevant text.
        foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
        {
            if (charTypes.HasFlag(type))
                sb.Append(CharactersByCategory[type]);
        }

        if (sb.Length == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sb), ResourceStrings.CharBankEmpty);
        }

        if (sb.Length == 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sb), ResourceStrings.CharBankTooShort);
        }

        CharacterBank =  sb.ToString();
    }

    /// <summary>
    /// Creates a collection of strings, each containing random characters
    /// from the character bank, according to the settings provided.
    /// </summary>
    /// <param name="count">The number of strings.</param>
    /// <param name="lengthOfEach">The length of each string.</param>
    public IEnumerable<string> CreateUniqueStrings(int count, int lengthOfEach)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), ResourceStrings.StringCountZero);
        }

        if (lengthOfEach < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lengthOfEach), ResourceStrings.StringLengthZero);
        }

        if (count == 0)
            return Enumerable.Empty<string>();

        var set = new HashSet<string>();

        while (set.Count < count)
        {
            set.Add(CreateSingleUniqueString(lengthOfEach));
        }

        return set;
    }

    /// <summary>
    /// Creates a single string of a specific length using random characters
    /// from the character bank.
    /// </summary>
    /// <param name="length">The desired string length.</param>
    public string CreateSingleUniqueString(int length)
    {
        if (length < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length), ResourceStrings.LengthInvalidNegative);
        }

        if (length == 0)
            return string.Empty;

        var outputChars = new char[length];

        for (var i = 0; i < outputChars.Length; i++)
        {
            outputChars[i] = CharacterBank[Random.Next(CharacterBank.Length)];
        }

        return new string(outputChars);
    }
}