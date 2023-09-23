using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;

namespace Azalea.Graphics;

public class DrawNode //: IDisposable
{
	protected DrawColorInfo DrawColorInfo { get; private set; }

	protected IGameObject Source { get; private set; }

	public DrawNode(IGameObject source)
	{
		Source = source;
	}

	public virtual void ApplyState()
	{
		DrawColorInfo = Source.DrawColorInfo;
	}

	public virtual void Draw(IRenderer renderer)
	{
		ApplyState();
	}
}
