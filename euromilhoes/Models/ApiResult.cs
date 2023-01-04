namespace euromilhoes.Models;

public class ApiResult<T>
{
    public ApiResult(T? data = default)
    {
        Data = data;
    }

    public bool Success { get; set; } = true;

    public string? Message { get; set; }

    public T? Data { get; set; }
}
