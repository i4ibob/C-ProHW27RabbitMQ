using C_ProHW27RabbitMQ.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
 

namespace C_ProHW27RabbitMQ.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly RabbitMqService _rabbitMqService;
        private static readonly ConcurrentDictionary<string, List<string>> Messages = new();

        public ChatController(RabbitMqService rabbitMqService)
        {
            _rabbitMqService = rabbitMqService;
        }

        [HttpPost("send")]
        public IActionResult SendMessage(string fromUser, string toUser, string message)
        {
            var queueName = $"chat_{toUser}";
            _rabbitMqService.SendMessage(queueName, $"{fromUser}: {message}");
            return Ok("Message sent!");
        }

        [HttpGet("receive")]
        public IActionResult ReceiveMessages(string user)
        {
            var queueName = $"chat_{user}";
            var messages = new List<string>();

            _rabbitMqService.ReceiveMessages(queueName, message =>
            {
                messages.Add(message);
                if (!Messages.ContainsKey(user))
                {
                    Messages[user] = new List<string>();
                }
                Messages[user].Add(message);
            });

            return Ok(Messages.GetValueOrDefault(user) ?? new List<string>());
        }
    }
}
