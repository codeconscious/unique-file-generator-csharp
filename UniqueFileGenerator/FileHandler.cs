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

    public void SaveFiles()
    {
        // Only add a post-prefix space when the last character is not alphanumeric.
        var postPrefixDivider = Settings.Prefix switch
        {
            { Length: > 0 } when char.IsLetterOrDigit(Settings.Prefix[^1]) => " ",
            _ => string.Empty
        };

        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        var baseFileNames = stringFactory.CreateRandomStrings(Settings.FileCount, 10);
        var prefixedFileNames = new Queue<string>(
            baseFileNames.Select(n => Settings.Prefix + postPrefixDivider + n));

        var contentQueue = Settings.SizeInBytes.HasValue
            ? new Queue<string>(stringFactory.CreateRandomStrings(Settings.FileCount, Settings.SizeInBytes.Value))
            : new Queue<string>(prefixedFileNames);

        Directory.CreateDirectory(Settings.OutputDirectory);

        // Save each file.
        while (prefixedFileNames.Any())
        {
            var fileName = prefixedFileNames.Dequeue();
            var path = Settings.OutputDirectory + fileName + Settings.Extension;
            var content = new UTF8Encoding(true).GetBytes(contentQueue.Dequeue());

            using var fileStream = File.Create(path);
            fileStream.Write(content, 0, content.Length);
        }
    }
}