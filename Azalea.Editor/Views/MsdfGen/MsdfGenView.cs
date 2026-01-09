using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using System.Diagnostics;
using System.Text;

namespace Azalea.Editor.Views.MsdfGen;
internal class MsdfGenView : Composition
{
	public static Color TextColor => new(53, 58, 61);

	private int _size = 48;

	private FileInputControl _fontInputControl;
	private FileInputControl _outputDirectoryControl;

	public MsdfGenView()
	{
		Add(new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			RelativeSizeAxes = Axes.Both,
			Padding = new(20),
			Children = [
				new LabelControl(){
					Text = "Input font:"
				},
				_fontInputControl = new FileInputControl(){
					RelativeSizeAxes = Axes.X
				},
				new LabelControl(){
					Text = "Output directory:"
				},
				_outputDirectoryControl = new FileInputControl(){
					RelativeSizeAxes = Axes.X,
					AcceptedFiles = FileInputControl.AcceptedFileFlags.Directory
				},
				new LabelControl(){
					Text = $"Size: {_size}"
				},
				new GameObject(){
					Height = 20,
				},
				new ButtonControl(){
					Text = "Generate",
					ClickAction = _ => generate(_fontInputControl.SelectedPath,
						_outputDirectoryControl.SelectedPath, _size)
				}
			]
		});
	}

	private static void generate(string? fontPath, string? outputPath, int size)
	{
		var args = new StringBuilder();

		Debug.Assert(fontPath is not null);
		args.Append("-font ");
		args.Append(fontPath);
		args.Append(' ');

		args.Append("-allchars ");

		Debug.Assert(outputPath is not null);
		args.Append("-imageout ");
		args.Append(outputPath);
		args.Append(".bmp ");

		args.Append("-csv ");
		args.Append(outputPath);
		args.Append(".csv ");

		args.Append("-size ");
		args.Append(size);
		args.Append(' ');

		args.Append("-yorigin top");

		Process.Start(
			@"D:\Programming\Azalea\Azalea.VisualTests.Desktop\bin\Debug\net8.0\win-x64\bin\msdf-atlas-gen.exe",
			args.ToString());
	}

	private class FileInputControl : FlexContainer
	{
		public AcceptedFileFlags AcceptedFiles { get; set; } = AcceptedFileFlags.File;

		private List<string> _selectedFilePaths = [];

		public string? SelectedPath
		{
			get
			{
				if (_selectedFilePaths.Count > 0)
					return _selectedFilePaths[0];

				return null;
			}
		}

		public IEnumerable<string> SelectedPaths
		{
			get
			{
				foreach (var filePath in _selectedFilePaths)
					yield return filePath;
			}
		}

		private readonly SpriteText _chosenFileText;

		public FileInputControl()
		{
			BorderColor = new Color(206, 212, 218);
			BorderThickness = 1;
			Wrapping = FlexWrapping.NoWrapping;
			Alignment = FlexAlignment.Center;
			Height = 36;
			Masking = true;

			AddRange([
				new BasicButton(){
					Size = new(106, 1),
					BackgroundColor = new Color(233, 236, 239),
					HoveredColor = new Color(221, 224, 227),
					RelativeSizeAxes = Axes.Y,
					BorderColor = new Color(206, 212, 218),
					BorderThickness = 1,
					Text = "Choose File",
					FontSize = 16,
					TextColor = MsdfGenView.TextColor
				},
				new GameObject(){
					Width = 12,
				},
				_chosenFileText = new SpriteText(){
					Font = FontUsage.Default.With(size: 16),
					Color = MsdfGenView.TextColor,
					Text = "No file chosen"
				}
			]);
		}

		public override bool AcceptsFiles => true;
		protected override bool OnFileDropped(FileDroppedEvent e)
		{
			_selectedFilePaths.Clear();

			if (e.FilePaths.Length > 0)
			{
				for (int i = 0; i < e.FilePaths.Length; i++)
				{
					var path = e.FilePaths[i];

					if (File.Exists(path))
					{
						if (AcceptedFiles.HasFlag(AcceptedFileFlags.MultipleFiles) ||
							(AcceptedFiles.HasFlag(AcceptedFileFlags.File) && i == 0))
						{
							_selectedFilePaths.Add(path);
						}
					}
					else if (Directory.Exists(path))
					{
						if (AcceptedFiles.HasFlag(AcceptedFileFlags.MultipleDirectories) ||
							(AcceptedFiles.HasFlag(AcceptedFileFlags.Directory) && i == 0))
						{
							_selectedFilePaths.Add(path);
						}
					}
				}
			}

			if (_selectedFilePaths.Count == 0)
				_chosenFileText.Text = "No file chosen";
			else if (_selectedFilePaths.Count == 1)
				_chosenFileText.Text = _selectedFilePaths[0];
			else
				_chosenFileText.Text = $"{_selectedFilePaths.Count} Items";

			return true;
		}

		[Flags]
		public enum AcceptedFileFlags
		{
			File = 1,
			MultipleFiles = 2,
			Directory = 4,
			MultipleDirectories = 8
		}
	}

	private class LabelControl : SpriteText
	{
		public LabelControl()
		{
			Font = FontUsage.Default.With(size: 16);
			Color = MsdfGenView.TextColor;
			Margin = new Boundary(20, 0, 14, 0);
		}
	}

	private class ButtonControl : BasicButton
	{
		public ButtonControl()
		{
			Size = new(92, 38);
			BackgroundColor = new Color(13, 110, 253);
			HoveredColor = new Color(11, 94, 215);
			TextColor = Palette.White;
			FontSize = 16;
		}
	}
}
