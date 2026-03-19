namespace Azalea.Sounds.FFmpeg.Native;

internal struct AVRational(int num, int den)
{
	public int num = num;
	public int den = den;
}
