using AutoMapper;
using AutoMapper.QueryableExtensions;
using Gamgingroup.Data;
using Gamgingroup.DTOs;
using Gamgingroup.Helpers;
using Gamgingroup.Intefaces;
using Gamgingroup.Models;
using Microsoft.EntityFrameworkCore;

namespace Gamgingroup.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _dataContext;
        private readonly IMapper _imapper;

        public MessageRepository(DataContext dataContext, IMapper imapper)
        {
            _dataContext = dataContext;
            _imapper = imapper;
        }

        public void AddMessage(Message message)
        {
            _dataContext.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _dataContext.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _dataContext.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _dataContext.Messages
                .OrderByDescending(m => m.MessageSent)
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Inbox" => query.Where(x => x.Recipient.UserName == messageParams.Username),
                "Outbox" => query.Where(x => x.Sender.UserName == messageParams.Username),
                _ => query.Where(x => x.Recipient.UserName ==
                messageParams.Username && x.DateRead == null)
            };

            var messages = query.ProjectTo<MessageDto>(_imapper.ConfigurationProvider);
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername,
            string recipientUsername)
        {
            var messages = await _dataContext.Messages
                .Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.Recipient.UserName == currentUsername
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername
                )
                .OrderBy(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _dataContext.SaveChangesAsync();
            }

            return _imapper.Map<IEnumerable<MessageDto>>(messages);
        }










        public async Task<bool> SaveAllSync()
        {
            return await _dataContext.SaveChangesAsync() > 0;
        }
    }
}
