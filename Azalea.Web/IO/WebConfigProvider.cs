using Azalea.IO.Configs;

namespace Azalea.Web.IO;
internal class WebConfigProvider : ConfigProvider
{
	public WebConfigProvider()
	{
		var length = WebLocalStorage.GetLength();

		for (int i = 0; i < length; i++)
		{
			var key = WebLocalStorage.Key(i);
			var value = WebLocalStorage.GetItem(key);

			Dictionary.Add(key, value);
		}
	}

	public override void Save()
	{
		WebLocalStorage.Clear();

		foreach (var keyValuePair in Dictionary)
		{
			WebLocalStorage.SetItem(keyValuePair.Key, keyValuePair.Value);
		}
	}
}
