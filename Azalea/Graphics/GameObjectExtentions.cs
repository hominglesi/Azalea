namespace Azalea.Graphics;

public static class GameObjectExtentions
{
	public static T? FindClosestParent<T>(this GameObject? gameObject)
		where T : class, IGameObject
	{
		while ((gameObject = gameObject?.Parent) != null)
		{
			if (gameObject is T match)
				return match;
		}

		return null;
	}
}
