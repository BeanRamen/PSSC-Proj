using lab3.Models;
using lab3.Operations;
using System;
using static lab3.Models.ItemPublishedEvent;

namespace lab3.Workflows
{
    public class PublishItemWorkflow
    {
        public IItemPublishedEvent Execute(PublishItemCommand command, Func<CartRegistrationNumber, bool> checkCartExists)
        {
            Item.UnvalidatedItem unvalidatedPrices = new(command.InputItemPrices);

            Item.IItem item = new ValidateItemOperation(checkCartExists).Transform(unvalidatedPrices);
            item = new CalculateItemOperation().Transform(item);
            item = new PublishItemOperation().Transform(item);

            return item.ToEvent();
        }
    }
}