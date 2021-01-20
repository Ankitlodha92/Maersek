using Maersk.Sorting.Api;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MaersekSorting
{
    public class Worker : IWorker,IDisposable
    {
        private readonly ISortJobProcessor _sortJobProcessor;

        public Worker(ISortJobProcessor sortJobProcessor,ILogger<Worker> logger)
        {
            _sortJobProcessor = sortJobProcessor;
            Logger = logger;
        }

        public ILogger<Worker> Logger { get; }

        public void Dispose()
        {
           
        }

        public async Task DoWork(CancellationToken cancellationToken)
        {
            try
            {
                string jsonResponse = "";
                string filePath = @"jobs.txt";

                while (!cancellationToken.IsCancellationRequested)
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {

                            var jsonData = sr.ReadToEnd();
                            // De-serialize to object or create new list
                            var jobList = JsonConvert.DeserializeObject<List<SortJob>>(jsonData)
                                      ?? new List<SortJob>();


                            foreach (SortJob sjob in jobList)
                            {
                                if (sjob.Status != SortJobStatus.Completed)
                                {
                                    await _sortJobProcessor.Process(sjob);
                                    sjob.Status = SortJobStatus.Completed;
                                }

                            }
                            jsonResponse = JsonConvert.SerializeObject(jobList);
                            sr.Close();
                            sr.Dispose();
                        }
                        fs.Close();
                        fs.Dispose();

                    }
                    using (FileStream fswrite = new FileStream(filePath, FileMode.Create))
                    {

                        using (var sw = new StreamWriter(fswrite))
                        {
                            var jobList = JsonConvert.DeserializeObject<List<SortJob>>(jsonResponse)
                                     ?? new List<SortJob>();
                            if (jobList.Count > 0)
                            {
                                sw.WriteLine(jsonResponse);
                            }
                            sw.Close();
                            sw.Dispose();
                        }

                        fswrite.Close();
                        fswrite.Dispose();
                    }

                    //System.IO.File.WriteAllText(@"jobs.txt", jsonResponse);
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex.Message);
            }


        }




    }
}

