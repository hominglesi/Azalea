using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using Azalea.Numerics;
using Azalea.Utils;
using System.Collections.Generic;

namespace Azalea.Design.Containers;

public partial class CompositeGameObject
{
	protected class CompositeGameObjectDrawNode : DrawNode, ICompositeDrawNode
	{
		public List<DrawNode>? Children { get; set; }
		protected bool Masking { get; set; }
		protected Boundary MaskingPadding { get; set; }

		protected new CompositeGameObject Source => (CompositeGameObject)base.Source;

		public CompositeGameObjectDrawNode(CompositeGameObject source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			Masking = Source.Masking;
			MaskingPadding = Source.MaskingPadding;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			if (Masking)
			{
				var newScissor = (RectangleInt)Source.ScreenSpaceDrawQuad;
				if (MaskingPadding != Boundary.Zero)
				{
					newScissor.X -= MathUtils.Ceiling(MaskingPadding.Left);
					newScissor.Width += MathUtils.Ceiling(MaskingPadding.Horizontal);
					newScissor.Y -= MathUtils.Ceiling(MaskingPadding.Top);
					newScissor.Height += MathUtils.Ceiling(MaskingPadding.Vertical);
				}
				renderer.PushScissor(newScissor);
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
