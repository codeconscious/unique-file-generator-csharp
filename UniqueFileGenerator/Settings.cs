namespace UniqueFileGenerator;

public class Settings
{
    public int Count { get; init; }
    public int Digits { get; init; }
    public string Prefix { get; init; }
    public string Extension { get; init; }
    public string OutputDirectory { get; init; }
    public int? SizeInBytes { get; init; }

    private static readonly List<string> SupportedFlags = new() { "-p", "-e", "-o", "-s" };

    public Settings(string[] args)
    {
        var (count, argPairs) = ParseArgs(args);

        Count = count;

        Digits = count.ToString().Length;

        Prefix = argPairs.ContainsKey("-p")
            ? argPairs["-p"]
            : string.Empty;

        Extension = argPairs.ContainsKey("-e")
            ? "." + argPairs["-e"]
            : string.Empty;

        OutputDirectory = argPairs.ContainsKey("-o")
            ? "." + Path.DirectorySeparatorChar + argPairs["-o"] + Path.DirectorySeparatorChar
            : "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;

        SizeInBytes = argPairs.ContainsKey("-s")
            ? int.Parse(argPairs["-s"])
            : null;
    }

    private static (int Count, Dictionary<string, string> argDict) ParseArgs(string[] args)
    {
        if (args.Length == 0)
            throw new ArgumentException("The count must be specified", nameof(args));

        var argQueue = new Queue<string>(args);

        var countText = argQueue.Dequeue();
        if (!int.TryParse(countText, out var count))
            throw new InvalidOperationException("You must enter a count as the first argument.");
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count), "An invalid file count was supplied.");

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
                    throw new InvalidOperationException("Was a flag specified?");

                if (argDict.ContainsKey(currentFlag))
                    argDict[currentFlag] += " " + thisArg;
                else
                    argDict.Add(currentFlag, thisArg);
            }
        }

        // For testing only.
        // WriteLine("Count: " + count);
        // foreach (var arg in argDict)
        //     WriteLine($"{arg.Key}: {arg.Value}");

        return (count, argDict);
    }
}