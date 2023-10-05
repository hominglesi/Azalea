using Azalea.Design.Shapes;
using System.Timers;

namespace Azalea.Debugging;
public class DebugRectHighlight : HollowBox
{
	private readonly Timer _deathTimer;
	private bool _finished;

	public DebugRectHighlight()
	{
		_deathTimer = new Timer()
		{
			Enabled = true,
			Interval = 750,
		};
		_deathTimer.Elapsed += (_, _) => _finished = true;
	}

	protected override void Update()
	{
		if (_finished)
		{
			Parent?.RemoveInternal(this);
		}
	}
}
