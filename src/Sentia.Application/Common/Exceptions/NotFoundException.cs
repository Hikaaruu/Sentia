namespace Sentia.Application.Common.Exceptions;

public class NotFoundException(string resourceName, object key) : Exception($"{resourceName} '{key}' was not found.")
{
}
