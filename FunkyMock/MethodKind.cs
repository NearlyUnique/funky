namespace FunkyMock;

[Flags]
public enum MethodKind
{
    Unknown = 0,
    Ordinary = 1,
    ReadProperty = 2,
    WriteProperty = 4,
    ReadWrite = ReadProperty | WriteProperty,
}
