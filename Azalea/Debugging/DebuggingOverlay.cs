using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;

namespace Azalea.Debugging;
public class DebuggingOverlay : Composition
{
	private SplitContainer _horizontalSplit;
	private SplitContainer _verticalSplit;

	private Composition _leftContainer;
	private Composition _bottomContainer;
	private Composition _mainSplitContainer;
	private Composition _gameContainer;

	public DebugConsole DebugConsole { get; private set; }
	public DebugDisplayValues DisplayValues { get; private set; }

	public DebugInspector Inspector;
	private DebugSceneGraph _sceneGraph;

	internal void Initialize()
	{
		//We need to initialize this later because the constructor is called before the game has been initialized

		_leftContainer = new Composition()
		{
			BackgroundColor = new Color(51, 51, 51),
			Masking = true,
		};

		_bottomContainer = new Composition()
		{
			BackgroundColor = new Color(82, 82, 82),
		};

		_mainSplitContainer = new Composition();
		_gameContainer = new Composition()
		{
			RelativeSizeAxes = Axes.Both
		};

		_verticalSplit = new SplitContainer(_mainSplitContainer, _bottomContainer)
		{
			Direction = SplitDirection.Vertical
		};

		_horizontalSplit = new SplitContainer(_leftContainer, _verticalSplit)
		{
			RelativeSizeAxes = Axes.Both,
		};

		AddInternal(new ColliderDebug());

		DebugConsole = new DebugConsole();

		AddInternal(DisplayValues = new DebugDisplayValues()
		{
			Origin = Anchor.BottomLeft,
			Anchor = Anchor.BottomLeft,
			Direction = FlexDirection.VerticalReverse,
			RelativeSizeAxes = Axes.Both,
			Wrapping = FlexWrapping.Wrap,
			Position = new(5, -5),
			Spacing = new(5),
		});

		DisplayValues.AddDisplayedValue("Fps", () => Time.FpsCount);

		AddInternal(_gameContainer);
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
			if (_debuggerInitialized == false)
				initializeDebugger();

			if (_debuggerExpanded)
			{
				RemoveInternal(_horizontalSplit);
				_mainSplitContainer.Remove(_gameContainer);
				AddInternal(_gameContainer);
			}
			else
			{
				RemoveInternal(_gameContainer);
				_mainSplitContainer.Add(_gameContainer);
				AddInternal(_horizontalSplit);
			}
			_debuggerExpanded = !_debuggerExpanded;
		}

		if (e.Key == Keys.F9)
		{
			if (DebugConsole.Parent == null)
			{
				AddInternal(DebugConsole);
				DebugConsole.Activate();
			}
			else
			{
				DebugConsole.Deactivate();
				RemoveInternal(DebugConsole);
			}
		}

		if (e.Key == Keys.F10)
		{
			if (DisplayValues.Parent == null)
				AddInternal(DisplayValues);
			else
				RemoveInternal(DisplayValues);
		}

		return base.OnKeyDown(e);
	}

	public override void Add(GameObject gameObject) => _gameContainer.Add(gameObject);
	public override bool Remove(GameObject gameObject) => _gameContainer.Remove(gameObject);
	public override void Clear() => _gameContainer.Clear();
}
