using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea;

public struct HostPreferences
{
    public HostType Type = HostType.XNA;

    public HostPreferences() { }
}

public enum HostType
{
    XNA
}