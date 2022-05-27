using System;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using EnumFastToStringGenerated;
using Perfolizer.Horology;

namespace Console.Test.Benchmark;


[EnumGenerator]
public enum UserType
{
    //[Display(Name = "مرد")]
    Men,

    //[Display(Name = "زن")]
    Women,

    //[Display(Name = "نامشخص")]
    None
}

public class Program
{
    static void Main(string[] args)
    {
        Regex.CacheSize += 100;
        var config = ManualConfig.Create(DefaultConfig.Instance)
            .AddAnalyser(BenchmarkDotNet.Analysers.EnvironmentAnalyser.Default)
            .AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub)
            .AddDiagnoser(BenchmarkDotNet.Diagnosers.MemoryDiagnoser.Default)
            //.AddColumn(StatisticColumn.AllStatistics)
            //.AddColumn(StatisticColumn.Median)
            //.AddColumn(StatisticColumn.StdDev)
            //.AddColumn(StatisticColumn.StdErr)
            .AddColumn(StatisticColumn.OperationsPerSecond)
            .AddColumn(BaselineRatioColumn.RatioMean)
            .AddColumn(RankColumn.Arabic)
            .AddJob(Job.Default.WithRuntime(CoreRuntime.Core60)
                .WithIterationCount(32)
                .WithInvocationCount(64)
                .WithIterationTime(TimeInterval.FromSeconds(120))
                .WithWarmupCount(6)
                .WithLaunchCount(1));

        BenchmarkRunner.Run<EnumBenchmark>(config);
    }
}

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class EnumBenchmark
{
    [Benchmark]
    public string NativeToString()
    {
        UserType state = UserType.Men;
        return state.ToString();
    }

    [Benchmark]
    public string FasterToString()
    {
        UserType state = UserType.Men;
        return state.StringToFast();
    }

    [Benchmark]
    public bool NativeIsDefined()
    {
        return Enum.IsDefined(typeof(UserType), UserType.Men);
    }

    [Benchmark]
    public bool FastIsDefined()
    {
        return UserTypeEnumExtensions.IsDefined(UserType.Men);
    }

}
