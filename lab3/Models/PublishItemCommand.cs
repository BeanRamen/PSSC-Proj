using System.Collections.Generic;

namespace lab3.Models
{
    public record PublishItemCommand
    {
        public PublishItemCommand(IReadOnlyCollection<UnvalidatedCartPrice> inputItemPrices)
        {
            InputItemPrices = inputItemPrices;
        }

        public IReadOnlyCollection<UnvalidatedCartPrice> InputItemPrices { get; }
    }
}