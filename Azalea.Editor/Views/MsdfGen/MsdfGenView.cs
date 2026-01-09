using Azalea.Design.Containers;
using Azalea.Design.Controls;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.IO.Resources;
using System.Diagnostics;
using System.Text;

namespace Azalea.Editor.Views.MsdfGen;
internal class MsdfGenView : Composition
{
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
				_fontInputControl = new FileInputControl(),
				new LabelControl(){
					Text = "Output directory:"
				},
				_outputDirectoryControl = new FileInputControl(){
					AcceptedFiles = AcceptedFileFlags.Directory
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
				},
				new Sprite(){
					Texture = Assets.GetTexture(".Fake/azalea-icon.png")
				}
			]
		});
	}

	private static void generate(string? fontPath, string? outputPath, int size, string? outputFileName = null)
	{
		Debug.Assert(fontPath is not null);

		outputPath ??= Path.GetDirectoryName(fontPath);
		outputFileName ??= Path.GetFileNameWithoutExtension(fontPath);

		var args = new StringBuilder();

		args.Append("-font ");
		args.Append(fontPath);
		args.Append(' ');

		args.Append("-allchars ");

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

		Process.Start(
			@"bin\msdf-gen.exe",
			args.ToString());
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
