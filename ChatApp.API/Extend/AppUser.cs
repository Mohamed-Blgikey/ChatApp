using ChatApp.API.Entites;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.API.Extend
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }

        public IEnumerable<Message> MessageSent { get; set; }
        public IEnumerable<Message> MessageRecived { get; set; }
    }
}
