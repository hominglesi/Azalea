namespace Azalea.Sounds.OpenAL;
internal class ALSound : SoundByte
{
	internal ALBuffer Buffer;

	public ALSound(byte[] data, ALFormat format, int frequency)
	{
		Buffer = new ALBuffer();
		Buffer.SetData(data, format, frequency);
	}

	protected override void OnDispose()
		=> Buffer.Dispose();
}
