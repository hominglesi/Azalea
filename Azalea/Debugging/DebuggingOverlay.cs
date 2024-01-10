using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System.Numerics;

namespace Azalea.Debugging;
public class DebuggingOverlay : Composition
{
	private const float LeftContainerSize = 0.25f;
	private const float BottomContainerSize = 0.25f;

	protected override void Update()
	{
		if (_initialized == false)
		{
			initialize();
			_initialized = true;
		}
	}

	private Composition _gameContainer;
	private Composition _leftContainer;
	private Composition _bottomContainer;

	public DebugInspector Inspector;
	private DebugSceneGraph _sceneGraph;

	private static SpriteText? _fpsDislpay;
	public static SpriteText FpsDisplay
	{
		get
		{
			_fpsDislpay ??= new FpsDisplay()
			{
				Origin = Anchor.TopRight,
				Anchor = Anchor.TopRight,
			};
			return _fpsDislpay;
		}
	}

	private bool _initialized;
	private void initialize()
	{
		//We need to initialize this later because the constructor is called before the game has been initialized
		AddInternal(_gameContainer = new Composition()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			RelativeSizeAxes = Axes.Both,
			Size = new(1f, 1f)
		});
		//REMOVED WHEN COMPOSITION WAS SIMPLIFIED PROBABLY BROKE SOMETHING
		//RemoveInternal(InternalComposition);
		//_gameContainer.Add(InternalComposition);

		AddInternal(FpsDisplay);

		AddInternal(_leftContainer = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Size = new(0, 1),
			BackgroundColor = new Color(51, 51, 51),
			Masking = true,
		});

		AddInternal(_bottomContainer = new Composition()
		{
			Origin = Anchor.BottomRight,
			Anchor = Anchor.BottomRight,
			RelativeSizeAxes = Axes.Both,
			Size = new(1 - LeftContainerSize, 0),
			BackgroundColor = new Color(82, 82, 82),
		});
	}

	private bool _debuggerExpanded;
	private void expandDebugger()
	{
		_gameContainer.Size = new(1 - LeftContainerSize, 1 - BottomContainerSize);
		_leftContainer.Width = LeftContainerSize;
		_bottomContainer.Height = BottomContainerSize;

		if (_debuggerInitialized == false) initializeDebugger();

		_debuggerExpanded = true;
	}

	private void hideDebugger()
	{
		_gameContainer.Size = Vector2.One;
		_leftContainer.Width = 0;
		_bottomContainer.Height = 0;

		_debuggerExpanded = false;
	}

	private bool _debuggerInitialized;
	private void initializeDebugger()
	{
		_leftContainer.Add(new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Padding = new(16),
			Children = new GameObject[]
			{
				new Composition()
				{
					RelativeSizeAxes = Axes.X,
					Size = new(1, 32),
					BackgroundColor = new Color(122, 122, 122),
					Child = new FlexContainer()
					{
						RelativeSizeAxes = Axes.Both,
						Direction = FlexDirection.HorizontalReverse,
						Children = new GameObject[]
						{
							new DebugSelectPointer()
							{
								Margin = new(4),
								Size = new(24, 24),
							}
						}
					},
				},
				new Composition()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(1, 0.50f),
					Position = new(0, 40),
					BackgroundColor = new Color(122, 122, 122),
					Child = new ScrollableContainer()
					{
						RelativeSizeAxes = Axes.Both,
						Padding = new(5),
						Child = Inspector = new DebugInspector()
						{
							RelativeSizeAxes = Axes.Both,
						}
					},
				},
				new Composition()
				{
					RelativeSizeAxes = Axes.Both,
					Size = new(1, 0.41f),
					RelativePositionAxes = Axes.Y,
					Y = 0.58f,
					BackgroundColor = new Color(122, 122, 122),
					Child = new ScrollableContainer()
					{
						RelativeSizeAxes = Axes.Both,
						Padding = new(5),
						Child = _sceneGraph = new DebugSceneGraph(this)
					}
				}
			}
		});

		_bottomContainer.Add(new Composition()
		{
			RelativeSizeAxes = Axes.Both
		});

		Inspector.SetObservedObject(this);
		_sceneGraph.ObjectSelected += Editor.InspectObject;

		_debuggerInitialized = true;
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Q && Input.GetKey(Keys.ControlLeft).Pressed)
		{
			if (_debuggerExpanded) hideDebugger();
			else expandDebugger();
		}

		return base.OnKeyDown(e);
	}
}
