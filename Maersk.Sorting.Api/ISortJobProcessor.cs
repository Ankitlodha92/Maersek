using System;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        Task<SortJob> GenerateId(int[] values);

        Task<SortJob> GetJobDetails(Guid jobId);

    }
}