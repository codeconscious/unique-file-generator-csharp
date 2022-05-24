namespace UniqueFileGenerator;

public record Arguments(uint Count, Dictionary<string, string> argDict);

public static class ArgParser
{
    private static readonly IReadOnlyList<string> SupportedFlags =
        new List<string>() { "-p", "-e", "-o", "-s", "-d" };

    public static Arguments ParseArgs(string[] args)
    {
        if (args.Length == 0)
            throw new ArgumentException(Resources.FileCountMissing, nameof(args));

        var argQueue = new Queue<string>(args);

        var fileCountText = argQueue.Dequeue().Replace(",", "");
        if (!uint.TryParse(fileCountText, out var fileCount))
        {
            // If all characters are digits, then a number that was too high was provided.
            if (fileCountText.All(char.IsDigit))
                throw new InvalidOperationException(Resources.FileCountTooHigh);

            // Otherwise, some invalid string was provided --
            // e.g., negative number, letters, symbols, etc.
            throw new InvalidOperationException(Resources.FileCountInvalidRange);
        }

        if (fileCount == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fileCount), Resources.FileCountInvalidZero);
        }

        var argDict = new Dictionary<string, string>();

        // Iterate through the args. Any non-flag arg is considered a value for the last-processed flag.
        // If there are multiple args for any such flag, they will be combined in a single string.
        var currentFlag = "";
        while (argQueue.Any())
        {
            var thisArg = argQueue.Dequeue();

            // If this is a supported flag.
            if (SupportedFlags.Contains(thisArg))
            {
                // Flags cannot be used twice.
                if (argDict.ContainsKey(thisArg))
                    throw new InvalidOperationException(Resources.FlagCanBeUsedOnce);

                currentFlag = thisArg;
            }
            // Otherwise, consider this a value for the current flag.
            else
            {
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

        return new Arguments(fileCount, argDict);
    }
}