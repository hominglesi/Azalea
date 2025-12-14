using Azalea.Utils;

namespace Azalea.Tests;
public class PathUtilsTests
{
	[TestCase("night-sky.png", ExpectedResult = "")]
	[TestCase("Textures/night-sky.png", ExpectedResult = "Textures/")]
	[TestCase("Textures/Backgrounds/night-sky.png",
		ExpectedResult = "Textures/Backgrounds/")]
	public string TestGetDirectoryFromPath(string path)
	{
		return PathUtils.GetDirectoryFromPath(path);
	}

	[TestCase("Textures/", ExpectedResult = "")]
	[TestCase("night-sky.png", ExpectedResult = "night-sky.png")]
	[TestCase("Textures/night-sky.png", ExpectedResult = "night-sky.png")]
	[TestCase("Textures/Backgrounds/night-sky.png",
		ExpectedResult = "night-sky.png")]
	public string TestGetFileFromPath(string path)
	{
		return PathUtils.GetFileFromPath(path);
	}

	[TestCase("night-sky.png", "DummyAssembly",
		ExpectedResult = "DummyAssembly.night-sky.png")]
	[TestCase("Textures/Backgrounds/night-sky.png", "DummyAssembly",
		ExpectedResult = "DummyAssembly.Textures.Backgrounds.night-sky.png")]
	public string TestGetResourcePathFromAssemblyPath(string path, string prefix)
	{
		return PathUtils.GetResourcePathFromAssemblyPath(path, prefix);
	}

	[TestCase("DummyAssembly.night-sky.png", "DummyAssembly",
		ExpectedResult = "night-sky.png")]
	[TestCase("DummyAssembly.Textures.Backgrounds.night-sky.png", "DummyAssembly",
		ExpectedResult = "Textures/Backgrounds/night-sky.png")]
	public string TestGetDirectoryPathFromResourcePath(string path, string prefix)
	{
		return PathUtils.GetDirectoryPathFromResourcePath(path, prefix);
	}

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
		return PathUtils.CombinePaths(path1, path2);
	}
}
