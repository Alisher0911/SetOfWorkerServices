using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace EmailService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IEmailSender _sender;

        public Worker(ILogger<Worker> logger, IEmailSender sender)
        {
            _logger = logger;
            _sender = sender;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync("https://localhost:5001/api/queue/retrieve/email");
                    string result = response.Content.ReadAsStringAsync().Result;
                    _logger.LogInformation(result);
                    try
                    {
                        var email = JsonConvert.DeserializeObject<MessageDTO>(result);
                        if (email != null)
                        {
                            var message = new Message(new string[] { "aorazbay09@gmail.com" }, "Email Service", email.JsonContent, "");
                            await _sender.SendEmail(message);
                            await client.GetAsync("https://localhost:5001/api/queue/retrieve/handled" + email.Id);
                        }
                    } catch (Exception e)
                    {
                        _logger.LogInformation(e.Message);
                    }
                }
                await Task.Delay(20000, stoppingToken);
            }
        }
    }
}
