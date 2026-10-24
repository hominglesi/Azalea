using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Textures;
using Azalea.IO.Resources;
using Azalea.Numerics;
using Azalea.Platform;
using Azalea.Platform.Rendering;
using Azalea.Platform.Rendering.Coordination;
using System;
using System.Collections.Generic;

namespace Azalea.SceneGraph;

public class World2D : GameObject
{
	public override void Draw(RenderCoordinator coordinator)
	{
		coordinator.BindShader(GameHost.Instance.Loader.DefaultQuadShader);
		coordinator.BindTexture(Assets.WhitePixelNative);
		coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, ScreenSpaceDrawQuad, DrawColorQuad, Rectangle.One);

		_rootEntity.Draw(coordinator);
	}

	#region RootEntity
	private readonly RootEntity _rootEntity = new();

	public IReadOnlyList<Entity> Children
	{
		get => _rootEntity.Children;
		set => _rootEntity.Children = value;
	}

	public Entity Child
	{
		get => _rootEntity.Child;
		set => _rootEntity.Child = value;
	}

	public void Add(Entity entity) => _rootEntity.Add(entity);
	public void AddRange(IEnumerable<Entity> entities) => _rootEntity.AddRange(entities);
	public void Remove(Entity entity) => _rootEntity.Remove(entity);
	public void RemoveRange(IEnumerable<Entity> entities) => _rootEntity.RemoveRange(entities);
	public void Clear() => _rootEntity.Clear();

	class RootEntity : Entity
	{
		private readonly ComponentArray<SpriteComponent> _sprites = [];

		protected override void OnChildIncluded(Entity child)
		{
			foreach (var grandchild in child.Children)
				OnChildIncluded(grandchild);

			if (child is IHasSpriteComponent sprite)
				_sprites.Add(sprite.SpriteComponent);
		}

		public void Draw(RenderCoordinator coordinator)
		{
			foreach (var sprite in _sprites)
			{
				var texture = sprite.Texture ?? Assets.MissingTexture;
				var shader = sprite.Shader ?? GameHost.Instance.Loader.DefaultQuadShader;
				var drawColorQuad = new ColorQuad(Palette.White);

				switch (texture)
				{
					case Texture tex:
						switch (tex.NativeTexture.State)
						{
							case TextureState.Ready:
								coordinator.BindShader(shader);
								coordinator.BindTexture(tex.NativeTexture);
								coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, new Quad(new(200), new(500)), drawColorQuad, tex.UVCoordinates);
								break;
							case TextureState.Loading:
								throw new NotImplementedException();
							case TextureState.Missing:
								throw new NotImplementedException();
						}

						break;
					case TextureAnimation texAnim:
						throw new NotImplementedException();
				}
			}
		}
	}

	#endregion
}
