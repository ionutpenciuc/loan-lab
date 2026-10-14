using BenchmarkDotNet.Attributes;
using DoseLab.Application;

namespace DoseLab.Benchmarks;

// Lesson 13 sample: how does the cost of one schedule grow with the number of weeks?
[MemoryDiagnoser]
public class TaperScheduleCalculatorBenchmarks
{
    [Params(1, 12, 52)]
    public int WeekCount { get; set; }

    [Benchmark]
    public TaperSchedule Calculate() => TaperScheduleCalculator.Calculate(40m, WeekCount);
}
