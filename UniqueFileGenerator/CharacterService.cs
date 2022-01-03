namespace UniqueFileGenerator;

/// <summary>
/// Sets up a collection of characters that can be chosen at random.
/// </summary>
public sealed class CharacterService
{
    private readonly Dictionary<CharacterType, string> CharacterDictionary = new()
    {
        { CharacterType.UpperCaseLetter, "ABCDEFGHIJKLMNOPQRSTUVWXYZ" },
        { CharacterType.LowerCaseLetter, "abcdefghijklmnopqrstuvwxyz" },
        { CharacterType.Numeric,         "0123456789" }
    };

    public string GetCharacters(CharacterType charTypes)
    {
        ArgumentNullException.ThrowIfNull(nameof(charTypes));

        var sb = new System.Text.StringBuilder();

        // Iterate through the character types, adding relevant text.
        foreach (CharacterType type in Enum.GetValues(typeof(CharacterType)))
        {
            if (charTypes.HasFlag(type))
                sb.Append(CharacterDictionary[type]);
        }

        return sb.ToString();
    }
}