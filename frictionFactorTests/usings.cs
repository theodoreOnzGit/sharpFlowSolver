global using Xunit;
global using Xunit.Abstractions;
using System;
namespace frictionFactorTests;



public abstract class testOutputHelper
{
	private ITestOutputHelper _output { get; set; }

	public testOutputHelper(ITestOutputHelper outputHelper){
		this._output = outputHelper;
	}

	public void cout(String text){
		Console.WriteLine(text);
		this._output.WriteLine(text);
	}

}
