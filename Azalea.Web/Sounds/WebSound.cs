using Azalea.Sounds;

namespace Azalea.Web.Sounds;
internal class WebSound : SoundByte
{
	public WebAudioBuffer Buffer { get; }

	public WebSound(ISoundData data)
	{
		Buffer = new WebAudioBuffer(data);
	}

	protected override void OnDispose() { }
}
