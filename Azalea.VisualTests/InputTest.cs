using Azalea.Graphics;
using Azalea.Graphics.Shapes;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using System;

namespace Azalea.VisualTests;
public class InputTest : TestScene
{
	public InputTest()
	{
		Add(new InputLoggingObject("Orange")
		{
			Color = Color.Orange,
			Position = new(300, 200),
			Size = new(200, 300)
		});

		Add(new InputLoggingObject("Purple")
		{
			Color = Color.Purple,
			Position = new(400, 300),
			Size = new(200, 300)
		});
	}

	protected override void Update()
	{
		if (Input.GetKey(Keys.P).Down)
			Console.WriteLine("P recieved directly");

		var hoveredObjects = Input.GetHoveredObjects();
		Console.WriteLine(hoveredObjects.Count);
	}

	private class InputLoggingObject : Box
	{
		private string _name;
		public InputLoggingObject(string name)
		{
			_name = name;
		}

		protected override bool OnHover(HoverEvent e)
		{
			Console.WriteLine($"{_name} Object hovered");
			return base.OnHover(e);
		}

		protected override void OnHoverLost(HoverLostEvent e)
		{
			Console.WriteLine($"{_name} Object hover lost");
			base.OnHoverLost(e);
		}

		protected override bool OnKeyDown(KeyDownEvent e)
		{
			Console.WriteLine($"{_name} Key down: {e.Key}");
			return base.OnKeyDown(e);
		}

		protected override bool OnKeyUp(KeyUpEvent e)
		{
			Console.WriteLine($"{_name} Key up: {e.Key}");
			return base.OnKeyUp(e);
		}
		protected override bool OnMouseDown(MouseDownEvent e)
		{
			Console.WriteLine($"{_name} Mouse down: {e.Button}");
			return true;
		}

		protected override bool OnMouseUp(MouseUpEvent e)
		{
			Console.WriteLine($"{_name} Mouse up: {e.Button}");
			return true;
		}
	}
}


