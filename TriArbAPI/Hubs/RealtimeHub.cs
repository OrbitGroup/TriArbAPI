using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TriArbAPI.Services;


namespace TriArbAPI.Hubs
{
    public class RealtimeHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            await Clients.Caller.SendAsync("GetAllTriangles", TriangleSubscriber.Triangles);
        }

        public async Task GetAllTriangles()
        {
            await Clients.All.SendAsync("GetAllTriangles", TriangleSubscriber.Triangles);
        }
    }
}
