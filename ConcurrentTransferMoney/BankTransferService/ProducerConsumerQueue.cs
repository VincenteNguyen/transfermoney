using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentTransferMoney.BankTransferService
{
    public class ProducerConsumerQueue
    {
        private readonly BlockingCollection<WorkItem> _taskQ = new BlockingCollection<WorkItem>();
        public readonly Task ConsumeTask;

        public ProducerConsumerQueue()
        {
            ConsumeTask = Task.Factory.StartNew(Consume);
        }

        public void Dispose()
        {
            _taskQ.CompleteAdding();
        }

        public Task EnqueueTask(Action action)
        {
            return EnqueueTask(action, null);
        }

        public Task EnqueueTask(Action action, CancellationToken? cancelToken)
        {
            var tcs = new TaskCompletionSource<object>();
            _taskQ.Add(new WorkItem(tcs, action, cancelToken));
            return tcs.Task;
        }

        private void Consume()
        {
            foreach (var workItem in _taskQ.GetConsumingEnumerable())
                if (workItem.CancelToken.HasValue &&
                    workItem.CancelToken.Value.IsCancellationRequested)
                {
                    workItem.TaskSource.SetCanceled();
                }
                else
                    try
                    {
                        workItem.Action();
                        workItem.TaskSource.SetResult(null); // Indicate completion
                    }
                    catch (OperationCanceledException ex)
                    {
                        if (ex.CancellationToken == workItem.CancelToken)
                            workItem.TaskSource.SetCanceled();
                        else
                            workItem.TaskSource.SetException(ex);
                    }
                    catch (Exception ex)
                    {
                        workItem.TaskSource.SetException(ex);
                    }
        }

        private class WorkItem
        {
            public readonly Action Action;
            public readonly CancellationToken? CancelToken;
            public readonly TaskCompletionSource<object> TaskSource;

            public WorkItem(
                TaskCompletionSource<object> taskSource,
                Action action,
                CancellationToken? cancelToken)
            {
                TaskSource = taskSource;
                Action = action;
                CancelToken = cancelToken;
            }
        }
    }
}