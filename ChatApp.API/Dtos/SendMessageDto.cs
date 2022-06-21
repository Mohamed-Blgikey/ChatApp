namespace ChatApp.API.Dtos
{
    public class SendMessageDto
    {
        public string SenderId { get; set; }
        public string ReciverId { get; set; }
        public string Content { get; set; }
    }
}
