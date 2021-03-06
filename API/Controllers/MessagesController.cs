namespace API.Controllers;

[Authorize]
public class MessagesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;

    private readonly IMapper _mapper;
    public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    /* [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto){
        var username = User.GetUserName();
        if(username == createMessageDto.RecipientUsername.ToLower()) return BadRequest("You cannot send messages to yourself");

        var sender = await _userRepository.GetUserByUserNameAsync(username);
        var recipient = await _userRepository.GetUserByUserNameAsync(createMessageDto.RecipientUsername);
        if(recipient == null) return NotFound();

        var message = new Message(){
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            Content = createMessageDto.Content
        };

        _unitOfWork.MessageRepository.AddMessage(message);
        if(await _unitOfWork.MessageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDto>(message));

        return BadRequest("Failed to send message");
    }*/

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesFromUser([FromQuery] MessageParams messageParams){
        messageParams.Username = User.GetUserName();

        var messages = await _unitOfWork.MessageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize,messages.TotalCount,messages.TotalPages);

        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage(int id){
        var username = User.GetUserName();
        var message = await _unitOfWork.MessageRepository.GetMessage(id);
        if(message.Sender.UserName != username && message.Recipient.UserName != username) return Unauthorized();

        if(message.Sender.UserName == username) message.SenderDeleted = true;
        if(message.Recipient.UserName == username) message.RecipinetDeleted = true;

        if(message.SenderDeleted && message.RecipinetDeleted) _unitOfWork.MessageRepository.DeleteMessage(message);

        if(await _unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting message"); 
    }

}