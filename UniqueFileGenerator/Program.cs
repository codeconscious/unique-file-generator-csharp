﻿global using Spectre.Console;

namespace UniqueFileGenerator;

public static class Program
{
    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintInstructions();
            return;
        }

        try
        {
            var settings = new Settings(args);

            if (!settings.ShouldProceedDespiteHighValues())
            {
                AnsiConsole.WriteLine(ResourceStrings.CancelledByUser);
                return;
            }

            var fileHandler = new FileHandler(settings);

            fileHandler.SaveFiles();

            AnsiConsole.MarkupLine($"{ResourceStrings.Completed(settings.FileCount)}");
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]{ResourceStrings.CancelledDueToError}{ex.Message}[/]");
            return;
        }
    }

    private static void PrintInstructions()
    {
        var outerTable = new Table();

        outerTable.AddColumn("Unique File Generator");
        outerTable.AddRow("Easily create an arbitrary number of unique (by name and content) files.  " +
            "Each filename contains a random collection of characters.  " +
            "You can supply optional parameters to customize files according to your needs.  " +
            "The tool ensures there is sufficient drive space available before beginning.");
        outerTable.AddEmptyRow();

        outerTable.AddRow("At the minimum, you must specify the number of files to generate.  " +
            "This should be a sequence of numbers (with optional commas).");
        outerTable.AddEmptyRow();

        var argTable = new Table();
        argTable.Border(TableBorder.None);
        argTable.AddColumn("Arg");
        argTable.AddColumn("Description");
        argTable.HideHeaders();
        argTable.Columns[0].PadRight(3);
        argTable.AddRow("-p", "Add a filename prefix. If the prefix ends with a non-alphanumeric character, no space will be added after the prefix; otherwise, one will be automatically added.");
        argTable.AddRow("-e", "The file extension of the generated files. The opening period is optional. If not specified, no extension is added.");
        argTable.AddRow("-s", "The desired size of each file in bytes, which will be populated with random characters. If not specified, each file will only contain its own name.");
        argTable.AddRow("-o", "The output subfolder, which will be created if needed. If not supplied, \"output\" is used by default.");
        argTable.AddRow("-d", "A delay in milliseconds to be applied between each file's creation. Defaults to 0 if unspecified.");

        outerTable.AddRow(argTable);
        outerTable.AddEmptyRow();

        outerTable.AddRow("Examples:\n" +
            "   uniquefilegen 10\n" +
            "        [gray]Creates 10 files with the default settings[/]\n" +
            "   uniquefilegen 1,000 -p TEST-1229 -e txt -o My Output Folder -s 1000000 -d 1000\n" +
            "        [gray]Creates one thousand 1MB files, each named like \"TEST-1229 ##########.txt\",\n" +
            "        in a subfolder called \"My Output Folder\", with a 1s delay after each new file.[/]");

        outerTable.AddEmptyRow();
        outerTable.AddRow("Homepage: https://github.com/codeconscious/unique-file-generator");

        AnsiConsole.Write(outerTable);
    }
}