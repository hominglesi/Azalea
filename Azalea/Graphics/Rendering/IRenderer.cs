using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;
using Azalea.Numerics;

namespace Azalea.Graphics.Rendering;

public interface IRenderer
{
	public const int VERTICES_PER_QUAD = 4;
	public const int INDICES_PER_QUAD = VERTICES_PER_QUAD + 2;
	public const int MAX_MIPMAP_LEVELS = 3;

	public Color ClearColor { get; set; }
	public bool AutomaticallyClear { get; set; }
	internal IVertexBatch<TexturedVertex2D> DefaultQuadBatch { get; }
	public Texture WhitePixel { get; }

	internal void BeginFrame();
	internal void FinishFrame();
	public void Clear();
	internal void FlushCurrentBatch();
	internal void SetViewport(Vector2Int size);

	internal IVertexBatch CreateQuadBatch(int size);
	internal Texture CreateTexture(Image image);
	internal bool BindTexture(Texture texture, int unit = 0);

	internal void PushScissor(RectangleInt scissorRect);
	internal void PopScissor();
}
