namespace BusinessCardAPI
{
    public class Result
    {
        public bool Ok { get; set; }
        public dynamic? Data { get; set; } = null;
        public string Message { get; set; } = "";
    }
}