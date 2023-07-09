namespace Azalea;

public struct HostPreferences
{
    public HostType Type = HostType.Silk;

    public HostPreferences() { }
}

public enum HostType
{
    XNA,
    Silk
}