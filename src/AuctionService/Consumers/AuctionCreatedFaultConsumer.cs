using Contracts;
using MassTransit;

namespace AuctionService.Consumers;

public class AuctionCreatedFaultConsumer : IConsumer<Fault<AuctionCreated>>
{
    public async Task Consume(ConsumeContext<Fault<AuctionCreated>> context)
    {
        Console.WriteLine($"Consuming AuctionCreatedFault for AuctionId: {context.Message.Message.Id}");

        var exception = context.Message.Exceptions.FirstOrDefault();

        if (exception.ExceptionType == "System.ArgumentException")
        {
            context.Message.Message.Model = "Foobar";
            await context.Publish(context.Message.Message);
        }
        else
        {
            Console.WriteLine($"Unhandled exception type: {exception.ExceptionType}");
        }
    }
}
