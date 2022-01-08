namespace UniqueFileGenerator;

public class Settings
{
    public int FileCount { get;}
    public string Prefix { get; }
    public string Extension { get; }
    public string OutputDirectory { get; }
    public int? SizeInBytes { get; }
    public CharacterType CharacterTypes { get; }

    private static readonly IReadOnlyList<string> SupportedFlags =
        new List<string>() { "-p", "-e", "-o", "-s" };

    public Settings(string[] args)
    {
        var (fileCount, argDict) = ParseArgs(args);

        FileCount = fileCount;

        // Only add a post-prefix space when the last character is not alphanumeric.
        Prefix = argDict.ContainsKey("-p")
            ? argDict["-p"] +(IsLastCharAlphanumeric(argDict["-p"]) ? " " : string.Empty)
            : string.Empty;

        Extension = argDict.ContainsKey("-e")
            ? (argDict["-e"][0] == '.' ? "" : ".") + argDict["-e"]
            : string.Empty;

        OutputDirectory = argDict.ContainsKey("-o")
            ? "." + Path.DirectorySeparatorChar + argDict["-o"] + Path.DirectorySeparatorChar
            : "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;

        SizeInBytes = argDict.ContainsKey("-s")
            ? int.Parse(argDict["-s"])
            : null;

        // TODO: Add a command line flag to specify character types. (Separate for filenames and content?)
        CharacterTypes = CharacterType.UpperCaseLetter |
                         CharacterType.Number;
    }

    private static (int Count, Dictionary<string, string> argDict) ParseArgs(string[] args)
    {
        if (args.Length == 0)
            throw new ArgumentException("The file count must be specified.", nameof(args));

        var argQueue = new Queue<string>(args);

        if (!int.TryParse(argQueue.Dequeue(), out var fileCount))
        {
            throw new InvalidOperationException(
                "You must enter a file count as the first argument.");
        }

        if (fileCount < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileCount), "An invalid file count was supplied.");
        }

        var argDict = new Dictionary<string, string>();

        // Iterate through the args. Any non-flag arg is considered to be related to the previous one.
        // If there are multiple args for any such flag, they will be combined in a single string.
        var currentFlag = "";
        while (argQueue.Any())
        {
            var thisArg = argQueue.Dequeue();

            if (SupportedFlags.Contains(thisArg))
            {
                if (argDict.ContainsKey(thisArg))
                    throw new InvalidOperationException("A flag can only be specified once.");
                else
                    currentFlag = thisArg;
            }
            else // Not a flag, so treat as a value.
            {
                if (string.IsNullOrWhiteSpace(currentFlag))
                {
                    throw new InvalidOperationException(
                        $"A flag was not specified for {thisArg}.");
                }

                if (argDict.ContainsKey(currentFlag))
                    argDict[currentFlag] += " " + thisArg;
                else
                    argDict.Add(currentFlag, thisArg);
            }
        }

        return (fileCount, argDict);
    }

    private bool IsLastCharAlphanumeric(string text) =>
        !string.IsNullOrEmpty(text) && char.IsLetterOrDigit(text[^1]);
}