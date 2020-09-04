using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static ConsoleApp1.Program;

namespace ConsoleApp1
{
    public delegate void Supplier<T>(long offset, long length);

    class RemoteExecutor<T>
    {
        public void Run(int serviceId, long offset, long length, Supplier<T> func)
        {
            func.Invoke(offset, length);
        }
    }

    public class WorkerThread
    {
        private Barrier barrier;
        private long length;
        private long offset;

        public WorkerThread(long offset, long length, Barrier barrier)
        {
            this.barrier = barrier;
            this.length = length;
            this.offset = offset;
        }

        public void realworker(long offset, long length)
        {
            // here we need to calcutate words and do the real work
            DestributedFS.getInstance().getData(offset);
        }

        public void ThreadProc(object id)
        {
            new RemoteExecutor<int>().Run((int)id, length, offset, new Supplier<int>(realworker));
            barrier.SignalAndWait();
        }
    }
}
