﻿using Azalea.Design.Containers;
using Azalea.Design.Docking;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Text;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests.TextRendering;
public class TextRenderingTest : TestScene
{
	private ItemBox _fontsItemBox;
	private FontInfoDisplay _fontInfoDisplay;
	private StringGlyphDisplay _stringDisplay;

	private List<string> _avalibleFonts = new();

	private DockingContainer _leftDocker;
	private DockingContainer _rightDocker;

	public TextRenderingTest()
	{

		Add(_leftDocker = new BasicDockingContainer()
		{
			RelativeSizeAxes = Axes.Y,
			Size = new(300, 1)
		});

		Add(_rightDocker = new BasicDockingContainer()
		{
			Origin = Anchor.TopRight,
			Anchor = Anchor.TopRight,
			RelativeSizeAxes = Axes.Y,
			Size = new(300, 1),
			ContentPadding = new(20)
		});

		_leftDocker.AddDockable("Fonts", _fontsItemBox = new() { RelativeSizeAxes = Axes.Both });
		_fontsItemBox.OnSelectedChanged += i => changeSelectedFont(_avalibleFonts[i]);

		_rightDocker.AddDockable("Font Info", _fontInfoDisplay = new());

		Add(_stringDisplay = new()
		{
			Position = new(300, 500),
			Text = "Ide Gas"
		});

		_avalibleFonts = getAllAssetsInDirectory("Fonts/ttf/");
		foreach (var file in _avalibleFonts)
			_fontsItemBox.Add(file);
	}

	private void changeSelectedFont(string fontPath)
	{
		var fontStream = Assets.GetStream(fontPath)!;

		var font = FontParser.Parse(fontStream);

		_fontInfoDisplay.Display(font);

		_stringDisplay.Font = font;
		_stringDisplay.GlyphScale = 300.0f / font.UnitsPerEm;
	}

	private List<string> getAllAssetsInDirectory(string directory)
	{
		var assets = Assets.MainStore.GetAvalibleResources();
		var output = new List<string>();

		foreach (var asset in assets)
		{
			if (asset.StartsWith(directory))
				output.Add(asset);
		}

		return output;
	}

	private class ItemBox : FlexContainer
	{
		private int _selectedIndex = -1;
		private List<ItemBoxItem> _items = new();

		public ItemBox()
		{
			Direction = FlexDirection.Vertical;
		}

		public void Add(string item)
		{
			var itemBoxItem = new ItemBoxItem(item);
			itemBoxItem.Click += e => onItemClicked(_items.IndexOf((ItemBoxItem)e.Target!));

			base.Add(itemBoxItem);
			_items.Add(itemBoxItem);

			if (_selectedIndex == -1)
				selectItem(0);
		}

		private void onItemClicked(int index)
		{
			if (_selectedIndex == index) return;

			if (_selectedIndex != -1)
				_items[_selectedIndex].Highlighted = false;

			selectItem(index);
		}

		private void selectItem(int index)
		{
			_selectedIndex = index;
			_items[_selectedIndex].Highlighted = true;
			OnSelectedChanged?.Invoke(_selectedIndex);
		}

		public Action<int>? OnSelectedChanged;

		public override void Add(GameObject gameObject)
			=> throw new InvalidOperationException("Don't use this");

		private class ItemBoxItem : Composition
		{
			private Box _highlightBackground;

			public ItemBoxItem(string text)
			{
				RelativeSizeAxes = Axes.X;
				AutoSizeAxes = Axes.Y;
				Add(_highlightBackground = new Box()
				{
					RelativeSizeAxes = Axes.Both,
					Color = Palette.Black,
					Alpha = 0
				});
				Add(new SpriteText()
				{
					X = 10,
					Text = text,
				});
			}

			private bool _highlighted;
			public bool Highlighted
			{
				get => _highlighted;
				set
				{
					if (_highlighted == value) return;

					_highlighted = value;

					_highlightBackground.Alpha = _highlighted ? 1 : 0;
				}
			}
		}
	}
}