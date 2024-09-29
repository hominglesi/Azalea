using System;

namespace Azalea.List;

public interface INotifyArrayChanged
{
	event Action ArrayElementsChanged;
}
