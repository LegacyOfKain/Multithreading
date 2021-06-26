//https://www.packtpub.com/product/mastering-c-concurrency/9781785286650
// Mastering C# Concurrency
//Used example from Chapter 1. Traditional Concurrency

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace IntegerCounter
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("returnCounterWithoutSynchronization = " + 
                returnCounterWithoutSynchronization());
            Console.WriteLine("returnCounterWithLockSynchronization = " + 
                returnCounterWithLockSynchronization());
            Console.WriteLine("returnCounterWithIncrementInterlockedSynchronization = " +
                returnCounterWithIncrementInterlockedSynchronization());
            Console.WriteLine("returnCounterWithoutSynchronizationParallelForeach = " +
                returnCounterWithoutSynchronizationParallelForeach());
            Console.WriteLine("returnCounterWithoutSynchronizationParallelForeachMaxParallelism = " +
                returnCounterWithoutSynchronizationParallelForeachMaxParallelism());           
            Console.WriteLine("returnCounterWithSynchronizationParallelForeach = " +
                returnCounterWithSynchronizationParallelForeach());
            Console.WriteLine("returnCounterWithSynchronizationParallelForeachMaxParallelism = " +
                returnCounterWithSynchronizationParallelForeachMaxParallelism());
            Console.WriteLine("returnCounterWithoutSynchronizationTaskRun = " +
                returnCounterWithoutSynchronizationTaskRun());
            Console.WriteLine("returnCounterWithSynchronizationTaskRun = " +
                returnCounterWithSynchronizationTaskRun());
            



            Console.ReadLine();
        }

        private static int returnCounterWithSynchronizationTaskRun()
        {
            const int iterations = 10000;
            var counter = 0;
            var threads = Enumerable.Range(0, 8)
                                    .Select(n => Task.Run(() =>
                                    {
                                        for (int i = 0; i < iterations; i++)
                                        {
                                            Interlocked.Increment(ref counter);
                                            Thread.SpinWait(100);
                                            Interlocked.Decrement(ref counter);
                                        }
                                    }))
                                    .ToArray();
            foreach (var thread in threads)
            {
                //Use this method to ensure that a thread has been terminated , i.e wait for all threads to end
                thread.Wait();
            }

            return counter;
        }

        private static int returnCounterWithoutSynchronizationTaskRun()
        {
            const int iterations = 10000;
            var counter = 0;
            var threads = Enumerable.Range(0, 8)
                                    .Select(n => Task.Run(() =>
                                    {
                                        for (int i = 0; i < iterations; i++)
                                        {
                                            counter += 1;
                                            Thread.SpinWait(100);
                                            counter -= 1;
                                        }
                                    }))
                                    .ToArray();
            foreach (var thread in threads)
            {
                //Use this method to ensure that a thread has been terminated , i.e wait for all threads to end
                thread.Wait();
            }

            return counter;
        }

        private static int returnCounterWithSynchronizationParallelForeachMaxParallelism()
        {
            const int iterations = 10000;
            var counter = 0;

            var numOfConcurrency = Enumerable.Range(0, 8).ToArray();
            Parallel.ForEach(numOfConcurrency,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                number =>
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        Interlocked.Increment(ref counter);
                        Thread.SpinWait(100);
                        Interlocked.Decrement(ref counter);
                    }

                });

            return counter;
        }

        private static int returnCounterWithoutSynchronizationParallelForeachMaxParallelism()
        {
            const int iterations = 10000;
            var counter = 0;

            var numOfConcurrency = Enumerable.Range(0, 8).ToArray();
            Parallel.ForEach(numOfConcurrency,
                new ParallelOptions { MaxDegreeOfParallelism = 4 },
                number =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    counter += 1;
                    Thread.SpinWait(100);
                    counter -= 1;
                }

            });

            return counter;
        }

        private static int returnCounterWithoutSynchronizationParallelForeach()
        {
            const int iterations = 10000;
            var counter = 0;
            
            var numOfConcurrency = Enumerable.Range(0, 8).ToArray();
            Parallel.ForEach(numOfConcurrency, number =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    counter += 1;
                    Thread.SpinWait(100);
                    counter -= 1;
                }
                
            });

            return counter;
        }

        private static int returnCounterWithSynchronizationParallelForeach()
        {
            const int iterations = 10000;
            var counter = 0;

            var numOfConcurrency = Enumerable.Range(0, 8).ToArray();
            Parallel.ForEach(numOfConcurrency, number =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    Interlocked.Increment(ref counter);
                    Thread.SpinWait(100);
                    Interlocked.Decrement(ref counter);
                }

            });

            return counter;
        }

        private static int returnCounterWithIncrementInterlockedSynchronization()
        {
            const int iterations = 10000;
            var counter = 0;
            //ThreadStart is a delegate that represents the method that executes on a Thread.
            ThreadStart proc = () =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    Interlocked.Increment(ref counter);
                    Thread.SpinWait(100);
                    Interlocked.Decrement(ref counter);
                }
            };
            var threads = Enumerable.Range(0, 8)
                                    .Select(n => new Thread(proc))
                                    .ToArray();
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
            {
                //Use this method to ensure that a thread has been terminated , i.e wait for all threads to end
                thread.Join();
            }

            return counter;
        }

        private static int returnCounterWithLockSynchronization()
        {
            const int iterations = 10000;
            var obj = new Object();
            var counter = 0;
            //ThreadStart is a delegate that represents the method that executes on a Thread.
            ThreadStart proc = () =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    lock (obj)
                    {
                        counter += 1;
                        Thread.SpinWait(100);
                        counter -= 1;
                    }

                }
            };
            var threads = Enumerable.Range(0, 8)
                                    .Select(n => new Thread(proc))
                                    .ToArray();
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
            {
                //Use this method to ensure that a thread has been terminated , i.e wait for all threads to end
                thread.Join();
            }

            return counter;
        }

        private static int returnCounterWithoutSynchronization()
        {
            const int iterations = 10000;
            var counter = 0;
            //ThreadStart is a delegate that represents the method that executes on a Thread.
            ThreadStart proc = () =>
            {
                for (int i = 0; i < iterations; i++)
                {
                    counter += 1;
                    Thread.SpinWait(100);
                    counter -= 1;
                }
            };
            var threads = Enumerable.Range(0, 8)
                                    .Select(n => new Thread(proc))
                                    .ToArray();
            foreach (var thread in threads)
                thread.Start();
            foreach (var thread in threads)
            {
                //Use this method to ensure that a thread has been terminated , i.e wait for all threads to end
                thread.Join();
            }

            return counter;
        }
    }
}
