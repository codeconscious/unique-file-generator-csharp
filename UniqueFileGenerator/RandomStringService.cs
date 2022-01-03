namespace UniqueFileGenerator;

public sealed class RandomStringService
{
    private string CharacterBank { get; }
    private static Random Random { get; } = new();

    public RandomStringService(string characterBank)
    {
        if (string.IsNullOrWhiteSpace(characterBank))
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterBank), "The character bank cannot be empty.");
        }

        CharacterBank = characterBank;
    }

    public IEnumerable<string> GetCharacterCollection(int count, int lengthOfEach)
    {
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count), "The count of desired strings must be greater than zero.");
        }

        if (lengthOfEach < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lengthOfEach), "The length of each string must be greater than zero.");
        }

        if (count == 0)
            return Enumerable.Empty<string>();

        var set = new HashSet<string>();

        while (set.Count < count)
        {
            set.Add(GetRandomChars(lengthOfEach));
        }

        return set;
    }

    private string GetRandomChars(int length)
    {
        if (length == 0)
            return string.Empty;

        var outputChars = new char[length];

        for (int i = 0; i < outputChars.Length; i++)
        {
            outputChars[i] = CharacterBank[Random.Next(CharacterBank.Length)];
        }

        return new string(outputChars);
    }
}