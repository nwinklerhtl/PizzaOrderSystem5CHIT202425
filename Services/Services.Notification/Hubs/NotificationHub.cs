using DataContracts.DataTransferObjects;
using Microsoft.AspNetCore.SignalR;

namespace Services.Notification.Hubs;

public class NotificationHub : Hub
{
    public async Task RegisterOrderUpdates(string orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderId);
    }

    public async Task SendNotificationToGroup(string groupId, NotificationDto notification)
    {
        await Clients.Group(groupId).SendAsync("OrderUpdate", notification);
    }
}