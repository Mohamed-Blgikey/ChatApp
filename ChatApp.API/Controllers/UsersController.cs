using ChatApp.API.Database;
using ChatApp.API.Dtos;
using ChatApp.API.Entites;
using ChatApp.API.Extend;
using ChatApp.API.Helper;
using ChatApp.API.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChatApp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> userManager;
        private readonly AppDbContext context;
        private readonly IHubContext<ChatHub> hubContext;

        public UsersController(UserManager<AppUser> userManager,AppDbContext context,IHubContext<ChatHub> hubContext)
        {
            this.userManager = userManager;
            this.context = context;
            this.hubContext = hubContext;
        }
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var users = await userManager.Users.Where(u=>u.Id != User.FindFirst(ClaimTypes.NameIdentifier).Value).ToListAsync();
            return Ok(new { Count =  users.Count,Data = users });
        }
        
        [HttpPost("SendMessage")]
        public async Task<IActionResult> Send([FromBody] SendMessageDto dto)
        {
            var message = new Message
            {
                Id = 0,
                SenderId = dto.SenderId,
                ReciverId = dto.ReciverId,
                Content = dto.Content
            };


            await context.Messages.AddAsync(message);
            await context.SaveChangesAsync();
            await hubContext.Clients.All.SendAsync("ReciveMessage", message);

            return Ok();
        }
        [HttpGet("GetConversation/{resciverId}")]
        public async Task<IActionResult> GetConversation(string resciverId)
        {

            var conv = await context.Messages.Where(m => (m.SenderId == User.FindFirst(ClaimTypes.NameIdentifier).Value && m.ReciverId == resciverId) ||(m.ReciverId == User.FindFirst(ClaimTypes.NameIdentifier).Value && m.SenderId == resciverId)).Include(m=>m.Sender).Include(m => m.Reciver).ToListAsync();
            return Ok(conv);
        }
    }
}
