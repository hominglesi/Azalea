using Azalea.IO.Resources;
using System.Collections.Generic;

namespace Azalea.Graphics.Textures;

public class TextureAnimation : ITexture
{
	internal readonly List<(Texture, float)> Frames = [];
	private float _totalDuration;

	public TextureAnimation() { }

	public TextureAnimation(IEnumerable<Texture> frames, float duration)
		=> AddFrames(frames, duration);

	public Texture GetTextureAt(float time)
	{
		if (Frames.Count == 0)
			return Assets.MissingTexture;

		time %= _totalDuration;
		float counter = 0;

		foreach (var (frame, frameDuration) in Frames)
		{
			if (time < counter + frameDuration)
				return frame;

			counter += frameDuration;
		}

		return Assets.MissingTexture;
	}

	public void AddFrame(Texture texture, float time)
	{
		Frames.Add((texture, time));
		_totalDuration += time;
	}

	public void AddFrames(IEnumerable<Texture> textures, float time)
	{
		foreach (var texture in textures)
			AddFrame(texture, time);
	}
}
