using ChatApp.API.Extend;

namespace ChatApp.API.Entites
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string SenderId { get; set; }
        public AppUser Sender { get; set; }

        public string ReciverId { get; set; }
        public AppUser Reciver { get; set; }
    }
}
