namespace Application.Common.Exceptions;
public class NotFoundException : Exception
{
    public string UserMessage { get; set; }

    public NotFoundException(string name, int id, string userMessage) : base($"Entity {name} with Id {id} was not found")
    {
        UserMessage = userMessage;
    }

    public NotFoundException(string name, int id, int userId, string userMessage) : base($"Entity {name} with Id {id} created by user {userId} was not found")
    {
        UserMessage = userMessage;
    }
}