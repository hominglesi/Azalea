using Azalea.IO.Resources;
using Azalea.Numerics;
using System;
using System.Collections.Generic;

namespace Azalea.Graphics.Textures;
public class TextureAnimation : ITexture
{
	private readonly List<(ITexture, float)> _frames = [];
	private float _totalDuration;

	public TextureAnimation() { }

	public TextureAnimation(IEnumerable<ITexture> frames, float duration)
		=> AddFrames(frames, duration);

	public INativeTexture GetNativeTexture(float time)
	{
		if (_frames.Count == 0)
			return Assets.MissingTexture.GetNativeTexture();

		time %= _totalDuration;
		float counter = 0;

		foreach (var (frame, frameDuration) in _frames)
		{
			if (time < counter + frameDuration)
				return frame.GetNativeTexture(time - counter);

			counter += frameDuration;
		}

		return Assets.MissingTexture.GetNativeTexture();
	}

	public Rectangle GetUVCoordinates(float time)
	{
		if (_frames.Count == 0)
			return Assets.MissingTexture.GetUVCoordinates(time);

		time %= _totalDuration;
		float counter = 0;

		foreach (var (frame, frameDuration) in _frames)
		{
			if (time < counter + frameDuration)
				return frame.GetUVCoordinates(time - counter);

			counter += frameDuration;
		}

		return Assets.MissingTexture.GetUVCoordinates(time);
	}

	public void UploadImage(Image image)
	{
		throw new Exception("Cannot upload Image to TextureAnimation! " +
			"Upload images to the individual Textures instead.");
	}

	public void SetFiltering(TextureFiltering minFilter, TextureFiltering magFilter)
	{
		throw new Exception("Cannot set filtering of TextureAnimation! " +
			"Set the filtering of the individual Textures instead.");
	}

	public void AddFrame(ITexture texture, float time)
	{
		_frames.Add((texture, time));
		_totalDuration += time;
	}

	public void AddFrames(IEnumerable<ITexture> textures, float time)
	{
		foreach (var texture in textures)
			AddFrame(texture, time);
	}
}
