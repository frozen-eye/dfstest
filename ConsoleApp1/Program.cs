using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{

    class MyStream : Stream
    {
        public override bool CanRead => true;

        public override bool CanSeek => true;

        public override bool CanWrite => false;

        public override long Length => Program.FileLength;

        public override long Position { get; set; }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            Random r = new Random();
            int len = Math.Min(buffer.Length - offset, count);

            if (len > 0)
            {
                for (int i = offset; i < offset + len; i++)
                {
                    buffer[i] = (byte) r.Next(32, 127);
                }

                Position += len;
            }

            return len;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch(origin)
            {
                case SeekOrigin.Begin:
                    Position = offset;
                    break;
                case SeekOrigin.Current:
                    Position += offset;
                    break;
                case SeekOrigin.End:
                    Position = Length - offset;
                    break;
            }
            return Position;
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }

    class Program
    {
        public static int NUM_OF_THREADS = 100;
        public static int FileLength = 1001;

        public class DFS
        {
            public long fileLength()
            {
                return FileLength;
            }

            public MyStream getData(long offset)
            {
                MyStream myStream = new MyStream();
                myStream.Seek(offset, SeekOrigin.Begin);
                return myStream;
            }
        }

        public class DestributedFS
        {
            public static DFS getInstance()
            {
                return new DFS();
            }
        }

        static void Main(string[] args)
        {
            Barrier barrier = new Barrier(NUM_OF_THREADS+1);

            long fileLength = DestributedFS.getInstance().fileLength();
            long chunkLength = fileLength / NUM_OF_THREADS;

            for (int i = 0; i < NUM_OF_THREADS; i++)
            {
                long workerThreadChunkLength = (i == NUM_OF_THREADS - 1) ? (fileLength - (NUM_OF_THREADS -1)* chunkLength) : (chunkLength);
                WorkerThread tws = new WorkerThread(chunkLength * i, workerThreadChunkLength, barrier);

                // Create a thread to execute the task, and then
                // start the thread.
                Thread t = new Thread(tws.ThreadProc);
                t.Start(i);
            }

            barrier.SignalAndWait();
            Console.WriteLine("Threads started, waiting..");
        }
    }
}
