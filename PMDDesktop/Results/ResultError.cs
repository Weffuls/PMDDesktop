namespace PMDDesktop.Results;

/// <summary>
/// <para>A <see cref="ResultError"/> describes why a <see cref="Result{,}"/> couldn't be completed.</para>
/// <para>You can use <see cref="Result.Result(TError)"/> to make a new <see cref="Result{,}"/> with a <see cref="Result.Error"/> instead of a <see cref="Result.Success"/>.</para>
/// </summary>
public abstract class ResultError
{

	/// <summary>
	/// <para>A string describing why this error happened.</para>
	/// <para>If a <see cref="ResultError"/> causes a user-facing issue, this string should be shown somewhere to explain what happened.</para>
	/// </summary>
	/// <returns></returns>
	public abstract string GetErrorReason();

}
