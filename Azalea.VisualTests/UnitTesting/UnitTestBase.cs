namespace Azalea.VisualTests.UnitTesting;
public class UnitTestBase
{
	private readonly string _parsedName;
	protected virtual string GetDisplayName() => "";
	public string DisplayName
		=> GetDisplayName() == "" ? _parsedName : GetDisplayName();

	public UnitTestBase()
	{
		_parsedName = VisualTestUtils.GetTestDisplayName(GetType());
	}
}
