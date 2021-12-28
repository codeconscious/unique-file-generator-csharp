using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using Spectre.Console;

namespace UniqueFileGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                var outerTable = new Table();
                outerTable.AddColumn("Unique File Generator");
                outerTable.AddRow("Create unique files with unique contents using random numbers.");
                outerTable.AddEmptyRow();

                const string usage = "Usage: Pass in the number of files, and then optionally add any additional arguments.\n\n" +
                    "Examples:\n" +
                    "   uniquefilegen 10  (Creates 10 files with the default settings)\n" +
                    "   uniquefilegen 1000 -p TEST- e txt  (Creates 1,000 files in the format \"TEST-12345.txt\")";
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
                argTable.AddRow("-p", "File name prefix. A space will be added afterward unless it ends with - or _.");
                argTable.AddRow("-e", "The desired file extension. (The opening period is unnecessary.)");
                argTable.AddRow("-o", "Specify an output subfolder. Defaults to \"output\"");
                //table.AddRow("-s", "The size of each file [Not support yet]");

                outerTable.AddRow(argTable);

                AnsiConsole.Write(outerTable);
                return;
            }

            Settings settings;
            try
            {
                settings = new Settings(args);
            }
            catch (System.Exception ex)
            {
                 AnsiConsole.WriteException(ex, ExceptionFormats.ShortenTypes);
                 return;
            }

            Directory.CreateDirectory(settings.OutputDirectory);

            var random = new Random();
            var noSpaceChars = new char[] { '_', '-' }; // Or any non-alphanumeric char?

            // TODO: Support checking for used ints.
            //var usedValues = new List<int>();

            // IDEA: Perhaps generate the numbers in a hashset or queue first, then dequeue them.

            for (var i = 0; i < settings.Count; i++)
            {
                var rndNumber = random.Next(1000, int.MaxValue);

                // Only add a space after a prefix if a prefix is specified
                // and its last character is not a special no-space one.
                var postPrefixDivider = settings.Prefix switch
                {
                    { Length: > 0 } when !noSpaceChars.Contains(settings.Prefix[^1]) => " ",
                    _ => string.Empty
                };

                var fileName = settings.Prefix + postPrefixDivider + rndNumber;

                var path = settings.OutputDirectory + fileName + settings.Extension;

                using var fileStream = File.Create(path);
                var content = new UTF8Encoding(true).GetBytes(fileName);
                fileStream.Write(content, 0, content.Length);
            }

            WriteLine($"{settings.Count} files created.");
        }
    }

    public class Settings
    {
        public int Count { get; init; }
        public string Prefix { get; init; }
        public string Extension { get; init; }
        public string OutputDirectory { get; init; }

        private static readonly List<string> SupportedFlags = new() { "-p", "-e", "-o", "-s" };

        public Settings(string[] args)
        {
            var (count, argPairs) = ParseArgs(args);

            Count = count;

            Prefix = argPairs.ContainsKey("-p") ? argPairs["-p"] : string.Empty;

            Extension = "." + (argPairs.ContainsKey("-e") ? argPairs["-e"] : string.Empty);

            OutputDirectory = argPairs.ContainsKey("-o")
                ? "." + Path.DirectorySeparatorChar + argPairs["-o"] + Path.DirectorySeparatorChar
                : "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;
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
                        argDict[currentFlag] += thisArg;
                    else
                        argDict.Add(currentFlag, thisArg);
                }
            }

            // For testing only.
            WriteLine("Count: " + count);
            foreach (var arg in argDict)
                WriteLine($"{arg.Key}: {arg.Value}");

            return (count, argDict);
        }
    }
}