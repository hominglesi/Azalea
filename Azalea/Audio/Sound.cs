using Azalea.Audio.OpenAL;
using Azalea.Utils;

namespace Azalea.Audio;
public class Sound : Disposable
{
	internal ALBuffer Buffer;

	internal Sound(ISoundData data)
	{
		Buffer = new ALBuffer();
		Buffer.SetData(data);
	}

	protected override void OnDispose()
	{
		Buffer.Dispose();
	}
}
