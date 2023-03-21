using System.IO;

namespace UniqueFileGenerator;

/// <summary>
/// The configuration settings used to generate new files.
/// </summary>
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
    /// The size of each created file, if specified.
    /// Otherwise, a default size is used.
    /// </summary>
    public uint? SizeInBytes { get; }

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
    public long DiskSpaceNecessary =>
        FileCount * (SizeInBytes ?? (uint)(Prefix.Length + RandomStringLength));

    public bool IsHighFileCount => FileCount > 50_000;
    public bool IsLargeSize => SizeInBytes > 100_000_000;
    public bool IsLongDelay => FileCreationDelayMs > 60_000; // 1m

    public const ushort RandomStringLength = 10; // This could be configurable too.

    public Settings(ParsedArguments parsedArgs)
    {
        FileCount = parsedArgs.FileCount;

        var args = parsedArgs.FlagValueMap;

        // Only add a post-prefix space when the last character is not alphanumeric.
        Prefix = args.ContainsKey("-p")
            ? args["-p"] + (IsLastCharAlphanumeric(args["-p"]) ? " " : string.Empty)
            : string.Empty;

        Extension = args.ContainsKey("-e")
            ? EnforceStartingPeriod(args["-e"])
            : string.Empty;

        OutputDirectory = (args.ContainsKey("-o") ? Path.Combine(".", args["-o"])
                                                  : Path.Combine(".", "output"))
                          + Path.DirectorySeparatorChar;

        // Parse the requested file size, if provided.
        // TODO: Accept sizes in formats like "30KB" or "10.4MB"
        if (args.ContainsKey("-s"))
        {
            if (uint.TryParse(args["-s"], out var parsedSize))
            {
                SizeInBytes = parsedSize switch
                {
                    0 => throw new ArgumentOutOfRangeException(Resources.FileSizeInvalidZero),
                    _ => parsedSize
                };
            }
            else // A non-numeric size was passed in.
            {
                throw new ArgumentOutOfRangeException(Resources.FileSizeInvalidRange);
            }
        }

        // Parse the file creation delay, if provided.
        if (args.ContainsKey("-d") &&
            int.TryParse(args["-d"], out var parsedDelay))
        {
            FileCreationDelayMs = parsedDelay switch
            {
                < 0 => throw new ArgumentOutOfRangeException(Resources.FileCreationDelayOutOfRange),
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

        // If the user requested very large file sizes, confirm the operation.
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
