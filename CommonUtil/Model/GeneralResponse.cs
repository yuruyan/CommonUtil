namespace CommonUtil.Model;

public class GeneralResponse<T> {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;
    public T data { get; set; }
}

public class GeneralResponse {
    public int code { get; set; }
    public string message { get; set; } = string.Empty;
}
