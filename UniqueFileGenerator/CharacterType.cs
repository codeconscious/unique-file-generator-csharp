namespace UniqueFileGenerator;

[Flags]
public enum CharacterType
{
    UpperCaseLetter = 0,
    LowerCaseLetter = 1 << 0,
    Numeric =         1 << 1
}
