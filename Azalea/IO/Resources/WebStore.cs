using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Azalea.IO.Resources;

internal class WebStore : IResourceStore
{
	private static readonly HttpClient _client;

	static WebStore()
	{
		_client = new HttpClient(new SocketsHttpHandler()
		{
			PooledConnectionLifetime = TimeSpan.FromMinutes(5),
			PooledConnectionIdleTimeout = TimeSpan.FromMinutes(2),
			ConnectTimeout = TimeSpan.FromSeconds(15),
			EnableMultipleHttp2Connections = true,
			AllowAutoRedirect = true,
			MaxConnectionsPerServer = 10,
			UseCookies = false,
			AutomaticDecompression = DecompressionMethods.All
		})
		{
			Timeout = TimeSpan.FromSeconds(30),

			DefaultRequestVersion = HttpVersion.Version20,
			DefaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower
		};

		_client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
		_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
	}

	public Stream? GetStream(string path)
	{
		// This would return a stream without saving all the data in memory but
		// we wouldn't really get any advantage of doing that since audio streaming
		// would still break because we assume all streams are seekable
		// return _client.GetStreamAsync(path).GetAwaiter().GetResult();

		for (int i = 0; i < 3; i++)
		{
			byte[]? data;

			try { data = _client.GetByteArrayAsync(path).GetAwaiter().GetResult(); }
			catch (HttpRequestException e)
			{
				if (e.StatusCode == HttpStatusCode.NotFound)
					return null;

				Thread.Sleep(100 * i);
				continue;
			}
			catch (Exception) { return null; }

			if (data is null)
				return null;

			return new MemoryStream(data);
		}

		return null;
	}

	public IEnumerable<(string, bool)> GetAvalibleResources(string subPath = "")
	{
		throw new Exception("Cannot list all web resources");
	}
}
