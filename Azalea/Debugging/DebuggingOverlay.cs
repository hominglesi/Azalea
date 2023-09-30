using Azalea.Design.Containers;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Inputs.Events;

namespace Azalea.Debugging;
public class DebuggingOverlay : Composition
{
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
	private Composition _topContainer;

	private bool _initialized;
	private void initialize()
	{
		//We need to initialize this later because the constructor is called before the game has been initialized
		AddInternal(_gameContainer = new Composition()
		{
			Origin = Anchor.BottomRight,
			Anchor = Anchor.BottomRight,
			RelativeSizeAxes = Axes.Both,
			Size = new(1f, 1f)
		});
		RemoveInternal(InternalComposition);
		_gameContainer.Add(InternalComposition);

		AddInternal(_leftContainer = new Composition()
		{
			RelativeSizeAxes = Axes.Both,
			Size = new(0, 0),
			BackgroundColor = Palette.Gray,
			Masking = true,
		});

		AddInternal(_topContainer = new Composition()
		{
			RelativePositionAxes = Axes.Both,
			Position = new(0.2f, 0),
			RelativeSizeAxes = Axes.Both,
			Size = new(0, 0),
			BackgroundColor = Palette.Gray
		});
	}

	private bool _debuggerExpanded;
	private void expandDebugger()
	{
		_gameContainer.Size = new(0.8f, 0.8f);
		_leftContainer.Size = new(0.2f, 1);
		_topContainer.Size = new(0.8f, 0.2f);

		if (_debuggerInitialized == false) initializeDebugger();

		_debuggerExpanded = true;
	}

	private void hideDebugger()
	{
		_gameContainer.Size = new(1, 1);
		_leftContainer.Size = new(0, 0);
		_topContainer.Size = new(0, 0);

		_debuggerExpanded = false;
	}

	private bool _debuggerInitialized;
	private void initializeDebugger()
	{
		_leftContainer.Add(new DebugSceneGraph(this));

		_debuggerInitialized = true;
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Inputs.Keys.F1)
		{
			if (_debuggerExpanded) hideDebugger();
			else expandDebugger();
		}

		return base.OnKeyDown(e);
	}
}
