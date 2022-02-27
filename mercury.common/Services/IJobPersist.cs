using Mercury.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.Common.Services
{
    public interface IJobPersist
    {
        void SaveJob(IJob job);
        IJob? GetJob(Guid guid);
        IEnumerable<IJob> GetJobs();
    }
}
