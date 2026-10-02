using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Azalea.Graphics.Textures;
public class TextureAnimation : ITexture
{
	private readonly List<(ITexture, float)> _frames = [];
	private float _totalDuration;

	public Platform.Rendering.NativeTexture? NewTexture => null;

	public int Width => throw new NotImplementedException();

	public int Height => throw new NotImplementedException();

	public Vector2 Size => throw new NotImplementedException();

	public TextureAnimation() { }

	public TextureAnimation(IEnumerable<ITexture> frames, float duration)
		=> AddFrames(frames, duration);

	/*
	public INativeTexture GetNativeTexture(float time)
	{
		if (_frames.Count == 0)
			return null;

		time %= _totalDuration;
		float counter = 0;

		foreach (var (frame, frameDuration) in _frames)
		{
			if (time < counter + frameDuration)
				return null; //frame.GetNativeTexture(time - counter);

			counter += frameDuration;
		}

		return null;
	}*/

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

	public void Bind(RenderCoordinator coordinator, float time)
	{
		if (_frames.Count == 0)
		{
			Assets.MissingTexture.Bind(coordinator, time);
			return;
		}

		time %= _totalDuration;
		float counter = 0;

		foreach (var (frame, frameDuration) in _frames)
		{
			if (time < counter + frameDuration)
			{
				frame.Bind(coordinator, time);
				return;
			}

			counter += frameDuration;
		}

		Assets.MissingTexture.Bind(coordinator, time);
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
