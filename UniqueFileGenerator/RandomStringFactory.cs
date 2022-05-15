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
    /// All characters that can be randomly selected by this class's other methods.
    /// </summary>
    private string CharacterBank { get; }

    private Random Random { get; } = new();

    /// <summary>
    /// Constructor that populates the character bank.
    /// </summary>
    /// <param name="charTypes">An enum representing one or more character types.</param>
    public RandomStringFactory(CharacterType charTypes)
    {
        ArgumentNullException.ThrowIfNull(nameof(charTypes));

        var sb = new System.Text.StringBuilder();

        // Get the combined distinct characters for each provided character type.
        var chars = string.Concat(
            Enum.GetValues(typeof(CharacterType))
                .Cast<CharacterType>()
                .Where(type => charTypes.HasFlag(type))
                .SelectMany(type => CharactersByCategory[type])
                .Distinct());

        sb.Append(chars);

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

        CharacterBank = sb.ToString();
    }

    /// <summary>
    /// Creates a single string of a specific length using random characters
    /// from the character bank.
    /// </summary>
    /// <param name="length">The desired string length.</param>
    public string CreateUniqueString(int length)
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