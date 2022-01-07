namespace UniqueFileGenerator;

public sealed class CustomStringFactory
{
    private readonly IReadOnlyDictionary<CharacterType, string> CharacterDictionary =
        new Dictionary<CharacterType, string>()
        {
            { CharacterType.UpperCaseLetter, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
            { CharacterType.LowerCaseLetter, "abcdefghijklmnopqrstuvwxyz" },
            { CharacterType.Number,          "0123456789" }
        };

    /// <summary>
    /// Returns a custom string based upon the requested character type(s).
    /// </summary>
    /// <param name="charTypes">One or more CharacterTypes.</param>
    public string CreateCustomString(CharacterType charTypes)
    {
        ArgumentNullException.ThrowIfNull(nameof(charTypes));

        var sb = new System.Text.StringBuilder();

        // Iterate through the character types, appending only relevant text.
        foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
        {
            if (charTypes.HasFlag(type))
                sb.Append(CharacterDictionary[type]);
        }

        return sb.ToString();
    }
}