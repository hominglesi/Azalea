using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using System.Collections.Generic;

namespace Azalea.Design.Containers;

public partial class CompositeGameObject
{
	protected class CompositeGameObjectDrawNode : DrawNode, ICompositeDrawNode
	{
		public List<DrawNode>? Children { get; set; }
		protected bool Masking { get; set; }

		protected new CompositeGameObject Source => (CompositeGameObject)base.Source;

		public CompositeGameObjectDrawNode(CompositeGameObject source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			Masking = Source.Masking;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			if (Masking)
			{
				renderer.PushScissor((RectangleInt)Source.DrawRectangle);
			}

			if (Children != null)
				foreach (var child in Children)
					child.Draw(renderer);

			if (Masking)
			{
				renderer.PopScissor();
			}
		}
	}
}
