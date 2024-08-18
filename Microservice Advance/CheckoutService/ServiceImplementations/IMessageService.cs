using CheckoutService.Enums;

namespace CheckoutService.ServiceImplementations;

public interface IMessageService
{
    void PublishSuccessMessage(object message);
    void PublishFailedMessage(object message);

}