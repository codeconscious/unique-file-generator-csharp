# Unique File Generator
This command line tool allows you to easily create an arbitrary number of unique (by name and content) files on your computer. Each filename contains a random collection of characters to differentiate them. You can supply optional parameters to customize files according to your needs. Before running, the tool will ensure there is sufficient drive space available for the operation. This tool targets .NET 6.

## Usage
At the minimum, you must specify the number of files you want to generate. This should be an sequence of numbers (with optional commas).

```
uniquefilegen [filecount]
```

### Arguments
Each argument is optional. You must supply at least one value for each argument used. If you supply multiple values for an argument, they will be concatenated into one string divided by spaces.

Arg. | Description
---- | :----
-p | Add a filename prefix. If the prefix ends with a non-alphanumeric character, no space will be added after the prefix; otherwise, one will be automatically added.
-e | The file extension of the generated files. The opening period is optional. If not specified, no extension is added.
-s | The desired size of each file in bytes, which will be populated with random characters. If not specified, each file will only contain its own name.
-o | The output subfolder, which will be created if needed. If not supplied, "output" is used by default.
-d | A delay in milliseconds to be applied between each file's creation. Defaults to 0 if unspecified.

*Examples:*

```
uniquefilegen 50,000 -p Random-
```
Creates 50,000 files, each named similarly to "Random-########", in a subfolder named "output". There are no file extensions, nor is there a space after the prefix (due to the prefix's ending hyphen).

```
uniquefilegen 100 -p TEST-1229 -e txt -o My Output Folder -s 1000000 -d 1000
```
Creates one hundred 1MB files, each named similarly to "TEST-1229 ##########.txt", with a 1s break between each file's creation, and in a subfolder called "My Output Folder".
