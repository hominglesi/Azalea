using Azalea.Design.Containers.Text;

namespace Azalea.VisualTests;
public class TextContainerTest : TestScene
{
	private TextContainer _container;
	public TextContainerTest()
	{
		Add(_container = new TextContainer()
		{
			Width = 1000,
		});
		//_container.Text = "New Line\nI sada je New Line\nllkwanldkadw\n";
		_container.AddText("New Line");
		_container.NewLine();
		_container.AddText("I sada je New Line");
		_container.NewLine();
		_container.AddText("llkwanldkadw");
		_container.NewLine();
		_container.AddText("awdioanoudwda");
	}
}
