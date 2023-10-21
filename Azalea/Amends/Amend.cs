using System;

namespace Azalea.Amends;
public abstract class Amend<T> : IAmend
{
	protected T Target;
	protected Action<T>? Action;

	public bool IsFinished { get; protected set; }
	public bool HasStarted { get; protected set; }

	public Amend(T target, Action<T>? action)
	{
		Target = target;
		Action = action;
	}

	public virtual void Start()
	{
		HasStarted = true;
	}

	public virtual void Perform()
	{
		Action?.Invoke(Target);
	}

	public virtual void Finish()
	{
		IsFinished = true;
	}

	public abstract void Update(float deltaTime);
}
