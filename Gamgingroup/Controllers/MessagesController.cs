using AutoMapper;
using Gamgingroup.DTOs;
using Gamgingroup.Intefaces;
using Gamgingroup.Models;
using Gamgingroup.Extensions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Gamgingroup.Helpers;
using System.Collections.Generic;

namespace Gamgingroup.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IMapper _imapper;

        public MessageController(IUserRepository userRepository,
            IMessageRepository messageRepository, IMapper imapper)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _imapper = imapper;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("please chat with others");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);

            if (recipient == null) return NotFound();
            var message = new Message                 //to orgainze the details to pass it to message 
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllSync())
                return Ok(_imapper.Map<MessageDto>(message));

            return BadRequest("Something go wrong. Cannot send the message now.");
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDto>>> 
            GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            var messages = await _messageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,
                messages.TotalCount, messages.TotalPages);

            return messages;

        }



        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username)
        {
            var currentUsername = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
        }
    }
}