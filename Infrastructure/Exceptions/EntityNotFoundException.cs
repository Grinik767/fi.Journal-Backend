namespace Infrastructure.Exceptions;

public class EntityNotFoundException(string message, Exception innerException) : Exception(message, innerException);