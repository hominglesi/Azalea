namespace Azalea.VisualTests;

internal class VisualTests : AzaleaGame
{
    private FillFlowContainer _container;

    protected override void OnInitialize()
    {
        Resources.AddStore(new NamespacedResourceStore<byte[]>(new DllResourceStore(typeof(VisualTests).Assembly), "Resources"));

        Host.Renderer.ClearColor = Color.Azalea;

        Add(_container = new FillFlowContainer()
        {
            MaximumSize = new Vector2(500, 500)
        });
        _container.Direction = FillDirection.Vertical;

        _container.Children = new GameObject[]
        {
            new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Size = new Vector2(100, 100)
            },
            new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Size = new Vector2(100, 100)
            },
            new Outline(container = new GridContainer()
            {
                Content = GenerateRandomContent(5, 5).ToJagged(),
                Size = new Vector2(400, 200),
                Position = Vector2.Zero
                /*
                Content = content.ToJagged(),
                Size = new Vector2(400, 200),
                Position = new Vector2(50, 500)*/
            })/*,
            solid = new TestGameObject()
            {
                Texture = Host.Renderer.WhitePixel,
                Position = new Vector2(200, 200),
                Size = new Vector2(300, 50),
                Color = Color.Lime
            },
            new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Size = new Vector2(100, 100)
            },
            new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Size = new Vector2(100, 100)
            },
            new Sprite()
            {
                Texture = Assets.GetTexture("wall.png"),
                Size = new Vector2(100, 100)
            },
        };
    }

    private bool removed = false;

    protected override void Update()
    {
        //_container.Size = Input.MousePosition;

        Console.WriteLine(_container.Size);
    }
}
