using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Platform;
using System.Numerics;

namespace Azalea.Debugging;
public class DebuggingOverlay : ContentContainer
{
	private const float LeftContainerSize = 0.25f;
	private const float BottomContainerSize = 0.25f;

	private Composition _leftContainer;
	private Composition _bottomContainer;

	public DebugConsole DebugConsole { get; private set; }
	public DebugDisplayValues DisplayValues { get; private set; }

	public DebugInspector Inspector;
	private DebugSceneGraph _sceneGraph;

	internal void Initialize()
	{
		//We need to initialize this later because the constructor is called before the game has been initialized
		ContentComposition.Origin = Anchor.TopRight;
		ContentComposition.Anchor = Anchor.TopRight;

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

		//AddInternal(new ColliderDebug());

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
}
