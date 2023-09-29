using Azalea.Graphics;
using Azalea.Graphics.Rendering;
using System.Collections.Generic;

namespace Azalea.Design.Containers;

public partial class CompositeGameObject
{
	protected class CompositeGameObjectDrawNode : DrawNode, ICompositeDrawNode
	{
		public List<DrawNode>? Children { get; set; }

		protected new CompositeGameObject Source => (CompositeGameObject)base.Source;

		public CompositeGameObjectDrawNode(CompositeGameObject source)
			: base(source) { }

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			if (Children != null)
				foreach (var child in Children)
					child.Draw(renderer);
		}
	}
}
