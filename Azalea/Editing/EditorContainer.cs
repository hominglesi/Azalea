using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Editing.Design;
using Azalea.Editing.Legacy;
using Azalea.Editing.Views;
using Azalea.Graphics;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Platform;

namespace Azalea.Editing;
public class EditorContainer : Composition
{
	private readonly AzaleaGame _game;
	internal EditorContainerExpanded Expanded { get; }
	internal DisplayValues DisplayValues { get; }
	internal EditorConsole Console { get; }

	public EditorContainer(AzaleaGame game)
	{
		RelativeSizeAxes = Axes.Both;
		Add(_game = game);
		Expanded = new EditorContainerExpanded();
		AddInternal(DisplayValues = new DisplayValues());
		Console = new EditorConsole();

		DisplayValues.AddDisplayedValue("Fps", () => Time.FpsCount);
	}

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if (e.Key == Keys.Q && Input.GetKey(Keys.ControlLeft).Pressed)
		{
			if (Expanded.Parent is null)
			{
				Remove(_game);
				Add(Expanded);
				Expanded.AttachGame(_game);
			}
			else
			{
				Remove(Expanded);
				Expanded.DetachGame(_game);
				Add(_game);
			}

			return true;
		}

		if (e.Key == Keys.F9)
		{
			if (Console.Parent is null)
			{
				AddInternal(Console);
				Console.Activate();
			}
			else
			{
				Console.Deactivate();
				RemoveInternal(Console);
			}
			return true;
		}

		if (e.Key == Keys.F10)
		{
			if (DisplayValues.Parent is null)
				AddInternal(DisplayValues);
			else
				RemoveInternal(DisplayValues);

			return true;
		}

		return base.OnKeyDown(e);
	}

	internal class EditorContainerExpanded : Composition
	{
		private readonly DockingContainer _mainDocker;
		private readonly DockingContainer _bottomDocker;
		private readonly DockingContainer _sideTopDocker;
		private readonly DockingContainer _sideBottomDocker;

		private readonly Composition _gameView;
		private readonly LegacyColliderDebug _colliderDebug;
		public TemplateEditor TemplateEditor { get; }
		public LegacyInspector LegacyInspector { get; }

		public EditorContainerExpanded()
		{
			RelativeSizeAxes = Axes.Both;

			Add(new SplitContainer(
					new SplitContainer(
						_sideTopDocker = new EditorDockingContainer(new(15)),
						_sideBottomDocker = new EditorDockingContainer(new(15)))
					{
						Direction = SplitDirection.Vertical,
						ReversedPriority = true
					},
					new SplitContainer(
						_mainDocker = new EditorDockingContainer(Boundary.Zero, true),
						_bottomDocker = new EditorDockingContainer(Boundary.Zero))
					{
						Direction = SplitDirection.Vertical,
						ReversedPriority = true,
						Depth = 1000
					}
				)
			{
				RelativeSizeAxes = Axes.Both
			});

			_mainDocker.AddDockable("Game", _gameView = new Composition
			{
				RelativeSizeAxes = Axes.Both
			});
			_mainDocker.AddDockable("Template Editor", TemplateEditor = new TemplateEditor());

			_bottomDocker.AddDockable("Game Resources", new EditorExplorer(Assets.MainStore));

			if (Assets.PersistentStoreExists)
				_bottomDocker.AddDockable("Persistent Resources", new EditorExplorer(Assets.PersistentStore));

			if (Assets.ReflectedStoreExists)
				_bottomDocker.AddDockable("Reflected Resources", new EditorExplorer(Assets.ReflectedStore));

			_sideTopDocker.AddDockable("Inspector", LegacyInspector = new LegacyInspector());
			LegacyInspector.SetObservedObject(this);

			_sideBottomDocker.AddDockable("Scene Graph", new LegacySceneGraph(this));

			_colliderDebug = new LegacyColliderDebug();
		}

		public void AttachGame(AzaleaGame game)
			=> _gameView.Add(game);
		public void DetachGame(AzaleaGame game)
			=> _gameView.Remove(game);

		public void FocusTemplateEditor()
			=> _mainDocker.FocusContent(TemplateEditor);
	}
}
