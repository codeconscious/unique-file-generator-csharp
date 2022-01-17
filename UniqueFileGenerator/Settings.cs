namespace UniqueFileGenerator;

public class Settings
{
    public uint FileCount { get;}
    public string Prefix { get; }
    public string Extension { get; }
    public string OutputDirectory { get; }
    public int? SizeInBytes { get; }
    public int FileCreationDelay { get; }
    public CharacterType CharacterTypes { get; }

    public long DiskSpaceNecessary => FileCount * (SizeInBytes ?? (Prefix.Length + 10));
    public bool IsHighFileCount => FileCount > 50_000;
    public bool IsLargeSize => SizeInBytes > 100_000_000;

    private static readonly IReadOnlyList<string> SupportedFlags =
        new List<string>() { "-p", "-e", "-o", "-s", "-d" };

    public Settings(string[] args)
    {
        var (fileCount, argDict) = ParseArgs(args);

        FileCount = fileCount;

        // Only add a post-prefix space when the last character is not alphanumeric.
        Prefix = argDict.ContainsKey("-p")
            ? argDict["-p"] +(IsLastCharAlphanumeric(argDict["-p"]) ? " " : string.Empty)
            : string.Empty;

        Extension = argDict.ContainsKey("-e")
            ? EnforceStartingPeriod(argDict["-e"])
            : string.Empty;

        OutputDirectory = argDict.ContainsKey("-o")
            ? "." + Path.DirectorySeparatorChar + argDict["-o"] + Path.DirectorySeparatorChar
            : "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;

        // TODO: Accept sizes in formats like "30KB" or "10.4MB"
        if (argDict.ContainsKey("-s"))
        {
            if (int.TryParse(argDict["-s"], out var parsedSize))
            {
                SizeInBytes = parsedSize switch
                {
                    0 => throw new ArgumentOutOfRangeException(
                            nameof(parsedSize), ResourceStrings.FileSizeInvalidZero),
                    _ => parsedSize
                };
            }
            else // A non-numeric value was passed in.
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parsedSize), ResourceStrings.FileSizeInvalidRange);
            }
        }

        // Parse the file creation delay, if provided.
        if (argDict.ContainsKey("-d") &&
            int.TryParse(argDict["-d"], out var parsedDelay))
        {
            FileCreationDelay = parsedDelay switch
            {
                < 0 => throw new ArgumentOutOfRangeException(
                            nameof(parsedDelay), ResourceStrings.FileCreationDelayOutOfRange),
                _ => parsedDelay
            };
        }

        // TODO: Add a command line flag to specify character types. (Separate for filenames and content?)
        CharacterTypes = CharacterType.UpperCaseLetter |
                         CharacterType.Number;
    }

    private static (uint Count, Dictionary<string, string> argDict) ParseArgs(string[] args)
    {
        if (args.Length == 0)
            throw new ArgumentException(ResourceStrings.FileCountMissing, nameof(args));

        var argQueue = new Queue<string>(args);

        if (!uint.TryParse(argQueue.Dequeue(), out var fileCount))
            throw new InvalidOperationException(ResourceStrings.FileCountInvalidRange);

        if (fileCount == 0)
            throw new ArgumentOutOfRangeException(
                nameof(fileCount), ResourceStrings.FileCountInvalidZero);

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
                    throw new InvalidOperationException(ResourceStrings.FlagCanBeUsedOnce);

                currentFlag = thisArg;
            }
            else // Not a flag, so treat as a flag value.
            {
                if (string.IsNullOrWhiteSpace(currentFlag))
                    throw new InvalidOperationException(
                        ResourceStrings.ValueWithNoFlag_Prefix + thisArg);

                if (argDict.ContainsKey(currentFlag))
                    argDict[currentFlag] += " " + thisArg;
                else
                    argDict.Add(currentFlag, thisArg);
            }
        }

        return (fileCount, argDict);
    }

    public bool ShouldProceedDespiteHighValues()
    {
        // If the user requested a high number of files, confirm the operation.
        if (IsHighFileCount && !AnsiConsole.Confirm(ResourceStrings.CountWarning))
            return false;

        // If the user requested a very large file sizes, confirm the operation.
        if (IsLargeSize && !AnsiConsole.Confirm(ResourceStrings.SizeWarning))
            return false;

        return true;
    }

    private static bool IsLastCharAlphanumeric(string text) =>
        !string.IsNullOrEmpty(text) && char.IsLetterOrDigit(text[^1]);

    private static string EnforceStartingPeriod(string text) =>
        (text[0] == '.' ? string.Empty : ".") + text;
}