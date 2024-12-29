using Azalea.Design.Containers;
using Azalea.Design.Explorer;
using Azalea.Editing.Design;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.IO.Resources;
using Azalea.Markup;
using System;
using System.IO;

namespace Azalea.Editing.Views;
internal class EditorExplorer : Composition
{
	private const float __headerHeight = 22;

	private readonly IResourceStore _store;

	private readonly HeaderButton _iconsButton;
	private readonly HeaderButton _listButton;

	private readonly EditorScrollableContainer _scrollContainer;
	private readonly ResourceExplorerList _listExplorer;
	private readonly ResourceExplorerIcons _iconsExplorer;
	private readonly SpriteText _pathText;

	public EditorExplorer(IResourceStore store)
	{
		_store = store;

		RelativeSizeAxes = Axes.Both;
		AddRange(new GameObject[]
		{
			new Composition
			{
				RelativeSizeAxes = Axes.X,
				Height = __headerHeight,
				BackgroundColor = Palette.Gray,
				Children = new GameObject[]
				{
					_pathText = new SpriteText()
					{
						X = 8,
						Origin = Anchor.CenterLeft,
						Anchor = Anchor.CenterLeft
					},
					new FlexContainer()
					{
						Width = __headerHeight * 2,
						Height = __headerHeight,
						Origin = Anchor.CenterRight,
						Anchor = Anchor.CenterRight,
						Children = new[]
						{
							_iconsButton = new HeaderButton(false, "icons", headerPressed),
							_listButton = new HeaderButton(true, "list", headerPressed),
						}
					}
				}
			},
			_scrollContainer = new EditorScrollableContainer()
			{
				RelativeSizeAxes = Axes.Both,
				NegativeSize = new(0, __headerHeight),
				Y = __headerHeight,
				Child = _listExplorer = new ResourceExplorerList()
			}
		});

		_iconsExplorer = new ResourceExplorerIcons();

		setupSubExplorer(_listExplorer);
		setupSubExplorer(_iconsExplorer);
	}

	private void setupSubExplorer(ResourceExplorer explorer)
	{
		explorer.Store = _store;
		explorer.Position = new(4);
		explorer.SubPathChanged += newSubPath => _pathText.Text = newSubPath;
		explorer.ItemSelected += itemPressed;
	}

	private void itemPressed(string path, bool isDirectory)
	{
		if (isDirectory)
			return;

		var extention = Path.GetExtension(path);

		switch (extention)
		{
			case ".txt":
			case ".cfg":
			case ".cs":
				{
					using var stream = _store.GetStream(path)!;
					using var reader = new StreamReader(stream);
					Console.WriteLine(reader.ReadToEnd());
					return;
				}
			case ".at":
				{
					using var stream = _store.GetStream(path)!;
					using var reader = new StreamReader(stream);

					Editor.FocusTemplateEditor();
					Editor.InspectTemplate(TemplateConverter.Parse(reader.ReadToEnd()));
					return;
				}
		}
	}

	private void headerPressed(HeaderButton sender)
	{
		if (sender.Pressed)
			return;

		if (sender == _iconsButton)
		{
			_iconsButton.SetPressed(true);
			_listButton.SetPressed(false);
			_scrollContainer.Child = _iconsExplorer;
			_iconsExplorer.ShowDirectory(_listExplorer.SubPath);
		}
		else
		{
			_iconsButton.SetPressed(false);
			_listButton.SetPressed(true);
			_scrollContainer.Child = _listExplorer;
			_listExplorer.ShowDirectory(_iconsExplorer.SubPath);
		}
	}

	private class HeaderButton : Composition
	{
		public bool Pressed { get; private set; }
		private readonly Action<HeaderButton> _clickAction;

		public HeaderButton(bool active, string iconName, Action<HeaderButton> clickAction)
		{
			_clickAction = clickAction;
			Size = new(__headerHeight);

			Add(new Sprite()
			{
				Origin = Anchor.Center,
				Anchor = Anchor.Center,
				Texture = Assets.GetTexture($"Textures/{iconName}-icon-small.png")
			});

			SetPressed(active);
		}

		public void SetPressed(bool pressed)
		{
			Pressed = pressed;

			if (pressed)
			{
				BackgroundColor = EditorPallete.HoverBackground;
				BackgroundObject!.Alpha = 1;
				BorderColor = EditorPallete.HoverBorderBackground;
				BorderThickness = 1;
				BorderObject!.Alpha = 1;

			}
			else
			{
				if (BackgroundObject is not null)
					BackgroundObject.Alpha = 0;

				if (BorderObject is not null)
					BorderObject.Alpha = 0;
			}
		}

		protected override bool OnClick(ClickEvent e)
		{
			_clickAction?.Invoke(this);
			return true;
		}
	}
}
