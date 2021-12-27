using static System.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace UniqueFileGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                WriteLine($"Create unique files with unique contents.");
                WriteLine($"Arguments:");
                WriteLine($"  - The number of files to make.");
                WriteLine($"  - (Optional) File name prefix");
                WriteLine($"  - (Optional) The desired file extension (excluding the opening period)");
                return;
            }

            if (!int.TryParse(args[0], out var count) || count < 1)
            {
                WriteLine($"Invalid count {count} specified.");
                return;
            }

            var prefix = args.Length > 1 && !string.IsNullOrEmpty(args[1])
                ? args[1]
                : string.Empty;

            var extension = args.Length > 2 && !string.IsNullOrEmpty(args[2])
                ? "." + args[2]
                : string.Empty;

            // TODO: Think about if and how to support passing a path.
            // var directory = args.Length > 3 && !string.IsNullOrEmpty(args[3])
            //     ? args[2] + Path.DirectorySeparatorChar
            //     : "." + Path.DirectorySeparatorChar;
            var directory = "." + Path.DirectorySeparatorChar + "output" + Path.DirectorySeparatorChar;

            var random = new Random();

            // TODO: Support checking for used ints.
            //var usedValues = new List<int>();

            // IDEA: Perhaps generate the numbers in a hashset or queue first, then dequeue them.

            // Should this be parallel?
            for (var i = 0; i < count; i++)
            {
                var rndNumber = random.Next(1000, int.MaxValue);

                var fileName = prefix + " " + rndNumber;

                var path = directory + fileName + extension;

                using var fs = File.Create(path);
                var content = new UTF8Encoding(true).GetBytes(fileName);
                fs.Write(content, 0, content.Length);
            }

            WriteLine($"{count} files created.");
        }
    }
}