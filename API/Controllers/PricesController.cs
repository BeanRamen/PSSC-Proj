using System.Collections.ObjectModel;
using lab3.Models;
using lab3.Repos;
using lab3.Workflows;
using Microsoft.AspNetCore.Mvc;
using API.Models;

namespace API.Controllers
{
  [ApiController]
  [Route("[controller]")]
  public class PricesController : ControllerBase
  {
    private readonly ILogger<PricesController> logger;
    private readonly PublishItemWorkflow publishPriceWorkflow;
    private readonly IHttpClientFactory _httpClientFactory;

    public PricesController(ILogger<PricesController> logger, PublishItemWorkflow publishPriceWorkflow, IHttpClientFactory httpClientFactory)
    {
      this.logger = logger;
      this.publishPriceWorkflow = publishPriceWorkflow;
      _httpClientFactory = httpClientFactory;
    }
    

    [HttpGet("getAllPrices")]
    public async Task<IActionResult> GetAllPrices([FromServices] IPricesRepository pricesRepository)
    {
      List<CalculatedCartPrice> prices = await pricesRepository.GetExistingPricesAsync();
      var result = prices.Select(price => new
      {
        CartRegistrationNumber = price.CartRegistrationNumber.Value,
        ItemPrice = price.ItemPrice?.Value,
        Reducere = price.TVA?.Value + " %",
        FinalPrice = price.FinalPrice?.Value
      });
      return Ok(result);
    }

    [HttpGet("generateReceipt/{cartRegistrationNumber}")]
    public async Task<IActionResult> GenerateReceipt(
      [FromServices] GenerateReceiptWorkflow generateReceiptWorkflow,
      string cartRegistrationNumber)
    {
      var receipt = await generateReceiptWorkflow.ExecuteAsync(new CartRegistrationNumber(cartRegistrationNumber));
      return Ok(new { Message = "Receipt generated successfully.", Receipt = receipt });
    }

    [HttpGet("generateAwb")]
    public async Task<IActionResult> GenerateAwb([FromServices] GenerateAWBWorkflow generateAWBWorkflow)
    {
      var (address, awb) = await generateAWBWorkflow.ExecuteAsync();
      return Ok(new
      {
        Message = "AWB generated successfully.",
        DeliveryAddress = address,
        AWB = awb
      });
    }


    
    [HttpPost]
    public async Task<IActionResult> PublishPrices([FromBody] InputPrice[] prices)
    {
      ReadOnlyCollection<UnvalidatedCartPrice> unvalidatedPrices = prices
        .Select(MapInputPriceToUnvalidatedPrice)
        .ToList()
        .AsReadOnly();
      PublishItemCommand command = new(unvalidatedPrices);
      ItemPublishedEvent.IItemPublishedEvent workflowResult = await publishPriceWorkflow.ExecuteAsync(command);

      IActionResult response = workflowResult switch
      {
        ItemPublishedEvent.ItemPublishSucceededEvent @event => Ok(),
        ItemPublishedEvent.ItemPublishFailedEvent @event => BadRequest(@event.Reasons),
        _ => throw new NotImplementedException()
      };

      return response;
    }

    private static UnvalidatedCartPrice MapInputPriceToUnvalidatedPrice(InputPrice price) => new(
        CartRegistrationNumber: price.RegistrationNumber,
        ItemPrice: price.ItemPrice,
        TVA: price.TVA);
  }
}
