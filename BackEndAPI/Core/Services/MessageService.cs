using BackEndAPI.Core.DbContext;
using BackEndAPI.Core.Dtos.General;
using BackEndAPI.Core.Dtos.Message;
using BackEndAPI.Core.Entities;
using BackEndAPI.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackEndAPI.Core.Services
{
    public class MessageService : IMessageService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;
        private readonly UserManager<ApplicationUsers> _userManager;

        public MessageService(ApplicationDbContext context, ILogService logService, UserManager<ApplicationUsers> userManager)
        {
            _context = context;
            _logService = logService;
            _userManager = userManager;
        }

        public async Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal user, CreateMessageDto createMessageDto)
        {
            if (user.Identity.Name == createMessageDto.ReceiverUserName)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Sender and Receiver can not be same",
                    StatusCode = 400
                };
            }
            var isReceivedUserNameValid = _userManager.Users.Any(x => x.UserName == createMessageDto.ReceiverUserName);
            if (!isReceivedUserNameValid)
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    Message = "Receiver user name is not valid",
                    StatusCode = 400
                };
            }
            Message newMessage = new Message()
            {
                SendUserName = user.Identity.Name,
                ReceiverUserName = createMessageDto.ReceiverUserName,
                Text = createMessageDto.Text,
            };
            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();
            await _logService.SaveNewLog(user.Identity.Name, "Send Message");
            return new GeneralServiceResponseDto()
            {
                IsSucceed = true,
                Message = "Message Saved Successfully",
                StatusCode = 201
            };
        }

        public async Task<IEnumerable<GetMessageDto>> GetMessagesAsync()
        {
            var message = await _context.Messages.Select(x => new GetMessageDto()
            {
                Id = long.Parse(x.Id),
                SenderUserName = x.SendUserName,
                ReceiverUserName = x.ReceiverUserName,
                Text = x.Text,
                CreatedAt = x.CreatedAt,
            }).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return message;
        }

        public async Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal user)
        {
            var loggedInUser = user.Identity.Name;
            var message = await _context.Messages
                .Where(x => x.SendUserName == loggedInUser)
                .Select(x => new GetMessageDto()
                {
                    Id = long.Parse(x.Id),
                    SenderUserName = x.SendUserName,
                    ReceiverUserName = x.ReceiverUserName,
                    Text = x.Text,
                    CreatedAt = x.CreatedAt,
                }).OrderByDescending(x => x.CreatedAt).ToListAsync();
            return message;
        }

    }
}
