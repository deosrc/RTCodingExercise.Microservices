namespace RTCodingExercise.Microservices.Services;

public abstract record ApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
}
