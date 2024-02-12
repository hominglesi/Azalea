using Azalea.Design.Components;
using Azalea.Design.Shapes;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Physics.Colliders;
using System.Numerics;

namespace Azalea.Debugging;
public class ColliderDebug : GameObject
{
	public bool IsShown { get; set; }
	private bool _isToggled = false;

	protected override void Update()
	{
		if (Input.GetKey(Keys.W).Down && Input.GetKey(Keys.ControlLeft).Pressed && Input.GetKey(Keys.ShiftLeft).Pressed)
			_isToggled = !_isToggled;

		IsShown = _isToggled || (Input.GetKey(Keys.W).Pressed && Input.GetKey(Keys.ControlLeft).Pressed);
	}

	public override void Draw(IRenderer renderer)
	{
		if (IsShown)
		{
			var color = new DrawColorInfo(new Color(45, 75, 23, 80));
			var color2 = new DrawColorInfo(new Color(84, 42, 86, 140));

			renderer.BindShader(renderer.QuadShader);

			foreach (var collider in ComponentStorage<RectCollider>.GetComponents())
			{
				renderer.DrawQuad(renderer.WhitePixel, collider.Quad, color);

				var centerQuad = new Quad(collider.Position - new Vector2(5), new(10));

				renderer.DrawQuad(renderer.WhitePixel, centerQuad, color2);
			}

			renderer.BindShader(Circle.CircleShader);

			foreach (var collider in ComponentStorage<CircleCollider>.GetComponents())
			{
				var quad = new Quad(collider.Position - new Vector2(collider.Radius), new Vector2(collider.Radius * 2));

				renderer.DrawQuad(renderer.WhitePixel, quad, color);
			}
		}
	}
}
