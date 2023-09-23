using Azalea.Graphics.Colors;

namespace Azalea.Graphics;

public interface IGameObject
{
	DrawColorInfo DrawColorInfo { get; }

	DrawInfo DrawInfo { get; }
}
