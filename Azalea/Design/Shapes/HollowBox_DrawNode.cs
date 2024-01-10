using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Numerics;

namespace Azalea.Design.Shapes;

public partial class HollowBox : GameObject
{
	public class HollowBoxDrawNode : DrawNode
	{
		protected new HollowBox Source => (HollowBox)base.Source;

		protected Boundary Thickness { get; set; }
		protected bool OutsideContent { get; set; }
		protected Rectangle DrawRectangle { get; set; }
		protected DrawColorInfo ColorInfo { get; set; }
		protected Matrix3 DrawMatrix { get; set; }

		public HollowBoxDrawNode(HollowBox source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			Thickness = Source.Thickness;
			DrawRectangle = Source.DrawRectangle;
			ColorInfo = Source.DrawColorInfo;
			DrawMatrix = Source.DrawInfo.Matrix;
			OutsideContent = Source.OutsideContent;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			renderer.DrawRectangle(DrawRectangle, DrawMatrix, Thickness, ColorInfo, OutsideContent);
		}
	}
}

