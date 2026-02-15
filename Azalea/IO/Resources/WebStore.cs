using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace Azalea.IO.Resources;
internal class WebStore : IResourceStore
{
	private static readonly HttpClient _client;

	static WebStore()
	{
		_client = new HttpClient(new SocketsHttpHandler()
		{
			MaxConnectionsPerServer = 10,
			PooledConnectionLifetime = TimeSpan.FromMinutes(5),
			AutomaticDecompression =
				DecompressionMethods.GZip |
				DecompressionMethods.Deflate |
				DecompressionMethods.Brotli
		});
		_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
	}

	public Stream? GetStream(string path)
	{
		// This would return a stream without saving all the data in memory but
		// we wouldn't really get any advantage of doing that since audio streaming
		// would still break because we assume all streams are seekable
		// return _client.GetStreamAsync(path).GetAwaiter().GetResult();

		byte[]? data = null;

		try { data = _client.GetByteArrayAsync(path).GetAwaiter().GetResult(); }
		catch (Exception) { return null; }

		return new MemoryStream(data);
	}

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		throw new Exception("Cannot list all web resources");
	}
}
