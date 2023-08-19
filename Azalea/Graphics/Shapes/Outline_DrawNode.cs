using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;

namespace Azalea.Graphics.Shapes;

public partial class Outline
{
    internal class OutlineWrapperDrawNode : DrawNode
    {
        protected new OutlineWrapper Source => (OutlineWrapper)base.Source;

        protected float Thickness { get; set; }
        protected Quad ScreenSpaceDrawQuad { get; set; }
        protected DrawColorInfo ColorInfo { get; set; }

        public OutlineWrapperDrawNode(OutlineWrapper source)
            : base(source) { }

        public override void ApplyState()
        {
            base.ApplyState();

            Thickness = Source.Thickness;
            ScreenSpaceDrawQuad = Source.WrappedChild.ScreenSpaceDrawQuad;
            ColorInfo = Source.DrawColorInfo;
        }

        public override void Draw(IRenderer renderer)
        {
            base.Draw(renderer);

            renderer.DrawRectangle(ScreenSpaceDrawQuad, Thickness, ColorInfo);
        }
    }
}
