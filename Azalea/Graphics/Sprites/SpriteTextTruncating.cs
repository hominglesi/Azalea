using Azalea.Design.Containers;

namespace Azalea.Graphics.Sprites;
public class SpriteTextTruncating : Composition
{
	public SpriteText InternalText { get; set; }

	public string Text
	{
		get => InternalText.Text;
		set => InternalText.Text = value;
	}

	public FontUsage Font
	{
		get => InternalText.Font;
		set => InternalText.Font = value;
	}

	public SpriteTextTruncating()
	{
		Add(InternalText = new SpriteText()
		{
			Origin = Anchor.BottomCenter,
			Anchor = Anchor.BottomCenter,
		});
	}

	private string _lastText = "";
	private float _lastWidth = 0;
	protected override void Update()
	{
		if (_lastText != InternalText.Text || _lastWidth != DrawWidth)
		{
			var targetText = InternalText.Text;
			while (InternalText.Width > DrawWidth)
			{
				if (targetText.Length <= 0)
				{
					InternalText.Text = "";
					break;
				}

				targetText = targetText[0..^1];
				InternalText.Text = targetText + "...";
			}

			_lastText = InternalText.Text;
			_lastWidth = DrawWidth;
		}

		base.Update();
	}
}
