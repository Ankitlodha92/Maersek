using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(1000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;
            job.Output = output;
            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);
        }


        public async Task<SortJob> GenerateId(int[] values)
        {
            await Task.Delay(0000);
            return new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Pending,
                duration: null,
                input: values,
                output: null);


        }

        public async Task<SortJob> GetJobDetails(Guid jobId)
        {


            await Task.Delay(0);
            string filePath = @"jobs.txt";
            var jsonData = "";
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    jsonData = sr.ReadToEnd();
                }
            }
            // De-serialize to object or create new list
            var jobList = JsonConvert.DeserializeObject<List<SortJob>>(jsonData)
                      ?? new List<SortJob>();
            return jobList.Find(a => a.Id == jobId);
        }
    }
}
