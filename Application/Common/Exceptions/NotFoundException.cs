namespace Application.Common.Exceptions;
public class NotFoundException : Exception
{
    public NotFoundException() : base()
    {

    }

    public NotFoundException(string message) : base(message)
    {

    }

    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {

    }

    public NotFoundException(string name, int id) : base($"Entity {name} with Id {id} was not found")
    {

    }

    public NotFoundException(string name, int id, int userId) : base($"Entity {name} with Id {id} created by user {userId} was not found")
    {

    }
}