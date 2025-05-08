namespace Azalea.Platform.Windows.Enums;
internal enum FileAccess : uint
{
	None = 0u,
	GenericAll = 0x10000000u,
	GenericExecute = 0x20000000u,
	GenericRead = 0x80000000u,
	GenericWrite = 0x40000000u,
	FileReadData = 1u,
	FileWriteData = 2u,
	FileAppendData = 4u,
	//...//
}
