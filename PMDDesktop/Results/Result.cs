
using System.Diagnostics.CodeAnalysis;

namespace PMDDesktop.Results;

/// <summary>
/// <para>Very generic "result-pattern" class.</para>
/// <para>Use this for any function that may error but where try-catch can be avoided.</para>
/// <para>Use <see cref="TryResult"/> to get the result/error in a "try-pattern."</para>
/// </summary>
/// <typeparam name="TSuccess"></typeparam>
/// <typeparam name="TError"></typeparam>
public class Result<TSuccess, TError> where TError : ResultError
{

	/// <summary>
	/// The output on success, if it exists.
	/// </summary>
	public TSuccess? Success { get; private init; }

	/// <summary>
	/// The output on error, if it exists.
	/// </summary>
	public TError? Error { get; private init; }

	/// <summary>
	/// Create a new <see cref="Result{,}"/> returning <paramref name="error"/> as an error.
	/// </summary>
	/// <param name="error">The <see cref="TError"/> to hold.</param>
	public Result(TError error)
	{
		Error = error;
	}

	/// <summary>
	/// Create a new <see cref="Result{,}"/> returning <paramref name="result"/> as success.
	/// </summary>
	/// <param name="result">The <see cref="TSuccess"/> to hold.</param>
	public Result(TSuccess result)
	{
		Success = result;
	}

	/// <summary>
	/// Does this <see cref="Result{,}"/> contain a success?
	/// </summary>
	/// <returns>True if success is not null.</returns>
	public bool HasSuccess() => Error is not null;

	/// <summary>
	/// Does this <see cref="Result{,}"/> contain an error?
	/// </summary>
	/// <returns>True if error is not null.</returns>
	public bool HasError() => Success is not null;

	/// <summary>
	/// <para>Get the result or the error using a "try-pattern"-like function.</para>
	/// <para>This doesn't actually execute the function that creates the result, it only gives you the existing result in a convenient form.</para>
	/// </summary>
	/// <param name="success">The existing success, not null if true.</param>
	/// <param name="error">The existing error, not null if false.</param>
	/// <returns>True if Success; false if Error.</returns>
	public bool TryResult([NotNullWhen(true)] out TSuccess? success, [NotNullWhen(false)] out TError? error)
	{

		success = Success;
		error = Error;

		return success is not null;

	}

}
