using Gamgingroup.DTOs;
using Gamgingroup.Helpers;
using Gamgingroup.Models;

namespace Gamgingroup.Intefaces
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<AppUser> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}