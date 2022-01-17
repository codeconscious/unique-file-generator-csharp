namespace UniqueFileGenerator;

public static class ResourceStrings
    {
        public const string CountWarning = "You've requested the creation of many files. Do you want to continue?";
        public const string SizeWarning = "You've requested very large files. Do you want to continue?";
        public const string DelayWarning = "You've requested a very long delay between files. Do you want to continue?";
        public const string CancelledByUser = "Operation cancelled.";
        public const string CancelledDueToError = "Operation aborted: ";
        public static string Completed(uint count) =>
            $"{count} {(count == 1 ? "file" : "files" )} created.";

        public const string DriveSpaceInsufficient = "There is not enough free space on this disk to continue.";
        public const string DriveInfoReadError = "An unknown error occurred when reading the drive information.";

        public const string FileNameInvalid = "A valid filename was not specified.";
        public const string FileContentInvalid = "File content must be specified.";
        public const string PathRootParseError = "This drive's path root could not be determined.";

        public const string FileSizeInvalidZero = "File size cannot be 0.";
        public const string FileSizeInvalidRange = "The file size must be a positive number greater than 0.";
        public const string FileCountMissing = "The file count must be specified.";
        public const string FileCountInvalidRange = "You must enter a file count of at least 1 as the first argument.";
        public const string FileCountInvalidZero = "The file count cannot be zero.";
        public const string ValueWithNoFlag_Prefix = "A flag was not specified for ";
        public const string FlagCanBeUsedOnce = "A flag can only be specified once.";

        public const string CharBankEmpty = "The character bank is empty. It must contain at least 2 characters.";
        public const string CharBankTooShort = "The character bank must contain at least 2 characters.";
        public const string StringCountZero = "The count of desired strings must be greater than zero.";
        public const string StringLengthZero = "The length of each string must be greater than zero.";
        public const string LengthInvalidNegative = "The length cannot be negative.";
        public const string FileCreationDelayOutOfRange = "The file creation delay must be 0 or higher.";
    }