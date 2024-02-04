using Azalea.Graphics;
using Azalea.Graphics.Colors;
using System;
using System.Numerics;

namespace Azalea.Amends;
public static class AmendableExtentions
{
	public static T Execute<T>(this T amendable, Action<T> action)
		where T : Amendable
	{
		amendable.AddAmend(new InstantAmend<T>(amendable, action));
		return amendable;
	}

	public static T ExecuteFor<T>(this T amendable, Action<T> action, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new TimedAmend<T>(amendable, action, duration));
		return amendable;
	}

	public static T Then<T>(this T amendable)
		where T : Amendable
	{
		amendable.AddAmend(new BreakAmend<T>(amendable));
		return amendable;
	}

	public static T Wait<T>(this T amendable, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new TimedAmend<T>(amendable, null, duration));
		return amendable;
	}

	public static T Loop<T>(this T amendable, Action<T> action, float interval)
		where T : Amendable
	{
		amendable.AddAmend(new RepeatAmend<T>(amendable, action, interval));
		return amendable;
	}

	public static T ChangeFloatPropertyTo<T>(this T amendable, string propertyName, float newValue, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new FloatAmend<T>(amendable, propertyName, newValue, duration, relative: false));
		return amendable;
	}

	public static T ChangeFloatPropertyBy<T>(this T amendable, string propertyName, float change, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new FloatAmend<T>(amendable, propertyName, change, duration, relative: true));
		return amendable;
	}

	public static T ChangeVector2PropertyTo<T>(this T amendable, string propertyName, Vector2 newValue, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new Vector2Amend<T>(amendable, propertyName, newValue, duration, relative: false));
		return amendable;
	}

	public static T ChangeVector2PropertyBy<T>(this T amendable, string propertyName, Vector2 change, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new Vector2Amend<T>(amendable, propertyName, change, duration, relative: true));
		return amendable;
	}

	public static T ChangeBoundaryPropertyTo<T>(this T amendable, string propertyName, Boundary newValue, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new BoundaryAmend<T>(amendable, propertyName, newValue, duration, relative: false));
		return amendable;
	}

	public static T ChangeBoundaryPropertyBy<T>(this T amendable, string propertyName, Boundary change, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new BoundaryAmend<T>(amendable, propertyName, change, duration, relative: false));
		return amendable;
	}

	public static T ChangeColorQuadPropertyTo<T>(this T amendable, string propertyName, ColorQuad newValue, float duration)
		where T : Amendable
	{
		amendable.AddAmend(new ColorAmend<T>(amendable, propertyName, newValue, duration, relative: false));
		return amendable;
	}

	#region Rotation

	public static T RotateBy<T>(this T amendable, float change, float duration)
		where T : Amendable
		=> amendable.ChangeFloatPropertyBy("Rotation", change, duration);

	public static T RotateTo<T>(this T amendable, float newValue, float duration)
		where T : Amendable
		=> amendable.ChangeFloatPropertyTo("Rotation", newValue, duration);

	#endregion
	#region Position

	public static T RepositionBy<T>(this T amendable, Vector2 positionChange, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyBy("Position", positionChange, duration);

	public static T RepositionTo<T>(this T amendable, Vector2 newPosition, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyTo("Position", newPosition, duration);

	#endregion
	#region Size

	public static T ResizeBy<T>(this T amendable, Vector2 sizeChange, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyBy("Size", sizeChange, duration);

	public static T ResizeTo<T>(this T amendable, Vector2 newSize, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyTo("Size", newSize, duration);

	#endregion
	#region Scale

	public static T ScaleBy<T>(this T amendable, Vector2 scaleChange, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyBy("Scale", scaleChange, duration);

	public static T ScaleTo<T>(this T amendable, Vector2 newScale, float duration)
		where T : Amendable
		=> amendable.ChangeVector2PropertyTo("Scale", newScale, duration);

	#endregion
	#region Color

	public static T RecolorTo<T>(this T amendable, Color newColor, float duration)
		where T : Amendable
		=> amendable.ChangeColorQuadPropertyTo("Color", newColor, duration);

	#endregion

	#region Alpha
	public static T ChangeAlphaBy<T>(this T amendable, float alphaChange, float duration)
		where T : Amendable
		=> amendable.ChangeFloatPropertyBy("Alpha", alphaChange, duration);

	public static T ChangeAlphaTo<T>(this T amendable, float newAlpha, float duration)
		where T : Amendable
		=> amendable.ChangeFloatPropertyTo("Alpha", newAlpha, duration);

	#endregion
}
