namespace Azalea.Inputs;

public abstract class ButtonEventManager<TButton>
{
    public readonly TButton Button;

    public ButtonEventManager(TButton button)
    {
        Button = button;
    }
}
