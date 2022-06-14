namespace SimpleFileSystemServer.Model;

public class GeneralResponse<T> {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;
    public T data { get; set; }

    public override string ToString() {
        return $"{{{nameof(code)}={code.ToString()}, {nameof(message)}={message}, {nameof(data)}={data}}}";
    }
}

public class GeneralResponse {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;

    public override string ToString() {
        return $"{{{nameof(code)}={code.ToString()}, {nameof(message)}={message}}}";
    }
}
