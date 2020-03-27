using System;
using System.Diagnostics;

namespace LuxEngine
{
    public static class Benchmark
    {
        public static void Run(string name, int iterations, Action action)
        {
            try
            {
                Console.Write($"Running benchmark '{name}' for {iterations} iterations... ");

                // Perform garbage collection.
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // Force JIT compilation of the method.
                action.Invoke();

                // Run the benchmark.
                Stopwatch watch = Stopwatch.StartNew();
                for (int i = 0; i < iterations; i++)
                {
                    action.Invoke();
                }
                watch.Stop();

                // Output results.
                Console.WriteLine($"Elapsed time {watch.ElapsedMilliseconds / iterations} ms.");
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine($"Out of memory!");
            }
        }
    }
}