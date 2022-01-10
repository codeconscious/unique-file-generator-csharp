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

    private IEnumerable<FileData> GetFileData()
    {
        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        for (nuint i = 0; i < Settings.FileCount; i++)
        {
            var randomChars = stringFactory.CreateSingleUniqueString(10);
            var fileName = Settings.Prefix + randomChars;

            var contents = Settings.SizeInBytes.HasValue
                ? stringFactory.CreateSingleUniqueString(Settings.SizeInBytes.Value)
                : fileName;

            yield return new FileData(fileName, contents);
        }
    }

    public void SaveFiles()
    {
        EnsureSufficientDiskSpace();

        Directory.CreateDirectory(Settings.OutputDirectory);

        foreach (var file in GetFileData())
        {
            var path = Settings.OutputDirectory + file.Name + Settings.Extension;
            var content = new UTF8Encoding(true).GetBytes(file.Content);

            using var fileStream = File.Create(path);
            fileStream.Write(content, 0, content.Length);
        }
    }

    private void EnsureSufficientDiskSpace()
    {
        var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        var rootPath = Path.GetPathRoot(appPath);
        var driveInfo = new DriveInfo(rootPath);
        var freeSpace = driveInfo.AvailableFreeSpace;

        if (freeSpace < Settings.DiskSpaceNecessary)
            throw new InvalidOperationException(ResourceStrings.InsufficientDriveSpace);
    }

    private class FileData
    {
        public string Name { get; }
        public string Content { get; }

        public FileData(string name, string content)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(ResourceStrings.InvalidFileName, nameof(name));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException(ResourceStrings.InvalidFileContent, nameof(content));

            Name = name;
            Content = content;
        }
    }
}