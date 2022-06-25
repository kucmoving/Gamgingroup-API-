using Gamgingroup.DTOs;
using Gamgingroup.Helpers;
using Gamgingroup.Models;

namespace Gamgingroup.Intefaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipentUsername);
        Task<bool> SaveAllSync();
    }
}
