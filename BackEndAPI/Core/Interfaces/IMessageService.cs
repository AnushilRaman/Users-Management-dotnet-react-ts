using BackEndAPI.Core.Dtos.General;
using BackEndAPI.Core.Dtos.Message;
using System.Security.Claims;

namespace BackEndAPI.Core.Interfaces
{
    public interface IMessageService
    {
        Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal user, CreateMessageDto createMessageDto);
        Task<IEnumerable<GetMessageDto>> GetMessagesAsync();
        Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal user);
    }
}
