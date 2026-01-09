using Azalea.Design.Containers;
using Azalea.Design.UserInterface;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Sprites;
using Azalea.Inputs.Events;

namespace Azalea.Design.Controls;
public class FileInputControl : AbstractFileInput
{
	private readonly SpriteText _chosenFileText;

	public FileInputControl()
	{
		Size = new(1, 36);
		RelativeSizeAxes = Axes.X;

		BorderColor = new Color(206, 212, 218);
		BorderThickness = 1;
		Masking = true;

		Add(new FlexContainer()
		{
			RelativeSizeAxes = Axes.Both,
			Wrapping = FlexWrapping.NoWrapping,
			Alignment = FlexAlignment.Center,
			Children = [
				new SelectFileButton(this),
				new GameObject(){
					Width = 12,
				},
				_chosenFileText = new SpriteText(){
					Font = FontUsage.Default.With(size: 16),
					Color = ControlPalette.DarkTextColor,
					Text = "No file chosen"
				}
			]
		});
	}

	protected override bool OnFileDropped(FileDroppedEvent e)
	{
		var output = base.OnFileDropped(e);

		if (SelectedFilePaths.Count == 0)
			_chosenFileText.Text = "No file chosen";
		else if (SelectedFilePaths.Count == 1)
			_chosenFileText.Text = SelectedFilePaths[0];
		else
			_chosenFileText.Text = $"{SelectedFilePaths.Count} Items";

		return output;
	}

	private class SelectFileButton : BasicButton
	{
		private readonly AbstractFileInput _parent;

		public SelectFileButton(AbstractFileInput parent)
		{
			_parent = parent;

			Size = new(106, 1);
			BackgroundColor = new Color(233, 236, 239);
			//HoveredColor = new Color(221, 224, 227);
			// We keep the hovered color the same since opening the file
			// dialog is not supported
			HoveredColor = BackgroundColor;
			RelativeSizeAxes = Axes.Y;
			BorderColor = new Color(206, 212, 218);
			BorderThickness = 1;
			Text = "Choose File";
			FontSize = 16;
			TextColor = ControlPalette.DarkTextColor;
		}

		public override bool AcceptsFiles => true;
		protected override bool OnFileDropped(FileDroppedEvent e)
		{
			_parent.TriggerEvent(e);

			return true;
		}
	}
}
