namespace Azalea.Graphics.Rendering;

public interface IRenderer
{
    public Color ClearColor { get; set; }

    internal void Clear();
}
