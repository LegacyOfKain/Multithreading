using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ParallelReadCSVfile
{
    class Program
    {
        static void Main(string[] args)
        {
            ProcessFile();
            Console.ReadLine();
        }

        private static readonly char[] Separators = { ',', ' ' };

        private static void ProcessFile()
        {
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
            

            var lines = File.ReadLines("5millionrows.csv");
            var numbers = ProcessRawNumbersParallel(lines);
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));

            /* This takes almost 1 minute 
            lines = File.ReadLines("5millionrows.csv");
            numbers = ProcessRawNumbersTaskRun(lines);
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
            */

            lines = File.ReadLines("5millionrows.csv");
            numbers = ProcessRawNumbers(lines);
            Console.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));

            var rowTotal = new List<double>();
            var totalElements = 0;

            foreach (var values in numbers)
            {
                var sumOfRow = values.Sum();
                rowTotal.Add(sumOfRow);
                totalElements += values.Count;
            }
            Console.WriteLine("totalElements = "+totalElements );
        }

        private static List<List<double>> ProcessRawNumbersParallel(IEnumerable<string> lines)
        {
            var numbers = new ConcurrentBag<List<double>>();
            /*System.Threading.Tasks.*/
            //ThreadPool.SetMinThreads(50, 50);
            Parallel.ForEach(lines,
                //new ParallelOptions { MaxDegreeOfParallelism = 64 },
                line =>
            {
                //lock (numbers)
                //{
                    numbers.Add(ProcessLine(line));
                //}
            });
            return numbers.ToList();
        }

        private static List<List<double>> ProcessRawNumbersTaskRun(IEnumerable<string> lines)
        {
            var numbers = new List<List<double>>();
            List<Task> TaskList = new List<Task>();
            foreach (var line in lines)
            {

                Task t = Task.Run(()=> 
                {
                    lock (numbers)
                    {
                        numbers.Add(ProcessLine(line));
                    }
                }  
                );
                TaskList.Add(t);

            }
            Task.WaitAll(TaskList.ToArray());
            return numbers;
        }

        private static List<List<double>> ProcessRawNumbers(IEnumerable<string> lines)
        {
            var numbers = new List<List<double>>();
            /*System.Threading.Tasks.*/
            foreach ( var line in lines)
            {
                 
                    numbers.Add(ProcessLine(line));
                 
            } 
            return numbers;
        }

        private static List<double> ProcessLine(string line)
        {
            var list = new List<double>();
            foreach (var s in line.Split(Separators, StringSplitOptions.RemoveEmptyEntries))
            {
                double i;
                if (Double.TryParse(s, out i))
                {
                    

                    list.Add(i);
                    // perform some complex actions
                    // If you comment the below line the task becomes easy
                    // And normal foreach is as fast as parallel for each
                    FindPrimeNumber(Int32.Parse(s));
                     
                    //Console.WriteLine("i = {0}" , i);

                }
            }
            return list;
        }

        // this is still slower that normal foreach if it runs inside a parallel foreach
        private static List<double> ProcessLineParallel(string line)
        {
            var list = new List<double>();
            var numbersInTheLine = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
            Thread[] threadsArray = new Thread[numbersInTheLine.Length];

            int counter = 0;
            foreach (var s in numbersInTheLine)
            {
                
                threadsArray[counter] = new Thread(() =>
                {
                    double i;
                    if (Double.TryParse(s, out i))
                    {
                        lock(list)
                        { 
                        list.Add(i);
                        }
                        //Console.WriteLine("i = {0}", i);

                    }
                }
                );              
                threadsArray[counter].Start();
                counter++;
            }
            counter = 0;
            foreach (var s in numbersInTheLine)
            {

                threadsArray[counter].Join();
                counter++;
            }

            return list;
        }

        public static long FindPrimeNumber(int n)
        {
            int count = 0;
            long a = 2;
            while (count < n)
            {
                long b = 2;
                int prime = 1;// to check if found a prime
                while (b * b <= a)
                {
                    if (a % b == 0)
                    {
                        prime = 0;
                        break;
                    }
                    b++;
                }
                if (prime > 0)
                {
                    count++;
                }
                a++;
            }
            return (--a);
        }
    }
}
