using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ConcurrentTransferMoney.Models;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class ProducerConsumerQueue : IDisposable
    {
        private readonly object _locker = new object();
        private readonly Queue<BankTransferModel> _tasks = new Queue<BankTransferModel>();
        private readonly EventWaitHandle _wh = new AutoResetEvent(false);
        private readonly Thread _worker;

        public ProducerConsumerQueue()
        {
            _worker = new Thread(Work);
            _worker.Start();
        }

        public void Dispose()
        {
            EnqueueTask(null); // Signal the consumer to exit.
            _worker.Join(); // Wait for the consumer's thread to finish.
            _wh.Close(); // Release any OS resources.
        }

        public void EnqueueTask(BankTransferModel task)
        {
            lock (_locker) _tasks.Enqueue(task);
            _wh.Set();
        }

        private void Work()
        {
            while (true)
            {
                BankTransferModel task = null;
                lock (_locker)
                    if (_tasks.Any())
                    {
                        task = _tasks.Dequeue();
                        if (task == null) return;
                    }
                if (task != null)
                {
                    Console.WriteLine("Performing task: " + task);
                    Thread.Sleep(1000); // simulate work...
                }
                else
                    _wh.WaitOne(); // No more tasks - wait for a signal
            }
        }
    }
}