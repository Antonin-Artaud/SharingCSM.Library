using BenchmarkDotNet.Running;

namespace SharingCsm.Library.Benchmarks
{
	internal static class Program
	{
		static void Main(string[] args)
		{
			BenchmarkRunner.Run(typeof(Program).Assembly);
		}
	}
}
