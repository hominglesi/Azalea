using Microsoft.Xna.Framework;

namespace Azalea.Platform.XNA;

internal sealed class GameWrapper : Game
{
    internal GraphicsDeviceManager Graphics => _graphics;
    private readonly GraphicsDeviceManager _graphics;

    public GameWrapper()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
    }

    internal event Action? OnInitialize;
    protected override void Initialize()
    {
        OnInitialize?.Invoke();
        base.Initialize();
    }

    internal event Action? OnLoadContent;
    protected override void LoadContent()
    {
        OnLoadContent?.Invoke();
    }

    internal event Action<GameTime>? OnUpdate;
    protected override void Update(GameTime gameTime)
    {
        OnUpdate?.Invoke(gameTime);
        base.Update(gameTime);
    }

    internal event Action<GameTime>? OnDraw;
    protected override void Draw(GameTime gameTime)
    {
        OnDraw?.Invoke(gameTime);
        base.Draw(gameTime);
    }
}
