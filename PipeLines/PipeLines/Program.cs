using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PipeLines
{
    class Program
    {
        static void Main(string[] args)
        {
        // TaskCancelation();
           

           PipeLineFun();
        }

        private static void TaskCancelation()
        {
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken ct = tokenSource2.Token;

            var task = Task.Factory.StartNew(() =>
            {
                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                bool moreToDo = true;
                while (moreToDo)
                {
                    // Poll on this property if you have to do
                    // other cleanup before throwing.
                    if (ct.IsCancellationRequested)
                    {
                        // Clean up here, then...
                        ct.ThrowIfCancellationRequested();
                    }
                }
            }, tokenSource2.Token); // Pass same token to StartNew.

            Thread.Sleep(1 * 1000);
            tokenSource2.Cancel();

            // Just continue on this thread, or Wait/WaitAll with try-catch:
            try
            {
                task.Wait();
            }
            catch (AggregateException e)
            {
                foreach (var v in e.InnerExceptions)
                    Console.WriteLine(e.Message + " " + v.Message);
            }
            finally
            {
                tokenSource2.Dispose();
            }

            Console.ReadKey();
        }

        private static void PipeLineFun()
        {
            int BufferSize = int.MaxValue;

            var buffer1 = new BlockingCollection<string>(BufferSize);
            var buffer2 = new BlockingCollection<string>(BufferSize);
            var buffer3 = new BlockingCollection<string>(BufferSize);

            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.None);
            var tokenSource2 = new CancellationTokenSource();
            CancellationToken cancellationToken = tokenSource2.Token;

            var stage1 = taskFactory.StartNew(() => ReadLinesFromFile(buffer1, cancellationToken));
            var stage2 = taskFactory.StartNew(() => BreakIntoWords(buffer1, buffer2, cancellationToken));
            var stage3 = taskFactory.StartNew(() => CorrectCase(buffer2, buffer3, tokenSource2.Token));
            var stage4 = taskFactory.StartNew(() => PrintWords(buffer3, cancellationToken));

            Thread.Sleep(1 * 1000);
            tokenSource2.Cancel();
            Task.WaitAll(stage4, stage2, stage3, stage1);

            Console.WriteLine("Done !!!");
            Console.ReadLine();
        }

        private static void PrintWords(BlockingCollection<string> input, CancellationToken cancellationToken)
        {
            long count = 0;
            try
            {
               
               foreach (var item in input.GetConsumingEnumerable())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    count++;
                    Console.WriteLine(item);
                }
               
            }
           finally
            {
                //input.CompleteAdding();
            }
            Console.WriteLine("Words count {0}",count);
        }

        private static void CorrectCase(BlockingCollection<string> input, BlockingCollection<string> output, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var item in input.GetConsumingEnumerable())
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    output.Add(item.ToUpper(), cancellationToken);
                }
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine("Cancelled");
            }
            finally
            {
               output.CompleteAdding();
            }
        }

        static void BreakIntoWords(BlockingCollection<string> input, BlockingCollection<string> output, CancellationToken cancellationToken)
        {
            try
            {
               foreach (var item in input.GetConsumingEnumerable())
                {
                    var result = item;
                    var wrods = result.Split(' ');
                    foreach (var wrod in wrods)
                    {
                        output.Add(wrod);

                    }
                   
                }
            }
               
            finally
            {
                output.CompleteAdding();
            }
        }

        static void ReadLinesFromFile(BlockingCollection<string> output, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var phrase in ReadAllLines())
                {
                    output.Add(phrase, cancellationToken);
                 
                }
            }
            
            finally
            {
                output.CompleteAdding();
            }
        }

        private static IEnumerable<string> ReadAllLines()
        {
           List<string> lines = new List<string>();
            
            using (
                FileStream fileStream = new FileStream(@"C:\ProgramData\SCIEX\Logs\SciexOS\MassSpecDriver\MassSpecDriverService1.log", FileMode.Open))
            {
                using (TextReader reader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                       lines.Add(line);
                    }
                }
            }
            return lines;
        }
    }
}
