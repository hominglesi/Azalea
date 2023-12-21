using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Graphics.Textures;
using System.Collections.Generic;
using System.Diagnostics;

namespace Azalea.Graphics.Sprites;

public partial class SpriteText
{
	internal class SpriteTextDrawNode : TexturedShaderDrawNode
	{
		protected new SpriteText Source => (SpriteText)base.Source;

		private List<ScreenSpaceCharacterPart>? _parts;

		public SpriteTextDrawNode(SpriteText source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			updateScreenSpaceCharacters();
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			Debug.Assert(_parts is not null);

			for (int i = 0; i < _parts.Count; i++)
			{
				renderer.DrawQuad(_parts[i].Texture, _parts[i].DrawQuad, DrawColorInfo);
			}
		}

		private void updateScreenSpaceCharacters()
		{
			int partCount = Source.Characters.Count;

			if (_parts == null)
				_parts = new List<ScreenSpaceCharacterPart>(partCount);
			else
			{
				_parts.Clear();
				_parts.EnsureCapacity(partCount);
			}

			foreach (var character in Source.Characters)
			{
				_parts.Add(new ScreenSpaceCharacterPart
				{
					DrawQuad = Source.ToScreenSpace(character.DrawRectangle),
					Texture = character.Texture
				});
			}
		}
	}

	internal struct ScreenSpaceCharacterPart
	{
		public Quad DrawQuad;

		public Texture Texture;
	}
}
