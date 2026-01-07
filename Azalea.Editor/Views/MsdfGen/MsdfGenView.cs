using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;
using Azalea.Platform;
using System.Diagnostics;
using System.Text;

namespace Azalea.Editor.Views.MsdfGen;
internal class MsdfGenView : Composition
{
	private string? _fontPath = @"D:\Programming\Azalea\Azalea\Resources\Fonts\Roboto-Regular.ttf";
	private string? _outputPath = @"D:\Programming\Azalea\Azalea\Resources\Fonts\Roboto-Regular-Gen";
	private int _size = 48;

	public MsdfGenView()
	{
		Add(new FlexContainer()
		{
			Direction = FlexDirection.Vertical,
			Children = [
				new SpriteText(){
					Text = "Font path: " + _fontPath,
				},
				new SpriteText(){
					Text = "Output path: " + _outputPath,
				},
				new SpriteText(){
					Text = "Size: " + _size,
				},
				new AcceptFilesButton(){
					Text = "Generate",
					ClickAction = _ => generate(_fontPath, _outputPath, _size)
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

		System.Diagnostics.Process.Start(
			@"D:\Programming\Azalea\Azalea.VisualTests.Desktop\bin\Debug\net8.0\win-x64\bin\msdf-atlas-gen.exe",
			args.ToString());
	}

	private class AcceptFilesButton : BasicButton
	{
		protected override bool OnHover(HoverEvent e)
		{
			Window.AcceptFiles = true;
			return base.OnHover(e);
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			Window.AcceptFiles = false;
			base.OnHoverLost(e);
		}
	}
}
