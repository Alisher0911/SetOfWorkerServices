using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SetOfWorkerServices.Data;
using SetOfWorkerServices.Models;
using SetOfWorkerServices.Repositories.Interfaces;

namespace SetOfWorkerServices.Repositories
{
    public class QueueRepository: IQueueRepository
    {
        private readonly DataContext _context;

        public QueueRepository(DataContext context)
        {
            _context = context;
        }


        public async Task<bool> AddMessage(Message message)
        {
            _context.Messages.Add(message);
            return (await _context.SaveChangesAsync()) > 0;
        }

        public async Task<Message> GetEmailMessage()
        {
            var message = await _context.Messages.Where(x => !x.Handled && x.Type == "email").OrderBy(x => x.AddedAt).FirstOrDefaultAsync();
            return message;
        }

        public async Task<Message> GetLoggingMessage()
        {
            var message = await _context.Messages.Where(x => !x.Handled && x.Type == "log").OrderBy(x => x.AddedAt).FirstOrDefaultAsync();
            return message;
        }

        public async Task<bool> SetHandled(Guid messageId)
        {
            var message = _context.Messages.Where(x => x.Id == messageId).FirstOrDefault();
            if (message == null)
            {
                return false;
            }
            message.Handled = true;
            message.HandledAt = DateTime.Now;
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
