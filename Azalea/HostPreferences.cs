namespace Azalea;

public struct HostPreferences
{
    public HostType Type = HostType.XNA;

    public HostPreferences() { }
}

public enum HostType
{
    XNA,
    Silk
}