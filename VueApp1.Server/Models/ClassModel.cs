public class AuthData
{ 
    public string? Acc {  get; set; }
    public string? Pwd { get; set; }
    public string? Captcha { get; set; }
}


public class ResultModel<T>
{
    public bool isSuccess { get; set; }
    public string? ErrorMsg { get; set; }
    public T? Result { get; set; }
}