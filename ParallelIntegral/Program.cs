using System;
using System.Diagnostics;
using System.Threading;

namespace ParallelIntegral
{
    public static class Program
    {
        public static void Main(string[] args)
        {

            var ParalIntegral = new ParalIntegral(
                y: x => 1 / (Math.Log10(x)+ Math.Sqrt(x)+x),
                a: 2,
                b: 7,
                h: 0.0000001
            );
            var time = Stopwatch.StartNew();
            var Result = ParalIntegral.solo();
            Console.WriteLine($"Один поток: {time.ElapsedMilliseconds} ms");
            Console.WriteLine($"Результат: {Result}");
            time = Stopwatch.StartNew();
            ParalIntegral.Parallel();
            while (!ParalIntegral.Finish)
            {
            }
            Console.WriteLine($"Многопоточность: {time.ElapsedMilliseconds} ms");
            Console.WriteLine($"Результат: {ParalIntegral.Result}");
            Console.ReadKey();
        }
    }
}