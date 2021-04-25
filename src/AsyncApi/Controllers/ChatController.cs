using System;
using System.Threading.Tasks;
using AsyncApi.Services;
using Contract.Request;
using Contract.Response;
using Microsoft.AspNetCore.Mvc;

namespace AsyncApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        
        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }


        [HttpPost]
        public IActionResult Connect(ConnectRequest model)
        {
            var result = _chatService.Connect(model);
            if (result)
            {
                return Ok();
            }

            return Conflict();
        }

        [HttpGet]
        public IActionResult Clients()
        {
            var result = new ClientsResponse
            {
                Users = _chatService.GetClients()
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(MessageSendRequest model)
        {
            try
            {
                var result = await _chatService.SendMessage(model);
                return Ok(result);
            }
            catch (ClientNotFoundException)
            {
                return NotFound();
            }
        }
    }
}