using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ParallelHTTPRequest
{
    class Program
    {
        public static string[] urls = { "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
        "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com" ,
        "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
        "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com",
            "https://www.google.com", "https://www.microsoft.com", "https://www.visualstudio.com", "https://www.amazon.com", "https://www.youtube.com", "https://www.fb.com", "https://www.twitter.com"
        };

        static void Main(string[] args)
        {

            // use the command to check established connections
            // netstat -an | findstr 172 | findstr TCP | findstr EST

            //serial calls opens only 2 connections for google.com but is very slow
            Benchmark(
                () => SerialHTTPCallsWithWebRequest()
                , "SerialHTTPCallsWithWebRequest"
            );

            // HTTPClient closes the socket immediately but is slower than WebRequest
           
            Benchmark(
                () => ParallelHTTPCallsWithSingleHTTPClient()
                , "ParallelHTTPCallsWithSingleHTTPClient"
            );
            

            
            Benchmark(
                () => ParallelHTTPCallsWithMultiHTTPClient()
                , "ParallelHTTPCallsWithMultiHTTPClient"
            );
            

            //WebClient does not support concurrent I/O operations. so this does not work
            /*
            Benchmark(
                () => ParallelHTTPCallsWithSingleWebClient()
                , "ParallelHTTPCallsWithSingleWebClient"
            );
            */

             
           Benchmark(
               () => ParallelHTTPCallsWithMultiWebClient()
               , "ParallelHTTPCallsWithMultiWebClient"
           );
           

            // Webrequest and WebClient is faster but keeps the http socket open until the process is running
             
            Benchmark(
                () => ParallelHTTPCallsWithWebRequest()
                , "ParallelHTTPCallsWithWebRequest"
            );
               

            Console.ReadLine();
        }

        private static void SerialHTTPCallsWithWebRequest()
        {
            foreach(var url in urls )
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                //below line closes the socket after response is read ,
                //but makes it a bit slower but still faster than HTTPClient
                //myRequest.KeepAlive = false;
                //below line is for response timeout
                //myRequest.Timeout = 5000;
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();

                Console.WriteLine($" First 10 characters of '{url}' are \n {result.Substring(0, 10)}");
            } 
        }

        private static void ParallelHTTPCallsWithMultiWebClient()
        {
            Parallel.ForEach(urls, url =>
            {
                WebClient client = new WebClient();
                string reply = client.DownloadString(url);
                


                Console.WriteLine($" First 10 characters of '{url}' are \n {reply.Substring(0, 10)}");
                client.Dispose();
                reply = null;

                //}
            });
        }

        private static void ParallelHTTPCallsWithSingleWebClient()
        {
            using(WebClient client = new WebClient())
            {
                Parallel.ForEach(urls, url =>
                {

                    string reply = client.DownloadString(url);



                    Console.WriteLine($" First 10 characters of '{url}' are \n {reply.Substring(0, 10)}");



                    //}
                });
            }
           
        }

        private static void ParallelHTTPCallsWithWebRequest()
        {
            Parallel.ForEach(urls, url => {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "GET";
                //below line closes the socket after response is read ,
                //but makes it a bit slower but still faster than HTTPClient
                //myRequest.KeepAlive = false;
                //below line is for response timeout
                //myRequest.Timeout = 5000;
                WebResponse myResponse = myRequest.GetResponse();
                StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                string result = sr.ReadToEnd();
                sr.Close();
                myResponse.Close();
                 
                Console.WriteLine($" First 10 characters of '{url}' are \n {result.Substring(0, 10)}");
            });
        }

        private static void ParallelHTTPCallsWithSingleHTTPClient()
        {
            /*
            HttpClient Client = new HttpClient();
            var tasks = urls.Select(url => GetContent(Client, url) );
            Task.WhenAll(tasks).Wait();
            */


            using (var client = new HttpClient())
            {
                //below line does not help
                //client.DefaultRequestHeaders.Connection.Add("Keep-Alive");
                Parallel.ForEach(urls, url => {

                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;

                        Console.WriteLine($" First 10 characters of '{url}' are \n {responseString.Substring(0, 10)}");
                    }
                    
                });
                
            }
            
             
            Console.WriteLine("Async ");
 
        }

        private static void ParallelHTTPCallsWithMultiHTTPClient()
        {
            /*
            HttpClient Client = new HttpClient();
            var tasks = urls.Select(url => GetContent(Client, url) );
            Task.WhenAll(tasks).Wait();
            */



            Parallel.ForEach(urls, url =>
            {
                var client = new HttpClient();

                //using (var client = new HttpClient())
                //{
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = response.Content;

                        // by calling .Result you are synchronously reading the result
                        string responseString = responseContent.ReadAsStringAsync().Result;

                        Console.WriteLine($" First 10 characters of '{url}' are \n {responseString.Substring(0, 10)}");
                    }
                //}
            });




            Console.WriteLine("Async ");

        }

        public static async Task<string> GetContent(HttpClient Client, string url)
        {
            var response = await Client.GetStringAsync(url).ConfigureAwait(false);
            Console.WriteLine($" First 10 characters of '{url}' are \n {response.Substring(0, 10)}");
            return response;
        }

        public static void Benchmark(Action action, string methodname)
        {
            Console.WriteLine("Running method = {0}",
                            methodname);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            action();
            stopwatch.Stop();
            TimeSpan ts = stopwatch.Elapsed;
            Console.WriteLine("Elapsed Time is {0:00}h:{1:00}m:{2:00}s.{3}ms",
                            ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds);
        }
    }
}
