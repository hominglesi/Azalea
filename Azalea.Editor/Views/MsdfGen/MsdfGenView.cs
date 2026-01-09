using Azalea.Design.Containers;
using Azalea.Design.Controls;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using System.Diagnostics;
using System.Text;

namespace Azalea.Editor.Views.MsdfGen;
internal class MsdfGenView : Composition
{
	private int _size = 48;

	private FileInputControl _fontInputControl;
	private FileInputControl _outputDirectoryControl;

	private FlexContainer _outputTextContainer;

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
				_fontInputControl = new FileInputControl(),
				new FormTextControl(){
					Text = "Drag a file over the control to select it."
				},
				new LabelControl(){
					Text = "Output directory:"
				},
				_outputDirectoryControl = new FileInputControl(){
					AcceptedFiles = AcceptedFileFlags.Directory
				},
				new FormTextControl(){
					Text = "Drag a directory over the control to select it."
				},
				new LabelControl(){
					Text = $"Size: {_size}"
				},
				new ButtonControl(){
					Text = "Generate",
					ClickAction = _ => generate(_fontInputControl.SelectedPath,
						_outputDirectoryControl.SelectedPath, _size),
					Margin = new(16, 0, 16, 0)
				},
				_outputTextContainer = new FlexContainer(){
					RelativeSizeAxes = Axes.X,
					AutoSizeAxes = Axes.Y,
					Margin = new(16, 0, 0, 0),
					Direction = FlexDirection.Vertical
				}
			]
		});
	}

	private void generate(string? fontPath, string? outputPath, int size, string? outputFileName = null)
	{
		_outputTextContainer.Clear();

		if (fontPath is null)
		{
			outputString("Input font must be provided!");
			return;
		}

		outputPath ??= Path.GetDirectoryName(fontPath);
		outputFileName ??= Path.GetFileNameWithoutExtension(fontPath);

		if (Directory.Exists(outputPath) == false)
			Directory.CreateDirectory(outputPath!);

		var args = new StringBuilder();

		args.Append("-font ");
		args.Append(fontPath);
		args.Append(' ');

		args.Append("-imageout ");
		args.Append(outputPath);
		args.Append('/');
		args.Append(outputFileName);
		args.Append(".bmp ");

		args.Append("-csv ");
		args.Append(outputPath);
		args.Append('/');
		args.Append(outputFileName);
		args.Append(".csv ");

		args.Append("-size ");
		args.Append(size);
		args.Append(' ');

		args.Append("-yorigin top");

		var process = new Process()
		{
			StartInfo = new()
			{
				FileName = @"bin\msdf-gen.exe",
				Arguments = args.ToString(),
				UseShellExecute = false,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				CreateNoWindow = true
			}
		};

		process.Start();

		var imageCreated = false;
		var layoutCreated = false;

		while (!process.StandardError.EndOfStream)
		{
			var line = process.StandardError.ReadLine();
			if (string.IsNullOrWhiteSpace(line))
				continue;

			if (line == "Atlas image file saved.")
			{
				imageCreated = true;
				continue;
			}

			if (line == "Glyph layout written into CSV file.")
			{
				layoutCreated = true;
				continue;
			}

			outputString(line);
		}

		if (imageCreated && layoutCreated)
			outputString("Font atlas generated successfully.");
		else
			outputString("Font atlas could not be generated.");
	}

	private void outputString(string str)
	{
		_outputTextContainer.Add(new LabelControl()
		{
			Text = str,
			Margin = new Boundary(4, 0, 4, 0)
		});
	}

	private class LabelControl : SpriteText
	{
		public LabelControl()
		{
			Font = FontUsage.Default.With(size: 16);
			Color = ControlPalette.DarkTextColor;
			Margin = new Boundary(20, 0, 14, 0);
		}
	}

	private class FormTextControl : SpriteText
	{
		public FormTextControl()
		{
			Font = FontUsage.Default.With(size: 14);
			Color = new Color(108, 117, 125);
			Margin = new Boundary(8, 0, 8, 0);
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
