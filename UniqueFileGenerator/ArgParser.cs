namespace UniqueFileGenerator;

/// <summary>
/// A DTO for passing parsed arguments from the user.
/// </summary>
public sealed record ParsedArguments(
    uint FileCount,
    IReadOnlyDictionary<string, string> FlagValueMap);

/// <summary>
/// Contains logic for parsing arguments passed from the user.
/// </summary>
public static class ArgParser
{
    private static readonly IReadOnlyList<string> SupportedFlags =
        new List<string>() {
            "-p", // Prefix
            "-e", // Extension
            "-o", // Output directory
            "-s", // Size
            "-d"  // Delay
        };

    /// <summary>
    /// Parsed arguments passed from the user.
    /// </summary>
    /// <param name="args"></param>
    public static ParsedArguments ParseArgs(string[] args)
    {
        if (args.Length == 0)
            throw new ArgumentException(Resources.FileCountMissing, nameof(args));

        var argQueue = new Queue<string>(args);

        var fileCountText = argQueue.Dequeue().Replace(",", "");
        if (!uint.TryParse(fileCountText, out var fileCount))
        {
            // Parsing failed. If all characters are digits,
            // then a number that was too large was provided.
            if (fileCountText.All(char.IsDigit))
                throw new InvalidOperationException(Resources.FileCountTooHigh);

            // Otherwise, some invalid characters were provided.
            throw new InvalidOperationException(Resources.FileCountInvalidRange);
        }

        if (fileCount == 0)
            throw new ArgumentOutOfRangeException(Resources.FileCountInvalidZero);

        var argDict = new Dictionary<string, string>(SupportedFlags.Count);

        // Iterate through the args. Any non-flag arg is considered a value for the last-processed flag.
        // If there are multiple args for any such flag, they will be combined in a single string.
        var currentFlag = "";
        while (argQueue.Any())
        {
            var thisArg = argQueue.Dequeue();

            if (SupportedFlags.Contains(thisArg))
            {
                // Flags must not be used twice.
                if (argDict.ContainsKey(thisArg))
                    throw new InvalidOperationException(Resources.FlagCanBeUsedOnce);

                currentFlag = thisArg;
            }
            // Treat the arg as a value for the current flag.
            else
            {
                if (IsInFlagFormat(thisArg))
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.FlagInvalid, thisArg));
                }

                if (string.IsNullOrWhiteSpace(currentFlag))
                {
                    throw new InvalidOperationException(
                        string.Format(Resources.ValueWithNoFlag, thisArg));
                }

                // Appends the value to current flag's value string.
                if (argDict.ContainsKey(currentFlag))
                    argDict[currentFlag] += " " + thisArg;
                else
                    argDict.Add(currentFlag, thisArg);
            }
        }

        return new ParsedArguments(fileCount, argDict);
    }

    /// <summary>
    /// Determines if the given text in the format of a command line flag argument (e.g., "-e").
    /// </summary>
    /// <param name="text"></param>
    private static bool IsInFlagFormat(string text)
    {
        return !string.IsNullOrWhiteSpace(text) &&
            text.Length == 2 &&
            text[0] == '-';
    }
}
