using System.IO;

namespace UniqueFileGenerator;

public sealed class Settings
{
    /// <summary>
    /// The total number of files to create.
    /// </summary>
    public uint FileCount { get; }

    /// <summary>
    /// Text that should be prepend to each file name.
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// The extension for each created file.
    /// </summary>
    public string Extension { get; }

    /// <summary>
    /// The directory in which the files should be created.
    /// </summary>
    public string OutputDirectory { get; }

    /// <summary>
    /// The size of each created file.
    /// </summary>
    public int? SizeInBytes { get; }

    /// <summary>
    /// A delay in milliseconds to add between the creation of each file.
    /// </summary>
    public int FileCreationDelayMs { get; }

    /// <summary>
    /// The type or types of random characters that should be used.
    /// </summary>
    public CharacterType CharacterTypes { get; }

    /// <summary>
    /// The amount of disk space that is necessary for the operation.
    /// </summary>
    public long DiskSpaceNecessary => FileCount * (SizeInBytes ?? (Prefix.Length + 10));

    public bool IsHighFileCount => FileCount > 50_000;
    public bool IsLargeSize => SizeInBytes > 100_000_000;
    public bool IsLongDelay => FileCreationDelayMs > 60_000; // 1m

    public Settings(Arguments args)
    {
        FileCount = args.Count;

        var argDict = args.argDict;

        // Only add a post-prefix space when the last character is not alphanumeric.
        Prefix = argDict.ContainsKey("-p")
            ? argDict["-p"] + (IsLastCharAlphanumeric(argDict["-p"]) ? " " : string.Empty)
            : string.Empty;

        Extension = argDict.ContainsKey("-e")
            ? EnforceStartingPeriod(argDict["-e"])
            : string.Empty;

        OutputDirectory = (argDict.ContainsKey("-o") ? Path.Combine(".", argDict["-o"])
                                                     : Path.Combine(".", "output"))
                          + Path.DirectorySeparatorChar;

        // Parse the requested file size, if provided.
        // TODO: Accept sizes in formats like "30KB" or "10.4MB"
        if (argDict.ContainsKey("-s"))
        {
            if (int.TryParse(argDict["-s"], out var parsedSize))
            {
                SizeInBytes = parsedSize switch
                {
                    0 => throw new ArgumentOutOfRangeException(
                            nameof(parsedSize),
                            Resources.FileSizeInvalidZero),
                    _ => parsedSize
                };
            }
            else // A non-numeric value was passed in.
            {
                throw new ArgumentOutOfRangeException(
                    nameof(parsedSize),
                    Resources.FileSizeInvalidRange);
            }
        }

        // Parse the file creation delay, if provided.
        if (argDict.ContainsKey("-d") &&
            int.TryParse(argDict["-d"], out var parsedDelay))
        {
            FileCreationDelayMs = parsedDelay switch
            {
                < 0 => throw new ArgumentOutOfRangeException(
                            nameof(parsedDelay),
                            Resources.FileCreationDelayOutOfRange),
                _ => parsedDelay
            };
        }

        // TODO: Add a command line flag to specify character types. (Separate for filenames and content?)
        CharacterTypes = CharacterType.UpperCaseLetter |
                         CharacterType.Number;
    }



    /// <summary>
    /// Confirm with the user if they wish to continue despite higher-than-expected values.
    /// </summary>
    /// <returns>A bool indicating whether to continue (true) or cancel.</returns>
    public bool ShouldProceedDespiteHighValues()
    {
        // If the user requested a high number of files, confirm the operation.
        if (IsHighFileCount && !AnsiConsole.Confirm(Resources.CountWarning))
            return false;

        // If the user requested a very large file sizes, confirm the operation.
        if (IsLargeSize && !AnsiConsole.Confirm(Resources.SizeWarning))
            return false;

        // If the user requested a very long delay between each file, confirm the operation.
        if (IsLongDelay && !AnsiConsole.Confirm(Resources.DelayWarning))
            return false;

        return true;
    }

    private static bool IsLastCharAlphanumeric(string text) =>
        !string.IsNullOrEmpty(text) && char.IsLetterOrDigit(text[^1]);

    private static string EnforceStartingPeriod(string text) =>
        (text[0] == '.' ? string.Empty : ".") + text;
}