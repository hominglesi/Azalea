using Azalea.Audio;
using Azalea.Graphics;
using Azalea.IO.Assets;
using System;

namespace Azalea.VisualTests;
internal class AudioTest : TestScene
{
	private Sound _goodkid;
	private Sound _hitnormal;

	private AudioInstance _instance;

	public AudioTest()
	{
		_goodkid = Assets.GetSound(@"D:\Programming\Azalea\Azalea.VisualTests\Resources\Audio\goodkid.wav");
		_hitnormal = Assets.GetSound(@"D:\Programming\Azalea\Azalea.VisualTests\Resources\Audio\hitnormal.wav");

		AddRange(new GameObject[] {
			CreateFullscreenVerticalFlex(new GameObject[]
			{
				CreateActionButton(
					"Play 'goodkid.wav' at 0.1f",
					() => _instance = AudioManager.Play(_goodkid, 0.1f)),
				CreateActionButton(
					"Play 'hitnormal.wav' at 0.1f",
					() => _instance = AudioManager.Play(_hitnormal, 0.1f)),
				CreateActionButton(
					"Set _instance gain to 0.1f",
					() => { if (_instance is not null) _instance.Gain = 0.1f; }),
				CreateActionButton(
					"Set _instance gain to 0.6f",
					() => { if (_instance is not null) _instance.Gain = 0.6f; }),
				CreateActionButton(
					"Stop _instance",
					() => { _instance?.Stop(); }),
				CreateActionButton(
					"Garbage collect",
					() => GC.Collect())
			}),
			CreateObservedContainer(new GameObject[]
			{
				CreateObservedValue("_instance Playing",
					() => _instance is not null ? _instance.Playing : false),
				CreateObservedValue("_instance Gain",
					() => _instance is not null && _instance.Playing ? _instance.Gain : -1)
			})
		});
	}
}
