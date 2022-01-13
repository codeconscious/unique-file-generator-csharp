namespace UniqueFileGenerator;

public static class ResourceStrings
    {
        public const string CountWarning = "You've requested the creation of many files. Do you want to continue?";
        public const string SizeWarning = "You've requested very large files. Do you want to continue?";
        public const string CancelledByUser = "Operation cancelled.";
        public const string CancelledDueToError = "[red]The operation was aborted because an error occurred.[/]: ";
        public static string Completed(uint count) =>
            $"[green]{count} {(count == 1 ? "file" : "files" )} created.[/]";

        public const string InsufficientDriveSpace = "There is not enough free space on this disk to continue.";
        public const string ErrorReadingDriveInfo = "An unknown error occurred when reading the drive information.";

        public const string InvalidFileName = "A valid filename was not specified.";
        public const string InvalidFileContent = "File content must be specified.";
        public const string CouldNotReadPathRoot = "This drive's path root could not be determined.";

        public const string InvalidFileSizeZero = "File size cannot be 0.";
        public const string InvalidFileSizeOutOfRange = "The file size must be a positive number greater than 0.";
        public const string FileCountMissing = "The file count must be specified.";
        public const string InvalidFileCount = "You must enter a file count of at least 1 as the first argument.";
        public const string InvalidFileCountZero = "The file count cannot be zero.";
        public const string ValueWithNoFlag = "A flag was not specified for ";
        public const string CanOnlyUseFlagOnce = "A flag can only be specified once.";

        public const string CharBankEmpty = "The character bank is empty. It must contain at least 2 characters.";
        public const string CharBankTooShort = "The character bank must contain at least 2 characters.";
        public const string StringCountZero = "The count of desired strings must be greater than zero.";
        public const string StringLengthZero = "The length of each string must be greater than zero.";
        public const string LengthInvalidNegative = "The length cannot be negative.";
    }