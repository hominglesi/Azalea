using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azalea.Inputs;

public class ButtonState
{
    private bool _pressed;
    private bool _down;
    private bool _up;
    public bool Pressed => _pressed;
    public bool Released => !_pressed;
    public bool Down => _down;
    public bool Up => _up;

    internal void SetDown()
    {
        _pressed = true;
        _down = true;
    }

    internal void SetUp()
    {
        _pressed = false;
        _up = true;
    }

    internal void Update()
    {
        _up = false;
        _down = false;
    }
}
