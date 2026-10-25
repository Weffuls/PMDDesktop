
using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Results;

/// <summary>
/// <para>Very generic "result-pattern" class.</para>
/// <para>Use this for any function that may error but where try-catch can be avoided.</para>
/// <para>Use <see cref="TryResult"/> to get the result/error in a "try-pattern."</para>
/// </summary>
/// <typeparam name="TResult"></typeparam>
/// <typeparam name="TError"></typeparam>
public class Result<TResult, TError> where TError : ResultError
{

	private TResult? result;
	private TError? error;

	public Result(TError error)
	{
		this.error = error;
	}

	public Result(TResult result)
	{
		this.result = result;
	}

	public bool HasResult() => error is not null;
	public bool HasError() => result is not null;

	/// <summary>
	///
	/// </summary>
	/// <param name="result"></param>
	/// <param name="error"></param>
	/// <returns></returns>
	public bool TryResult([NotNullWhen(true)] out TResult? result, [NotNullWhen(false)] out TError? error)
	{

		result = this.result;
		error = this.error;

		return result is not null;

	}

}
