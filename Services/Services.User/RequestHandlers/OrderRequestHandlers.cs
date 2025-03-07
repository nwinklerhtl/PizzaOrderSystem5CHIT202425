using DataContracts.DataTransferObjects;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.Interfaces;
using Infrastructure.Messaging.Outbox.Domain;
using Microsoft.AspNetCore.Mvc;
using Services.User.Db;
using Services.User.Domain;

namespace Services.User.RequestHandlers;

public static class OrderRequestHandlers
{
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