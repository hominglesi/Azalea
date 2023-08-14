using System;

namespace Azalea.Lists;

public interface INotifyArrayChanged
{
    event Action ArrayElementsChanged;
}
