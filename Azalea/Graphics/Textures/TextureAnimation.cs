using Azalea.Graphics.Rendering;
using Azalea.IO.Resources;
using System.Collections.Generic;

namespace Azalea.Graphics.Textures;
public class TextureAnimation : Texture
{
	private Texture? _firstTexture;
	internal override INativeTexture NativeTexture
		=> _firstTexture is null ? Assets.MissingTexture.NativeTexture : _firstTexture.NativeTexture;
	public override int Width
		=> _firstTexture is null ? Assets.MissingTexture.Width : _firstTexture.Width;

	public override int Height
		=> _firstTexture is null ? Assets.MissingTexture.Height : _firstTexture.Height;

	public TextureAnimation()
		: base(Assets.MissingTexture.NativeTexture)
	{

	}

	private List<KeyValuePair<Texture, float>> _frames = new();
	private float _duration;

	public Texture GetTextureAtTime(float time)
	{
		if (_frames.Count == 0) return Assets.MissingTexture;

		time %= _duration;
		float counter = 0;

		foreach (var pair in _frames)
		{
			if (time < pair.Value + counter)
			{
				return pair.Key;
			}

			counter += pair.Value;
		}

		return Assets.MissingTexture;
	}

	public void AddFrame(Texture texture, float time)
	{
		_frames.Add(new(texture, time));
		_duration += time;

		_firstTexture ??= texture;
	}
}
