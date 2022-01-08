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
        // Only add a post-prefix space for non-alphanumeric characters.
        var postPrefixDivider = Settings.Prefix switch
        {
            { Length: > 0 } when char.IsLetterOrDigit(Settings.Prefix[^1]) => " ",
            _ => string.Empty
        };

        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        //var idQueue = new Queue<string>(settings.Count);
        var baseFileNames = stringFactory.CreateRandomStrings(Settings.FileCount, 10);
        var prefixedFileNameQueue = new Queue<string>(
            baseFileNames.Select(n => Settings.Prefix + postPrefixDivider + n));

        var contentQueue = Settings.SizeInBytes.HasValue
            ? new Queue<string>(stringFactory.CreateRandomStrings(Settings.FileCount, Settings.SizeInBytes.Value))
            : new Queue<string>(prefixedFileNameQueue);

        Directory.CreateDirectory(Settings.OutputDirectory);

        while (prefixedFileNameQueue.Any())
        {
            var fileName = prefixedFileNameQueue.Dequeue();
            var path = Settings.OutputDirectory + fileName + Settings.Extension;
            var content = new UTF8Encoding(true).GetBytes(contentQueue.Dequeue());

            using var fileStream = File.Create(path);
            fileStream.Write(content, 0, content.Length);
        }
    }
}