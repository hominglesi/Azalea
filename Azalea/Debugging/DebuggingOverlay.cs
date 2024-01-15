using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System.Numerics;

namespace Azalea.Debugging;
public class DebuggingOverlay : ContentContainer
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

		base.Update();
	}

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
		ContentComposition.Origin = Anchor.TopRight;
		ContentComposition.Anchor = Anchor.TopRight;

		AddInternal(FpsDisplay);

		AddInternal(_leftContainer = new Composition()
		{
			BackgroundColor = new Color(51, 51, 51),
			Masking = true,
		});

		AddInternal(_bottomContainer = new Composition()
		{
			Origin = Anchor.BottomRight,
			Anchor = Anchor.BottomRight,
			BackgroundColor = new Color(82, 82, 82),
		});

		AddInternal(new ColliderDebug());
	}

	protected override void UpdateContentLayout()
	{
		if (_debuggerExpanded)
		{
			if (_debuggerInitialized == false) initializeDebugger();

			ContentComposition.Width = DrawSize.X * (1 - LeftContainerSize);
			ContentComposition.Height = DrawSize.Y * (1 - BottomContainerSize);
			_leftContainer.Height = DrawSize.Y;
			_leftContainer.Width = DrawSize.X * LeftContainerSize;
			_bottomContainer.Height = DrawSize.Y * BottomContainerSize;
			_bottomContainer.Width = DrawSize.X * (1 - LeftContainerSize);
		}
		else
		{
			ContentComposition.Size = DrawSize;
			_leftContainer.Size = Vector2.Zero;
			_bottomContainer.Size = Vector2.Zero;
		}
	}

	private bool _debuggerExpanded;

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
			_debuggerExpanded = !_debuggerExpanded;
			UpdateContentLayout();
		}

		return base.OnKeyDown(e);
	}
}
