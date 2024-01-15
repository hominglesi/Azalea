using Azalea.Design.Components;
using Azalea.Extentions.MatrixExtentions;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Numerics;
using Azalea.Physics.Colliders;
using Azalea.Utils;
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
			foreach (var collider in ComponentStorage<RectCollider>.GetComponents())
      {
        var colliderRect = new Rectangle(Vector2.Zero, new(collider.SideA, collider.SideB));
        var matrix = collider.Parent.Parent.DrawInfo.Matrix;
        MatrixExtentions.TranslateFromLeft(ref matrix, collider.Position);
        float radians = MathUtils.DegreesToRadians(collider.Parent.Rotation);
        MatrixExtentions.RotateFromLeft(ref matrix, radians);
        MatrixExtentions.TranslateFromLeft(ref matrix, -(colliderRect.Size * ComputeAnchorPosition(collider.Parent.Origin)));

        renderer.DrawRectangle(colliderRect, matrix, new Boundary(4), new DrawColorInfo(new Color(20, 255, 20)), false);
      }
		}
	}
}
