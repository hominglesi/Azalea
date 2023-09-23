using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering.Vertices;
using Azalea.Graphics.Textures;

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

	internal void Initialize();
	internal void BeginFrame();
	internal void FinishFrame();
	public void Clear();
	internal void FlushCurrentBatch();

	internal IVertexBatch CreateQuadBatch(int size);
	internal Texture CreateTexture(int width, int height);
	internal bool BindTexture(Texture texture, int unit = 0);
}
