using Azalea.Design.Components;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Numerics;
using Azalea.Physics.Colliders;
using System.Numerics;

namespace Azalea.Debugging;
public class ColliderDebug : GameObject
{
	public bool IsShown { get; set; }
	private bool _isToggled = false;
	protected override DrawNode CreateDrawNode() => new ColliderDebugDrawNode(this);

	protected override void Update()
	{
		if (Input.GetKey(Keys.W).Down && Input.GetKey(Keys.ControlLeft).Pressed && Input.GetKey(Keys.ShiftLeft).Pressed)
			_isToggled = !_isToggled;

		IsShown = _isToggled || (Input.GetKey(Keys.W).Pressed && Input.GetKey(Keys.ControlLeft).Pressed);

	}

	private class ColliderDebugDrawNode : DrawNode
	{
		protected new ColliderDebug Source => (ColliderDebug)base.Source;
		public bool IsShown { get; set; }

		public ColliderDebugDrawNode(IGameObject source)
			: base(source) { }

		public override void ApplyState()
		{
			base.ApplyState();

			IsShown = Source.IsShown;
		}

		public override void Draw(IRenderer renderer)
		{
			base.Draw(renderer);

			if (IsShown)
			{
				foreach (var collider in ComponentStorage<RectCollider>.GetComponents())
				{
					var colliderRect = new Rectangle(Vector2.Zero, new(collider.SideA, collider.SideB));
					renderer.DrawRectangle(colliderRect, collider.Parent.DrawInfo.Matrix, new Boundary(4), new DrawColorInfo(new Color(20, 255, 20)), false);
				}
			}
		}
	}
}
