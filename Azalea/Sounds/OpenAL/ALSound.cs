namespace Azalea.Sounds.OpenAL;
internal class ALSound : SoundByte
{
	internal ALBuffer Buffer;

	public ALSound(ISoundData data)
	{
		Buffer = new ALBuffer();
		Buffer.SetData(data);
	}

	protected override void OnDispose()
		=> Buffer.Dispose();
}
