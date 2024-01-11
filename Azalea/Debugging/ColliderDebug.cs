using Azalea.Design.Components;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Physics.Colliders;

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
			foreach (var collider in ComponentStorage<RectCollider>.GetComponents())
			{
				renderer.DrawRectangle(collider.Parent.DrawRectangle, collider.Parent.DrawInfo.Matrix, new Boundary(4), new DrawColorInfo(new Color(20, 255, 20)), false);
			}
		}
	}
}
