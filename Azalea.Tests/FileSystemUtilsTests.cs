using Azalea.Utils;

namespace Azalea.Tests;
public class FileSystemUtilsTests
{
	[TestCase("Resources/", "Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Resources", "/Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Resources", "Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Files/Resources", "Folder/file.png", ExpectedResult = "Files/Resources/Folder/file.png")]
	[TestCase("/Resources", "Folder/file.png", ExpectedResult = "/Resources/Folder/file.png")]
	[TestCase("Resources\\", "Folder\\file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Resources/FakeFile", "../Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Resources/FakeFile/FakeFile2/", "../../Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("Resources/FakeFile/FakeFile2", "../../Folder/file.png", ExpectedResult = "Resources/Folder/file.png")]
	[TestCase("", "Folder/file.png", ExpectedResult = "Folder/file.png")]
	public string CombinePathTest(string path1, string path2)
	{
		return FileSystemUtils.CombinePaths(path1, path2);
	}
}
