using Azalea.IO.Resources;
using System;
using System.IO;

namespace Azalea.IO.Configs;
internal class FileConfigProvider : ConfigProvider
{
	private string _fileName;

	public FileConfigProvider(string fileName = "config.cfg")
	{
		_fileName = fileName;

		load();
	}

	private void load()
	{
		Storage persistentStore;

		try { persistentStore = Assets.PersistentStore; }
		catch { throw new Exception("Cannot create config from file without setting up a PersistentStore."); }

		if (persistentStore.Exists(_fileName))
		{
			var data = persistentStore.GetText(_fileName)!;
			ConfigParser.Parse(data, ref Dictionary);
		}
	}

	internal override void Save()
	{
		var configString = ConfigParser.Format(Dictionary);
		var persistentStore = Assets.PersistentStore;

		persistentStore.Delete(_fileName);
		using var stream = Assets.PersistentStore.GetOrCreateStream(_fileName);
		using var writer = new StreamWriter(stream);
		writer.Write(configString);
	}
}
