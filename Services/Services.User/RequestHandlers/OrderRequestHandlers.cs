using DataContracts.DataTransferObjects;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Outbox.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.User.Db;
using Services.User.Domain;

namespace Services.User.RequestHandlers;

public static class OrderRequestHandlers
{
    public static async Task<OrderInfoDto> HandleGetOrderRequest(
        [FromRoute] Guid orderId,
        [FromServices] UserDbContext dbContext)
    {
        var orderRequest = await dbContext
            .OrderRequests
            .Include(r => r.Items)
            .FirstOrDefaultAsync(r => r.Id == orderId);
        
        if (orderRequest is null) throw new ArgumentException("orderId is not valid");

        return new OrderInfoDto()
        {
            OrderId = orderRequest.Id,
            OrderItems = orderRequest.Items.Select(i => new OrderInfoItemDto()
            {
                Amount = i.Amount,
                ArticleName = i.ArticleName
            }).ToList()
        };
    }
    
    public static async Task<PlaceOrderResponseDto> HandlePlaceOrderRequest(
        [FromBody] PlaceOrderRequestDto dto,
        [FromServices] UserDbContext dbContext,
        [FromServices] IMessageSender sender)
    {
        var orderId = Ulid.NewUlid().ToGuid();
        
        dbContext.OrderRequests.Add(
            new OrderRequest
            {
                Id = orderId,
                CustomerAddress = dto.CustomerAddress,
                CustomerName = dto.CustomerName,
                Items = dto.Items.Select(i => new OrderRequestItem
                {
                    ArticleName = i.ArticleName,
                    ArticlePrice = i.ArticlePrice,
                    Amount = i.Amount
                }).ToList()
            });

        dbContext.Outbox.Add(OutboxMessage.FromMessage(
            new OrderReceived()
            {
                OrderId = orderId,
                TotalValue = dto.Items.Sum(i => i.Amount * i.ArticlePrice)
            }));

        await dbContext.SaveChangesAsync();

        return new PlaceOrderResponseDto()
        {
            OrderId = orderId
        };
    }
}