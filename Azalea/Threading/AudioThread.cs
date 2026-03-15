using System;

namespace Azalea.Threading;
internal class AudioThread : GameThread
{
	public AudioThread() : base(1000)
	{

	}

	protected override void Work()
	{
		Console.WriteLine("AUDIOED");
	}
}
