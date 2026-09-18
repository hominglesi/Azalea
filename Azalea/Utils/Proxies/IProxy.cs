using System;
using System.Collections.Generic;
using System.Text;

namespace Azalea.Utils.Proxies;

public interface IProxy<T>
{
	public bool HasNewValue(out T value);
	public void SetValue(T value);
}
