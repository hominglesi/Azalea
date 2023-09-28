using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;

namespace Azalea.Design.Shapes;

public partial class HollowBox : GameObject
{
	public class HollowBoxDrawNode : DrawNode
	{
		protected new HollowBox Source => (HollowBox)base.Source;

		protected float Thickness { get; set; }
		protected Quad ScreenSpaceDrawQuad { get; set; }
		protected DrawColorInfo ColorInfo { get; set; }

		public HollowBoxDrawNode(HollowBox source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			Thickness = Source.Thickness;
			ScreenSpaceDrawQuad = Source.ScreenSpaceDrawQuad;
			ColorInfo = Source.DrawColorInfo;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			renderer.DrawRectangle(ScreenSpaceDrawQuad, Thickness, ColorInfo);
		}
	}
}

