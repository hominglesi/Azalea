using Azalea.Design.Components;
using Azalea.Graphics;
using Azalea.Graphics.Colors;
using Azalea.Graphics.Primitives;
using Azalea.Graphics.Rendering;
using Azalea.Inputs;
using Azalea.Inputs.Events;
using Azalea.Numerics;
using Azalea.Platform.Rendering.Coordination;
using Azalea.Simulations.Colliders;
using System.Numerics;

namespace Azalea.Editing.Legacy;
public class LegacyColliderDebug : GameObject
{
	public bool IsShown { get; set; }
	private bool _isToggled = false;

	protected override bool OnKeyDown(KeyDownEvent e)
	{
		if(e.Key == Keys.W && e.State.ControlPressed && e.State.ShiftPressed)
		{
			if (e.State.ShiftPressed)
				_isToggled = !_isToggled;

			IsShown = true;
			return true;
		}

		return false;
	}

	protected override void OnKeyUp(KeyUpEvent e)
	{
		if (e.Key == Keys.W)
			IsShown = _isToggled;
	}

	public override void Draw(RenderCoordinator coordinator)
	{
		if (IsShown)
		{
			var color = new DrawColorInfo(new Color(45, 75, 23, 80));
			var color2 = new DrawColorInfo(new Color(84, 42, 86, 140));

			foreach (var collider in ComponentStorage<RectCollider>.GetComponents())
			{
				coordinator.BindTexture(Platform.Rendering.OpenGL.GLRenderer.LoadingContext!.WhitePixel);
				coordinator.BindProgram(coordinator.DefaultQuadProgram);
				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, collider.Quad, color.Color, Rectangle.One);

				var centerQuad = new Quad(collider.Position - new Vector2(5), new(10));
				coordinator.DefaultQuadBatch.Add(coordinator.CommandQueue, centerQuad, color2.Color, Rectangle.One);
			}
		}
	}
}
