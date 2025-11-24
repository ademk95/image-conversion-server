using Microsoft.AspNetCore.SignalR;

namespace ImageConversion.Services.Hubs;

public class ImageConversionHub : Hub
{
    public override Task OnConnectedAsync()
    {
        Clients.Caller.SendAsync("ConnId", Context.ConnectionId);
        return base.OnConnectedAsync();
    }
}