using Azalea.IO.ObservedDirectories;
using Azalea.IO.Resources;
using Azalea.Platform;
using System.Collections.Generic;
using System.IO;

namespace Azalea.VisualTests.UnitTesting.UnitTests.IO;
public class ObservedDirectoryTests : UnitTestSuite
{
	protected static ObservedDirectory? ObservedDirectory;

	protected static List<string>? CreatedEvents;
	protected static List<(string, string)>? LoadedEvents;
	protected static List<string>? ModifiedEvents;
	protected static List<string>? DeletedEvents;

	public class ReadExistingFilesTest : UnitTest
	{
		public ReadExistingFilesTest()
		{
			AddOperation("Create 3 files", () =>
			{
				createTestFile("file1.txt");
				createTestFile("file2.txt");
				createTestFile("file3.txt");
			});

			AddOperation("Create ObservedDirectory", createObservedDirectory);

			AddResult("Were 3 created events fired", () => checkEventCount(3, 0, 0, 0));
		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class ReadNewFilesTest : UnitTest
	{
		public ReadNewFilesTest()
		{
			AddOperation("Create ObservedDirectory", createObservedDirectory);

			AddOperation("Create 3 files", () =>
			{
				createTestFile("file1.txt");
				createTestFile("file2.txt");
				createTestFile("file3.txt");
			});

			AddResult("Were 3 created events fired", () => checkEventCount(3, 0, 0, 0));
		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class DeleteExistingFilesTest : UnitTest
	{
		public DeleteExistingFilesTest()
		{
			AddOperation("Create ObservedDirectory", createObservedDirectory);

			AddOperation("Create 3 files", () =>
			{
				createTestFile("file1.txt");
				createTestFile("file2.txt");
				createTestFile("file3.txt");
			});

			AddResult("Were 3 created events fired", () => checkEventCount(3, 0, 0, 0));

			AddOperation("Delete 3 files", () =>
			{
				deleteTestFile("file1.txt");
				deleteTestFile("file2.txt");
				deleteTestFile("file3.txt");
			});

			AddResult("Were 3 deleted events fired", () => checkEventCount(0, 0, 0, 3));
		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class LoadCachedFilesTest : UnitTest
	{
		public LoadCachedFilesTest()
		{
			AddOperation("Prepare cache", prepareCache);
			string path = Assets.PersistentStore.Path + "Temp/Dir";
			AddOperation("Create ObservedDirectory", createObservedDirectory);

			AddResult("Was the correct data loaded", () => LoadedEvents!.Count == 3 &&
				LoadedEvents![0].Item2 == "file1.txt" &&
				LoadedEvents![1].Item2 == "file2.txt" &&
				LoadedEvents![2].Item2 == "file3.txt");

			AddResult("Were 3 loaded events fired", () => checkEventCount(0, 3, 0, 0));
		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class MultiplePathsTest : UnitTest
	{
		public MultiplePathsTest()
		{
			var directory1 = Assets.PersistentStore.Path + "Temp/Dir1/";
			AddOperation("Create 3 files in first directory", () =>
			{
				createTestFileInDirectory(directory1, "file1.txt");
				createTestFileInDirectory(directory1, "file2.txt");
				createTestFileInDirectory(directory1, "file3.txt");
			});
			var directory2 = Assets.PersistentStore.Path + "Temp/Dir2/";
			AddOperation("Create 3 files in second directory", () =>
			{
				createTestFileInDirectory(directory2, "file1.txt");
				createTestFileInDirectory(directory2, "file2.txt");
				createTestFileInDirectory(directory2, "file3.txt");
			});

			AddOperation("Create ObservedDirectory", () => createObservedDirectory([directory1, directory2]));

			AddResult("Were 6 created events fired", () => checkEventCount(6, 0, 0, 0));
		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class PathAddedTest : UnitTest
	{
		public PathAddedTest()
		{
			var directory1 = Assets.PersistentStore.Path + "Temp/Dir1/";
			AddOperation("Create 3 files in first directory", () =>
			{
				createTestFileInDirectory(directory1, "file1.txt");
				createTestFileInDirectory(directory1, "file2.txt");
				createTestFileInDirectory(directory1, "file3.txt");
			});

			AddOperation("Create ObservedDirectory", () => createObservedDirectory([directory1]));

			AddResult("Were 3 created events fired", () => checkEventCount(3, 0, 0, 0));

			AddResult("Were all events reset", () => checkEventCount(0, 0, 0, 0));


			var directory2 = Assets.PersistentStore.Path + "Temp/Dir2/";
			AddOperation("Create 3 files in a new directory", () =>
			{
				createTestFileInDirectory(directory2, "file1.txt");
				createTestFileInDirectory(directory2, "file2.txt");
				createTestFileInDirectory(directory2, "file3.txt");
			});


			AddOperation("Add a new path to observed directory", () => ObservedDirectory!.AddPath(directory2));

			AddResult("Were 3 new created events fired", () => checkEventCount(3, 0, 0, 0));

			AddOperation("Saving data to cache", () => saveCache());

		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}

	public class PathRemovedTest : UnitTest
	{
		public PathRemovedTest()
		{
			var directory1 = Assets.PersistentStore.Path + "Temp/Dir1/";
			AddOperation("Create 3 files in first directory", () =>
			{
				createTestFileInDirectory(directory1, "file1.txt");
				createTestFileInDirectory(directory1, "file2.txt");
				createTestFileInDirectory(directory1, "file3.txt");
			});


			var directory2 = Assets.PersistentStore.Path + "Temp/Dir2/";
			AddOperation("Create 3 files in a new directory", () =>
			{
				createTestFileInDirectory(directory2, "file1.txt");
				createTestFileInDirectory(directory2, "file2.txt");
				createTestFileInDirectory(directory2, "file3.txt");
			});

			AddOperation("Create ObservedDirectory", () => createObservedDirectory([directory1, directory2]));

			AddResult("Were 6 created events fired", () => checkEventCount(6, 0, 0, 0));


			AddOperation("Remove directory from observed directory", () => ObservedDirectory!.RemovePath(directory2));

			AddResult("Were 3 deleted events fired", () => checkEventCount(0, 0, 0, 3));

			AddOperation("Saving data to cache", () => saveCache());

		}

		public override void TearDown(UnitTestContainer scene) => tearDown();
	}


	private static void createTestFile(string name)
	{
		var directory = Assets.PersistentStore.Path + "Temp/Dir/";

		if (Directory.Exists(directory) == false)
			Directory.CreateDirectory(directory);

		var file = File.Create(directory + name);
		file.Close();
	}

	private static void createTestFileInDirectory(string path, string name)
	{
		var directory = path;

		if (Directory.Exists(directory) == false)
			Directory.CreateDirectory(directory);

		var file = File.Create(directory + name);
		file.Close();
	}

	private static void createMultipleTestFiles(string[] paths, string name)
	{
		foreach (var path in paths)
		{

			if (Directory.Exists(path) == false)
				Directory.CreateDirectory(path);

			var file = File.Create(path + name);
			file.Close();

		}
	}


	private static void deleteTestFile(string name)
	{
		File.Delete(Assets.PersistentStore.Path + "Temp/Dir/" + name);
	}

	private static void createObservedDirectory()
	{
		CreatedEvents = [];
		LoadedEvents = [];
		ModifiedEvents = [];
		DeletedEvents = [];

		var path = Assets.PersistentStore.Path + "Temp/Dir";


		Directory.CreateDirectory(path);

		ObservedDirectory = new ObservedDirectory([path], "Temp/cache.txt")
		{
			SerializeFileMethod = path => path[(path.LastIndexOf('\\') + 1)..]
		};

		ObservedDirectory.OnCreated += path => CreatedEvents.Add(path);
		ObservedDirectory.OnLoaded += (path, data) => LoadedEvents.Add((path, data));
		ObservedDirectory.OnModified += path => ModifiedEvents.Add(path);
		ObservedDirectory.OnDeleted += path => DeletedEvents.Add(path);

		ObservedDirectory.Start();
	}

	private static void createObservedDirectory(string[] paths)
	{
		CreatedEvents = [];
		LoadedEvents = [];
		ModifiedEvents = [];
		DeletedEvents = [];

		foreach (var path in paths)
			Directory.CreateDirectory(path);

		ObservedDirectory = new ObservedDirectory(paths, "Temp/cache.txt")
		{
			SerializeFileMethod = path => path[(path.LastIndexOf('\\') + 1)..]
		};

		ObservedDirectory.OnCreated += path => CreatedEvents.Add(path);
		ObservedDirectory.OnLoaded += (path, data) => LoadedEvents.Add((path, data));
		ObservedDirectory.OnModified += path => ModifiedEvents.Add(path);
		ObservedDirectory.OnDeleted += path => DeletedEvents.Add(path);

		ObservedDirectory.Start();
	}

	private static void tearDown()
	{
		ObservedDirectory?.Dispose();
		Assets.PersistentStore.Delete("Temp");
	}

	private static bool checkEventCount(int created, int loaded, int modified, int deleted)
	{
		var bruh = Time.GetCurrentPreciseTime().AddSeconds(0.1f);
		while (Time.GetCurrentPreciseTime() < bruh) { }
		var correct = CreatedEvents!.Count == created && LoadedEvents!.Count == loaded
		&& ModifiedEvents!.Count == modified && DeletedEvents!.Count == deleted;

		CreatedEvents!.Clear();
		LoadedEvents!.Clear();
		ModifiedEvents!.Clear();
		DeletedEvents!.Clear();

		return correct;
	}

	private static void prepareCache()
	{
		createTestFile("file1.txt");
		createTestFile("file2.txt");
		createTestFile("file3.txt");

		string path = Assets.PersistentStore.Path + "Temp/Dir";
		createObservedDirectory([path]);

		ObservedDirectory!.SaveCache();
	}

	private static void saveCache() { ObservedDirectory!.SaveCache(); }
}
