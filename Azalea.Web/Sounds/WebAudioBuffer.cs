using Azalea.Sounds;
using Azalea.Utils;
using System;

namespace Azalea.Web.Sounds;
internal class WebAudioBuffer : Disposable
{
	public object Handle { get; set; }

	public WebAudioBuffer(ISoundData data)
	{
		var bufferSize = data.Size / 2 / data.ChannelCount;
		Handle = WebAudio.CreateBuffer(data.ChannelCount, bufferSize, data.Frequency);
		WebAudio.BufferData(Handle, data.Data);
	}

	protected override void OnDispose()
	{
		throw new NotImplementedException();
	}
}
