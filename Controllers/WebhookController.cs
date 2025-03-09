using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        // Read the webhook payload
        using var reader = new System.IO.StreamReader(Request.Body);
        var payload = await reader.ReadToEndAsync();

        // Debugging: Log the raw payload
        Console.WriteLine("Raw Webhook Payload: " + payload);

        try
        {
            // Deserialize the payload
            var webhookEvent = JsonSerializer.Deserialize<WebhookEvent>(payload);

            // Debugging: Log the deserialized payload
            Console.WriteLine("Deserialized Webhook Payload: " + JsonSerializer.Serialize(webhookEvent));

            // Handle the payment status
            switch (webhookEvent.EventType)
            {
                case "PaymentSucceeded":
                    Console.WriteLine("Payment Succeeded: " + webhookEvent.OrderCode);
                    break;

                case "PaymentFailed":
                    Console.WriteLine("Payment Failed: " + webhookEvent.OrderCode);
                    Console.WriteLine("Error Code: " + webhookEvent.ErrorCode);
                    Console.WriteLine("Error Text: " + webhookEvent.ErrorText);
                    break;

                default:
                    Console.WriteLine("Unknown Event Type: " + webhookEvent.EventType);
                    break;
            }

            return Ok();
        }
        catch (JsonException ex)
        {
            // Debugging: Log JSON deserialization errors
            Console.WriteLine("Error deserializing webhook payload: " + ex.Message);
            return BadRequest("Invalid JSON payload");
        }
    }

    public class WebhookEvent
    {
        public string EventType { get; set; } // e.g., "PaymentSucceeded", "PaymentFailed"
        public long OrderCode { get; set; }
        public string MerchantId { get; set; }
        public string CustomerEmail { get; set; }
        public string ErrorCode { get; set; } // For failed payments
        public string ErrorText { get; set; } // For failed payments
    }
}