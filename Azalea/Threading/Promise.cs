using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Azalea.Threading;
public class Promise<T> : IPromise
{
	private readonly Task<T>? _task;
	private readonly T? _wrappedResult;

	public Promise(Func<T> executedFunction)
	{
		_task = Task.Run(executedFunction);

		PromiseSystem.AddPromise(this);
	}

	/// <summary>
	/// If an API expects a Promise but you already have the result you can wrap the result
	/// in a promise without the actuall overhead of a real promise
	/// </summary>
	public Promise(T result)
	{
		_wrappedResult = result;
	}

	private List<Action<T>> _onResolvedActions = [];

	public Promise<T> Then(Action<T> onResolved)
	{
		if (_wrappedResult is not null)
		{
			onResolved(_wrappedResult);
			return this;
		}

		Debug.Assert(_task is not null);

		if (_task.IsCompleted)
			onResolved(_task.Result);
		else
			_onResolvedActions.Add(onResolved);

		return this;
	}

	internal void ExecuteOnResolved()
	{
		Debug.Assert(_task is not null);

		foreach (var action in _onResolvedActions)
			action.Invoke(_task.Result);

		_onResolvedActions.Clear();
		_isResolved = true;
	}

	void IPromise.ExecuteOnResolved() => ExecuteOnResolved();

	private bool _isResolved = false;
	public bool IsResolved => _task is null || _isResolved;

	bool IPromise.IsResolvedInternal => _task is null || _task.IsCompleted;
}

/* PromisedTexture(Promise<Texture>) texture = Assets.GetTextureAsync();
 * _sprite.Texture = texture;
 * 
 * _sprite.Draw() {
 *	if(texture is PromisedTexture promised){
 *		if(promised.Resolved == false) drawLoading
 *		else set resolved texture
 *	}
 * }
 * 
 * Assets.GetTextureAsync();
 * 
 * ResourceExtentions.GetTextureAsync(){
 *	new Thread(){
 *		get stream from store ("ALL STREAMS NEED TO BE ATOMIC")
 *		
 *		
 *	}
 * }
 * 
 */
