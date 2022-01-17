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

        for (uint i = 0; i < Settings.FileCount; i++)
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
        var hasSufficientSpace = EnsureSufficientDriveSpace();

        // Abort if we've confirmed there is insufficient available drive space.
        if (hasSufficientSpace.HasValue && !hasSufficientSpace.Value)
            throw new InvalidOperationException(ResourceStrings.DriveSpaceInsufficient);

        Directory.CreateDirectory(Settings.OutputDirectory);

        AnsiConsole.Progress()
            .AutoClear(true)
            .Columns(new ProgressColumn[]
            {
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new SpinnerColumn(),
            })
            .Start(ctx =>
            {
                var task = ctx.AddTask("File Creation");
                var incrementBy = 100f / Settings.FileCount;

                foreach (var file in GetFileData())
                {
                    var path = Settings.OutputDirectory + file.Name + Settings.Extension;
                    var content = new UTF8Encoding(true).GetBytes(file.Content);

                    using var fileStream = File.Create(path);
                    fileStream.Write(content, 0, content.Length);

                    task.Increment(incrementBy);
                }
            });
    }

    /// <summary>
    /// Checks that there is sufficient available space on the drive for the operation.
    /// </summary>
    /// <returns>
    ///     A nullable bool. True and false indicate a clear result.
    ///     Null indicates that the checking operation failed.
    /// </returns>
    private bool? EnsureSufficientDriveSpace()
    {
        // Don't allow the drive space to drop before this amount of bytes.
        const long driveSpaceToKeepAvailable = 500_000_000; // Roughly 0.5GB

        try
        {
            var appPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            var rootPath = Path.GetPathRoot(appPath);
            if (rootPath == null)
                return null;

            var driveInfo = new DriveInfo(rootPath);
            var availableFreeSpace = driveInfo.AvailableFreeSpace - driveSpaceToKeepAvailable;

            return Settings.DiskSpaceNecessary < availableFreeSpace;
        }
        catch (System.Exception)
        {
             return null;
        }
    }

    private class FileData
    {
        public string Name { get; }
        public string Content { get; }

        public FileData(string name, string content)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(ResourceStrings.FileNameInvalid, nameof(name));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException(ResourceStrings.FileContentInvalid, nameof(content));

            Name = name;
            Content = content;
        }
    }
}