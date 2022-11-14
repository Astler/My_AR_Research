using System.Collections.Generic;

namespace Infrastructure.Interfaces.Activate
{
    public interface IActivatablesQueue
    {
        IEnumerable<IActivatable> GetActivatablesQueue();
    }
}