﻿using static System.Console;
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
                WriteLine($"Create unique files with unique contents using random numbers. Saves to ./output directory.");
                WriteLine($"Arguments:");

                // var table = new Table();
                // table.AddColumn("Arg");
                // table.AddColumn("Description");
                // table.AddRow("-p", "File name prefix");
                // table.AddRow("-e", "The desired file extension. (The opening period is optional.)");
                // AnsiConsole.Write(table);

                WriteLine($"  - The number of files to make");
                WriteLine($"  - (Optional) File name prefix");
                WriteLine($"  - (Optional, but must be used with the second argument) The desired file extension (excluding the opening period)");
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

        private static List<string> SupportedFlags = new() { "-p", "-e", "-o", "-s" };

        public Settings(string[] args)
        {
            var (count, argPairs) = ParseArgs(args);

            Count = count > 0
                ? count
                : throw new ArgumentOutOfRangeException(nameof(count));

            Prefix = argPairs.ContainsKey("-p") ? argPairs["-p"] : string.Empty;

            Extension = "." + (argPairs.ContainsKey("-e") ? argPairs["-e"] : string.Empty);

            OutputDirectory = argPairs.ContainsKey("-o")
                ? argPairs["-o"]
                : "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;
        }

        private static (int Count, Dictionary<string, string> argDict) ParseArgs(string[] args)
        {
            if (args.Length == 0)
                throw new ArgumentException("The count must be specified", nameof(args));

            var argQueue = new Queue<string>(args);

            var expectedCountText = argQueue.Dequeue();
            if (!int.TryParse(expectedCountText, out var expectedCount))
                throw new InvalidOperationException("You must enter a count as the first argument.");
            if (expectedCount < 1)
                throw new ArgumentOutOfRangeException(nameof(expectedCount));

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
                else // Not a flag
                {
                    if (string.IsNullOrWhiteSpace(currentFlag))
                        throw new InvalidOperationException("Was a flag specified?");

                    if (argDict.ContainsKey(currentFlag))
                        argDict[currentFlag] = argDict[currentFlag] + thisArg; // use SB?
                    else
                        argDict.Add(currentFlag, thisArg);
                }
            }

            WriteLine("Count: " + expectedCount);
            foreach (var arg in argDict)
                WriteLine($"{arg.Key}: {arg.Value}");

            return (expectedCount, argDict);
        }
    }
}