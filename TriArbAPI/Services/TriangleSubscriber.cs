using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TriArbAPI.Hubs;
using StackExchange.Redis;

namespace TriArbAPI.Services
{
    public class TriangleSubscriber : BackgroundService
    {
        private readonly IHubContext<RealtimeHub> _hubContext;

        private readonly ILogger<TriangleSubscriber> _logger;

        private ConnectionMultiplexer redis;

        private IDatabase db;

        private ISubscriber subscriber;

        

        public static ConcurrentDictionary<string, decimal> Triangles = new ConcurrentDictionary<string, decimal>();

        public TriangleSubscriber(ILogger<TriangleSubscriber> logger, IHubContext<RealtimeHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;

            redis = ConnectionMultiplexer.Connect(System.IO.File.ReadAllText("/mnt/secrets-store/redis"));
            db = redis.GetDatabase();
            subscriber = redis.GetSubscriber();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            subscriber.Subscribe("triangles", (channel, message) =>
            {
                ReceiveUpdate(message);
            });

            await Task.Run(async () =>
            {
                await Wait(stoppingToken);
            }, stoppingToken);
        }

        private async Task ReceiveUpdate(RedisValue message)
        {
            var profit = decimal.Parse(await db.HashGetAsync(message.ToString(), "profit"));
            _logger.LogDebug($"{message} | {profit}");
            Triangles.AddOrUpdate(message.ToString(), profit, (key, oldValue) => oldValue = profit);
            _hubContext.Clients.All.SendAsync("TriangleUpdate", message, profit);
        }

        private async Task Wait(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}
