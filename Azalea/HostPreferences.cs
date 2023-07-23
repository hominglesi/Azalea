namespace Azalea;

public struct HostPreferences
{
    public HostType Type = HostType.Veldrid;

    public HostPreferences() { }
}

public enum HostType
{
    Silk,
    Veldrid,
    XNA,
}