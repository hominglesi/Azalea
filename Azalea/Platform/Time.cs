namespace Azalea.Platform;

public static class Time
{
	internal static float _deltaTime;

	public static float DeltaTime => _deltaTime;
	public static float DeltaTimeMs => _deltaTime * 1000;
}
