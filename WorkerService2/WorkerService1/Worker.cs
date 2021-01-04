using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace WorkerService1
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher watcher;
        private readonly string path = @"/Users/alisher/Documents/worker";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }


        public override Task StartAsync(CancellationToken cancellationToken)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = path;

            watcher.Created += OnChanged;
            //watcher.Created += OnCreated;
            //watcher.Changed += OnChanged;
            //watcher.Deleted += OnDeleted;
            //watcher.Renamed += OnRenamed;

            return base.StartAsync(cancellationToken);
        }

        public async Task SendMessage(string filename)
        {
            var message = new
            {
                Type = "email",
                JsonContent = "A file " + filename + " was added."
            };

            var json = JsonConvert.SerializeObject(message);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://localhost:5001/api/queue/add", data);
                string result = response.Content.ReadAsStringAsync().Result;
                _logger.LogInformation(result);
            }
        }


        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("New message sending at : {time}", DateTimeOffset.Now);
            SendMessage(e.FullPath);
            //_emailLogger.OnChanged(e);
        }

        /*private void OnDeleted(object sender, FileSystemEventArgs e)
        {
            _emailLogger.OnDeleted(e);
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            _emailLogger.OnRenamed(e);
        }

        private void OnCreated(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("New message sending at : {time}", DateTimeOffset.Now);
            SendMessage(e.FullPath);
            //_emailLogger.OnCreated(e);
        }*/



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                watcher.EnableRaisingEvents = true;
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(5000, stoppingToken);
            }
        }


    }
}
