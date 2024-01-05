using Azalea.Graphics;
using System;
using System.Collections.Generic;

namespace Azalea.Design.Containers.Text;
public class TextNewLine : GameObject, ITextPart
{
	private GameObject _text;

	public TextNewLine()
	{
		_text = new NewLine();
	}

	public IEnumerable<GameObject> GameObjects
	{
		get
		{
			yield return _text;
		}
	}

	public event Action<IEnumerable<GameObject>>? GameObjectPartsRecreated;

	public void RecreateGameObjectsFor(TextContainer textComposition)
	{

	}

	private class NewLine : GameObject
	{
		public NewLine()
		{
			RelativeSizeAxes = Axes.X;
			Size = new(1, 1);
		}
	}
}
