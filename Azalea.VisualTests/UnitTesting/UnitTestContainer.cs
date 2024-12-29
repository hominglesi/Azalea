using Azalea.Design.Containers;

namespace Azalea.VisualTests.UnitTesting;
public class UnitTestContainer : Composition
{
	public void ForceUpdate() => UpdateSubTree();
}
