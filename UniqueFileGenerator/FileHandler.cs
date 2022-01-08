using System.Text;

namespace UniqueFileGenerator;

public class FileHandler
{
    private Settings Settings { get; }

    public FileHandler(Settings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);

        Settings = settings;
    }

    // TODO: Refactor to return one file at a time.
    private IEnumerable<FileData> PrepareFileData()
    {
        // Only add a post-prefix space when the last character is not alphanumeric.
        var postPrefixDivider = Settings.Prefix switch
        {
            { Length: > 0 } when char.IsLetterOrDigit(Settings.Prefix[^1]) => " ",
            _ => string.Empty
        };

        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        var baseFileNames = stringFactory.CreateUniqueRandomStrings(Settings.FileCount, 10);
        var fileNames = baseFileNames.Select(n => Settings.Prefix + postPrefixDivider + n);

        var contents = Settings.SizeInBytes.HasValue
            ? stringFactory.CreateUniqueRandomStrings(Settings.FileCount,
                                                      Settings.SizeInBytes.Value)
            : fileNames;

        return fileNames.Zip(contents, (k, v) => new FileData(k, v));
    }

    public void SaveFiles()
    {
        var fileData = PrepareFileData();

        Directory.CreateDirectory(Settings.OutputDirectory);

        foreach (var file in fileData)
        {
            var path = Settings.OutputDirectory + file.Name + Settings.Extension;
            var content = new UTF8Encoding(true).GetBytes(file.Content);

            using var fileStream = File.Create(path);
            fileStream.Write(content, 0, content.Length);
        }
    }

    private class FileData
    {
        public string Name { get; }
        public string Content { get; }

        public FileData(string name, string content)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("A proper filename must be specified.", nameof(name));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("File content must be specified.", nameof(content));

            Name = name;
            Content = content;
        }
    }
}