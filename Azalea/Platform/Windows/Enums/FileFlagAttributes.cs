namespace Azalea.Platform.Windows.Enums;
internal enum FileFlagAttributes : uint
{
	Archive = 0x20,
	Encrypted = 0x4000,
	Hidden = 0x2,
	Normal = 0x80,
	Offline = 0x1000,
	Readonly = 0x1,
	System = 0x4,
	Temporary = 0x100,
	BackupSemantics = 0x02000000,
	DeleteOnClose = 0x04000000,
	NoBuffering = 0x20000000,
	OpenNoRecall = 0x00100000,
	OpenReleasePoint = 0x00200000,
	Overlapped = 0x40000000,
	PosixSemantics = 0x01000000,
	RandomAccess = 0x10000000,
	SessionAware = 0x00800000,
	SequentialScan = 0x08000000,
	WriteThrough = 0x80000000
}
