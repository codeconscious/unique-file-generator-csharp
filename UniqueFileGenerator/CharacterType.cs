namespace UniqueFileGenerator;

[Flags]
public enum CharacterType
{
    UpperCaseLetter = 0,
    LowerCaseLetter = 1 << 0,
    Number =          1 << 1
}
