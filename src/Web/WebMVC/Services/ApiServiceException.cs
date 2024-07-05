using System.Runtime.Serialization;

namespace RTCodingExercise.Microservices.Services;

public class ApiServiceException<ApiService> : Exception where ApiService : class
{
    public ApiServiceException()
    {
    }

    public ApiServiceException(string? message) : base(message)
    {
    }

    public ApiServiceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }

    protected ApiServiceException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
