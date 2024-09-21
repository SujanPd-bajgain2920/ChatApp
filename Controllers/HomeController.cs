using ChatApp.Models;
using ChatApp.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;

namespace ChatApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ChatappContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDataProtector _protector;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ChatappContext context, IWebHostEnvironment env, DataSecurityProvider key, IDataProtectionProvider provider, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _env = env;
            _protector = provider.CreateProtector(key.Key);
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            var users = _context.UserLists.ToList();
            var userListEdit = users.Select(e => new UserListEdit
            {
                UserId = e.UserId,
                FullName = e.FullName,
                EmailAddress = e.EmailAddress,
                UserPassword = e.UserPassword,
                EncId = _protector.Protect(e.UserId.ToString())
            }).ToList();
            ViewData["Users"] = userListEdit;

            return View(userListEdit);
        }


        [HttpGet]
        public async Task<IActionResult> UserMessage(short receiverId)
        {
            var currentUserId = Convert.ToInt16(User.Identity!.Name); // Assuming UserId is stored in Identity

            // Fetch messages between the current user and the receiver
            var messages = await _context.Messages
                .Where(m => (m.SenderId == currentUserId && m.ReceiverId == receiverId) ||
                             (m.SenderId == receiverId && m.ReceiverId == currentUserId))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            // Fetch the receiver's details
            var receiver = await _context.UserLists
                .Where(u => u.UserId == receiverId)
                .Select(u => new { u.FullName })
                .FirstOrDefaultAsync();

            ViewBag.ReceiverName = receiver?.FullName;
            ViewBag.CurrentUserId = currentUserId; // Pass current user ID for comparison
            ViewBag.ReceiverId = receiverId; // Pass receiver ID to the view

            // Return the messages to the view
            return View(messages);
        }

      


    }
}
