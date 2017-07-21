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
            int BufferSize = int.MaxValue;

            var buffer1 = new BlockingCollection<string>(BufferSize);
            var buffer2 = new BlockingCollection<string>(BufferSize);
            var buffer3 = new BlockingCollection<string>(BufferSize);

            var taskFactory=new TaskFactory(TaskCreationOptions.LongRunning,TaskContinuationOptions.None);

            var stage1 = taskFactory.StartNew(() => ReadLinesFromFile(buffer1));
            var stage2 = taskFactory.StartNew(() => BreakIntoWords(buffer1,buffer2));
            var stage3 = taskFactory.StartNew(() => CorrectCase(buffer2, buffer3));
            var stage4 = taskFactory.StartNew(() => PrintWords(buffer3));

            Task.WaitAll(stage4, stage2,stage3,stage1);

            Console.WriteLine("Done !!!");
            Console.ReadLine();
        }

        private static void PrintWords(BlockingCollection<string> input)
        {
            long count = 0;
            try
            {
               
               foreach (var item in input.GetConsumingEnumerable())
                {
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

        private static void CorrectCase(BlockingCollection<string> input, BlockingCollection<string> output)
        {
            try
            {
               foreach (var item in input.GetConsumingEnumerable())
                {
                    output.Add(item.ToUpper());
                }
            }
            finally
            {
               output.CompleteAdding();
            }
        }

        static void BreakIntoWords(BlockingCollection<string> input, BlockingCollection<string> output)
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

        static void ReadLinesFromFile(BlockingCollection<string> output)
        {
            try
            {
                foreach (var phrase in ReadAllLines())
                {
                    output.Add(phrase);
                  //  Thread.Sleep(10 * 1000);
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
