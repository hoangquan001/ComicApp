namespace ComicApp.Models
{
    public class ServiceResponse<T>
    {
        public T? Data { get; set; }
        public int Status { get; set; }
        public string Message { get; set; } = "";
    }
}