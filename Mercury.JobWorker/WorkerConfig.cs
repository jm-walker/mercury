using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mercury.JobWorker
{
    public class WorkerConfig
    {
        public string[] Plugins { get; set; } = Array.Empty<string>();
    }
}
