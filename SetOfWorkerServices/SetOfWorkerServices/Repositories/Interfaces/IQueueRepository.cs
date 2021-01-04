using System;
using System.Threading.Tasks;
using SetOfWorkerServices.Models;

namespace SetOfWorkerServices.Repositories.Interfaces
{
    public interface IQueueRepository
    {
        Task<bool> AddMessage(Message message);
        Task<bool> SetHandled(Guid messageId);
        Task<Message> GetEmailMessage();
        Task<Message> GetLoggingMessage();
    }
}
