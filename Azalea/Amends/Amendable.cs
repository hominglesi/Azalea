using Azalea.Platform;
using System.Collections.Generic;

namespace Azalea.Amends;
public abstract class Amendable
{
	private List<IAmend> _amends = new();

	public int AmendCount => _amends.Count;

	public void UpdateAmends()
	{
		var deltaTime = Time.DeltaTime;
		var soloBreak = true;

		for (int i = 0; i < _amends.Count; i++)
		{
			var amend = _amends[i];

			if (amend is IBreakingAmend)
			{
				if (soloBreak)
				{
					_amends.RemoveAt(i);
					i--;
				}
				else
				{
					break;
				}
			}
			else
			{
				if (amend is not IRepeatableAmend) soloBreak = false;
			}

			if (amend.HasStarted == false) amend.Start();

			amend.Update(deltaTime);

			if (amend.IsFinished)
			{
				_amends.RemoveAt(i);
				i--;
			}
		}
	}

	public void AddAmend(IAmend amend)
	{
		_amends.Add(amend);
	}

	public void FinishAmends()
	{
		foreach (var amend in _amends)
		{
			if (amend.HasStarted == false) amend.Start();

			amend.Finish();
		}

		_amends.Clear();
	}
}
