using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SortController : ControllerBase
    {
        private readonly ISortJobProcessor _sortJobProcessor;

        public SortController(ISortJobProcessor sortJobProcessor)
        {
            _sortJobProcessor = sortJobProcessor;
        }

        [HttpPost]
        public async Task<ActionResult<SortJob>> EnqueueJob(int[] values)
        {
            // TODO: Should enqueue a job to be processed in the background.
            // throw new NotImplementedException();
            var jsonData = "";

            string filePath = @"jobs.txt";
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    jsonData = sr.ReadToEnd();
                    sr.Dispose();
                }
                fs.Dispose();
            }
            // De-serialize to object or create new list
            var jobList = JsonConvert.DeserializeObject<List<SortJob>>(jsonData)
                      ?? new List<SortJob>();


            var pendingJobs = await _sortJobProcessor.GenerateId(values);
            jobList.Add(pendingJobs);
            //var completedJob = await _sortJobProcessor.Process(pendingJob);
            string jsonResponse = JsonConvert.SerializeObject(jobList);

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                using (var sw = new StreamWriter(fs))
                {
                    sw.WriteLine(jsonResponse);
                    sw.Dispose();
                }
                fs.Dispose();
            }
           // System.IO.File.WriteAllText(@"jobs.txt", jsonResponse);
            return Ok(pendingJobs);
        }

        //[HttpPost]

        //public async Task<ActionResult<SortJob>> EnqueueAndRunJob(int[] values)
        //{
        //    //string JsonString = JsonConvert.SerializeObject(inputno);

        //    //List<int> values = JsonConvert.DeserializeObject<List<int>>(JsonString);

        //    var pendingJob = new SortJob(
        //        id: Guid.NewGuid(),
        //        status: SortJobStatus.Pending,
        //        duration: null,
        //        input: values,
        //        output: null);

        //    var completedJob = await _sortJobProcessor.Process(pendingJob);

        //    return Ok(pendingJob);
        //}



        [HttpGet]
        public List<SortJob> GetJobs()
        {
            // TODO: Should return all jobs that have been enqueued (both pending and completed).
            var jsonData = "";
            string filePath = @"jobs.txt";
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

            return jobList;


        }

        [HttpGet("{jobId}")]
        public async Task<ActionResult<SortJob>> GetJob(Guid jobId)
        {

            return await _sortJobProcessor.GetJobDetails(jobId);

        }
    }
}
