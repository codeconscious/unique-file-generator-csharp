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

    private IEnumerable<FileData> PrepareFileData()
    {
        // Only add a post-prefix space when the last character is not alphanumeric.
        var postPrefixDivider = Settings.Prefix switch
        {
            { Length: > 0 } when char.IsLetterOrDigit(Settings.Prefix[^1]) => " ",
            _ => string.Empty
        };

        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        for (var i = 0; i < Settings.FileCount; i++)
        {
            var randomChars = stringFactory.CreateSingleUniqueString(10);
            var fileName = Settings.Prefix + postPrefixDivider + randomChars;

            var contents = Settings.SizeInBytes.HasValue
                ? stringFactory.CreateSingleUniqueString(Settings.SizeInBytes.Value)
                : fileName;

            yield return new FileData(fileName, contents);
        }
    }

    public void SaveFiles()
    {
        Directory.CreateDirectory(Settings.OutputDirectory);

        foreach (var file in PrepareFileData())
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