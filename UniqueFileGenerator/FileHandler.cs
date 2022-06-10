using System.Text;
using System.IO;
using System.Threading;

namespace UniqueFileGenerator;

public sealed class FileHandler
{
    private Settings Settings { get; }

    public FileHandler(Settings settings)
    {
        Settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <summary>
    /// Create FileData instances using the settings.
    /// </summary>
    private IEnumerable<FileData> GetFileData()
    {
        var stringFactory = new RandomStringFactory(Settings.CharacterTypes);

        for (uint i = 0; i < Settings.FileCount; i++)
        {
            var randomText = stringFactory.CreateUniqueString(length: Settings.RandomStringLength);
            var fileName = Settings.Prefix + randomText;

            var contents = Settings.SizeInBytes.HasValue
                ? stringFactory.CreateUniqueString(length: Settings.SizeInBytes.Value)
                : fileName;

            yield return new FileData(fileName, contents);
        }
    }

    /// <summary>
    /// Create and write files to the drive.
    /// </summary>
    public void CreateFiles()
    {
        var hasSufficientSpace = EnsureSufficientDriveSpace();

        if (hasSufficientSpace == false)
            throw new InvalidOperationException(Resources.DriveSpaceInsufficient);

        Directory.CreateDirectory(Settings.OutputDirectory);

        // Create the files and show a progress bar.
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

                    if (Settings.FileCreationDelayMs > 0)
                        Thread.Sleep(Settings.FileCreationDelayMs);
                }
            });
    }

    /// <summary>
    /// Checks, if possible, that there is sufficient available space on the drive for the operation.
    /// </summary>
    /// <returns>
    ///     A nullable bool. True and false indicate a clear result.
    ///     Null indicates that the checking operation failed.
    /// </returns>
    private bool? EnsureSufficientDriveSpace()
    {
        // Don't allow the drive space to drop below this size in bytes.
        const long driveSpaceToKeepAvailable = 536_870_912; // 0.5GB

        try
        {
            var appPath = System.AppContext.BaseDirectory;

            var rootPath = Path.GetPathRoot(appPath);
            if (rootPath == null)
                return null;

            var driveInfo = new DriveInfo(rootPath);
            var availableFreeSpace = driveInfo.AvailableFreeSpace - driveSpaceToKeepAvailable;

            return Settings.DiskSpaceNecessary < availableFreeSpace;
        }
        catch (Exception)
        {
             return null;
        }
    }

    /// <summary>
    /// Represents the data necessary to save a file on the drive.
    /// </summary>
    private record class FileData
    {
        public string Name { get; }
        public string Content { get; }

        public FileData(string name, string content)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException(Resources.FileNameInvalid, nameof(name));

            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException(Resources.FileContentInvalid, nameof(content));

            Name = name.Trim();
            Content = content;
        }
    }
}
