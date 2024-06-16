using Azalea.Design.Containers;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using Azalea.Text;
using System;
using System.Collections.Generic;

namespace Azalea.VisualTests;
public class TextRenderingTest : TestScene
{
	private ItemBox _fontsItemBox;
	private SpriteText _numTablesText;
	private TextContainer _tablesContainer;

	private List<string> _avalibleFonts = new();

	public TextRenderingTest()
	{
		Add(_fontsItemBox = new()
		{
			RelativeSizeAxes = Axes.Y,
			Size = new(400, 1),
			BackgroundColor = Palette.Beige
		});
		_fontsItemBox.OnSelectedChanged += i => changeSelectedFont(_avalibleFonts[i]);

		Add(_numTablesText = new()
		{
			Position = new(420, 0)
		});

		Add(_tablesContainer = new()
		{
			Position = new(420, 30),
			Size = new(400, 700)
		});

		_avalibleFonts = getAllAssetsInDirectory("Fonts/ttf/");
		foreach (var file in _avalibleFonts)
			_fontsItemBox.Add(file);
	}

	private void changeSelectedFont(string font)
	{
		var fontStream = Assets.GetStream(font)!;

		using FontReader reader = new(fontStream);

		reader.SkipBytes(4);
		var numTables = reader.ReadUInt16();
		reader.SkipBytes(6);

		_numTablesText.Text = "numTables: " + numTables;

		_tablesContainer.Clear();

		for (int i = 0; i < numTables; i++)
		{
			var tag = reader.ReadTag();
			var checksum = reader.ReadUInt32();
			var offset = reader.ReadUInt32();
			var length = reader.ReadUInt32();

			_tablesContainer.AddText($"Tag: {tag}, Offset: {offset} \n");
		}
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
					Color = Palette.Orange,
					Alpha = 0
				});
				Add(new SpriteText()
				{
					X = 10,
					Color = Palette.Black,
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
