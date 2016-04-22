using System.Threading;
using System.Threading.Tasks;

namespace TCell.Entities.Threading
{
    public class TaskFacility : EntityBase
    {
        public Task TaskInstance { get; set; }
        public CancellationTokenSource CancellationToken { get; set; }
        public bool IsCancellationRequested
        {
            get { return (CancellationToken == null) ? true : CancellationToken.IsCancellationRequested; }
        }

        public void CancelTask()
        {
            if (CancellationToken != null)
                CancellationToken.Cancel();

            TaskInstance = null;
        }
    }
}
