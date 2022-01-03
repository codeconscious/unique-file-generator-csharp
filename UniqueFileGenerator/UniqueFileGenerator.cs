global using static System.Console;
using System.Text;
using Spectre.Console;

namespace UniqueFileGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
// #if DEBUG
//             args = new[] { "20", "-p", "TEST-" };
// #endif

        if (args.Length == 0)
        {
            PrintInstructions();
            return;
        }

        Settings settings;
        try
        {
            settings = new Settings(args);
            SaveFiles(settings);
        }
        catch (Exception ex)
        {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenTypes);
                return;
        }

        WriteLine($"{settings.Count} files created.");
    }

    private static void SaveFiles(Settings settings)
    {
        Directory.CreateDirectory(settings.OutputDirectory);

        var random = new Random();

        // Only add a space after a prefix if a prefix is specified
        // and its last character is not a special no-space one.
        var postPrefixDivider = settings.Prefix switch
        {
            { Length: > 0 } when char.IsLetterOrDigit(settings.Prefix[^1]) => " ",
            _ => string.Empty
        };

        var rndCharGenerator = new RandomCharacterGenerator();

        //var idQueue = new Queue<string>(settings.Count);
        var baseFileNameQueue = rndCharGenerator.GetCharacterSet(settings.Count, 10, true);
        var prefixedFileNameQueue = new Queue<string>(baseFileNameQueue.Select(n => settings.Prefix + postPrefixDivider + n));

        var contentQueue = settings.SizeInBytes.HasValue
            ? new Queue<string>(rndCharGenerator.GetCharacterSet(settings.Count, settings.SizeInBytes.Value, true))
            : new Queue<string>(prefixedFileNameQueue);

        while (prefixedFileNameQueue.Any())
        {
            var fileName = prefixedFileNameQueue.Dequeue();
            var path = settings.OutputDirectory + fileName + settings.Extension;
            var content = new UTF8Encoding(true).GetBytes(contentQueue.Dequeue());

            using var fileStream = File.Create(path);
            fileStream.Write(content, 0, content.Length);
        }
    }

    private static void PrintInstructions()
    {
        var outerTable = new Table();
        outerTable.AddColumn("Unique File Generator");
        outerTable.AddRow("Create unique files with unique contents using random numbers.");
        outerTable.AddEmptyRow();

        const string usage = "Usage: Pass in the number of files and, optionally, add additional arguments.\n\n" +
            "Examples:\n" +
            "   uniquefilegen 10  [gray](Creates 10 files with the default settings)[/]\n" +
            "   uniquefilegen 1000 -p TEST- e txt  [gray](Creates 1,000 files in the format \"TEST-12345.txt\")[/]";
        outerTable.AddRow(usage);
        outerTable.AddEmptyRow();
        // outerTable.AddRow("Usage: uniquefilegen [count] [arguments]");
        // outerTable.AddRow("Example: uniquefilegen 10 [arguments]");

        var argTable = new Table();
        argTable.Border(TableBorder.None);
        argTable.AddColumn("Arg");
        argTable.AddColumn("Description");
        argTable.HideHeaders();
        argTable.Columns[0].PadRight(3);
        argTable.AddRow("-p", "File name prefix. If the last character is alphanumeric, a space will be added after it.");
        argTable.AddRow("-e", "The desired file extension (with no opening period).");
        argTable.AddRow("-o", "Specify an output subfolder. Defaults to \"output\". Multiple terms are okay.");
        argTable.AddRow("-s", "The size of each file in bytes. (They will be populated with random alphanumeric characters.)");

        outerTable.AddRow(argTable);

        AnsiConsole.Write(outerTable);
    }
}