namespace PMDDesktop.Server.Assets.Builder;

public readonly struct BuildStep
{

	internal BuildStep(string name, StepFunction function)
	{
		this.onExecute = function;
		this.name = name;
	}

	internal delegate Task StepFunction(AssetManager assets);

	internal readonly StepFunction onExecute;
	public readonly string name;

}
