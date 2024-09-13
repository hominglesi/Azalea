using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;
using System.Diagnostics;

namespace Azalea.Debugging;
public class DebuggingOverlay : Composition
{
	private GameObject? _debuggingOverlay;
	private BasicDockingContainer? _leftDocker;
	private BasicDockingContainer? _bottomDocker;
	private BasicDockingContainer? _mainDocker;

	private Composition? _gameCategoryContainer;
	private Composition _gameContainer;

	public DebugConsole DebugConsole { get; init; }
	public DebugDisplayValues DisplayValues { get; init; }

	public DebugInspector Inspector;
	private DebugSceneGraph _sceneGraph;

	public DebuggingOverlay()
	{
		AddRangeInternal(new GameObject[]
		{
			_gameContainer = new Composition()
			{
				RelativeSizeAxes = Axes.Both
			},
			DisplayValues = new DebugDisplayValues()
			{
				Origin = Anchor.BottomLeft,
				Anchor = Anchor.BottomLeft,
				Direction = FlexDirection.VerticalReverse,
				RelativeSizeAxes = Axes.Both,
				Wrapping = FlexWrapping.Wrap,
				Position = new(5, -5),
				Spacing = new(5),
				Depth = -1000
			},
			new ColliderDebug()
		});

		DebugConsole = new DebugConsole();
		Inspector = new DebugInspector();
		_sceneGraph = new DebugSceneGraph(this);

		DisplayValues.AddDisplayedValue("Fps", () => Time.FpsCount);
	}

	private void initializeDebugger()
	{
		_debuggingOverlay = new SplitContainer(
			_leftDocker = new BasicDockingContainer()
			{
				ContentPadding = new(0)
			},
			new SplitContainer(
				_mainDocker = new BasicDockingContainer()
				{
					ContentPadding = new(0)
				},
				_bottomDocker = new BasicDockingContainer()
				{
					ContentPadding = new(0),
				}
			)
			{
				Direction = SplitDirection.Vertical,
				ReversedPriority = true
			}
		)
		{
			RelativeSizeAxes = Axes.Both,
		};

		_mainDocker.AddDockable("Game", _gameCategoryContainer = new Composition()
		{
			RelativeSizeAxes = Axes.Both
		});

		_mainDocker.AddDockable("Editor", new Composition()
		{
			RelativeSizeAxes = Axes.Both
		});

		_leftDocker.ContentBackground.Color = new Color(51, 51, 51);
		_leftDocker.AddDockable("Inspector", new Composition()
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
						Child = Inspector
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
						Child = _sceneGraph
					}
				}
			}
		});

		_bottomDocker.ContentBackground.Color = new Color(82, 82, 82);
		_bottomDocker.AddDockable("Resources", new ResourceExplorer()
		{
			RelativeSizeAxes = Axes.Both
		});

		Inspector.SetObservedObject(this);
		_sceneGraph.ObjectSelected += Editor.InspectObject;
	}

	private bool _debuggerExpanded;
	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Q && Input.GetKey(Keys.ControlLeft).Pressed)
		{
			if (_debuggingOverlay == null)
				initializeDebugger();

			Debug.Assert(_debuggingOverlay is not null);
			Debug.Assert(_gameCategoryContainer is not null);

			if (_debuggerExpanded)
			{
				RemoveInternal(_debuggingOverlay);
				_gameCategoryContainer.Remove(_gameContainer);
				AddInternal(_gameContainer);
			}
			else
			{
				RemoveInternal(_gameContainer);
				_gameCategoryContainer.Add(_gameContainer);
				AddInternal(_debuggingOverlay);
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
