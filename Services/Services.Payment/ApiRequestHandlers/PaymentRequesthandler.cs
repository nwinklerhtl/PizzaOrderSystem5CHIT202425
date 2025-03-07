using DataContracts.DataTransferObjects;
using DataContracts.Messages.ServiceMessages;
using Infrastructure.Messaging.Outbox.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Services.Payment.Db;

namespace Services.Payment.ApiRequestHandlers;

public static class PaymentRequesthandler
{
    public static async Task<PaymentResponseDto> HandlePayment(
        [FromBody] PaymentRequestDto dto,
        [FromServices] PaymentDbContext dbContext)
    {
        var pendingPayment = await dbContext.Payments.FirstOrDefaultAsync(p => p.OrderId == dto.OrderId);

        if (pendingPayment is null) throw new ArgumentException("No pending payment for order");

        if (pendingPayment.PaymentAmount > dto.Amount) throw new ArgumentException("Too little, too less");
        
        // user paid the (at least) the right amount
        pendingPayment.PaidAt = DateTimeOffset.UtcNow;

        dbContext.Outbox.Add(OutboxMessage.FromMessage(new PaymentReceived()
        {
            OrderId = dto.OrderId
        }));

        await dbContext.SaveChangesAsync();

        return new PaymentResponseDto()
        {
            OrderId = dto.OrderId,
            PaidAt = pendingPayment.PaidAt.Value
        };
    }
}