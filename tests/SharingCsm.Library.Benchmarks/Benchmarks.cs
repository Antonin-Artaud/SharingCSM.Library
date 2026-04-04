using BenchmarkDotNet.Attributes;
using SharingCsm.Library.Infrastructure.Services.Catalogs;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SharingCsm.Library.Benchmarks;

[MemoryDiagnoser]
public class CatalogImportServiceBenchmark
{
	private CatalogImportService _importService = null!;
	private MemoryStream _memoryStream = null!;

	[Params(100, 10_000, 100_000, 1_000_000)]
	public int NumberOfLines { get; set; }

	[GlobalSetup]
	public void Setup()
	{
		_importService = new CatalogImportService();

		var sb = new StringBuilder();
		for (int i = 0; i < NumberOfLines; i++)
		{
			sb.AppendLine($"{Guid.NewGuid()};Livre Benchmark {i};Categorie Test");
		}

		var bytes = Encoding.UTF8.GetBytes(sb.ToString());

		_memoryStream = new MemoryStream(bytes);
	}

	[GlobalCleanup]
	public void Cleanup()
	{
		_memoryStream?.Dispose();
	}

	[Benchmark(Baseline = true)]
	public async Task<int> ImportClassic_WithSplit()
	{
		_memoryStream.Position = 0;

		int count = 0;

		await foreach (var dto in _importService.ImportBadAsync(_memoryStream, CancellationToken.None))
		{
			count++;
		}

		return count;
	}

	[Benchmark]
	public async Task<int> ImportFast_WithSpan()
	{
		_memoryStream.Position = 0;
		int count = 0;

		await foreach (var dto in _importService.ImportFastAsync(_memoryStream, CancellationToken.None))
		{
			count++;
		}

		return count;
	}
}
