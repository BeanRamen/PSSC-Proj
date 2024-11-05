using lab3.Exceptions;
using static lab3.Models.Item;

namespace lab3.Operations
{
  internal abstract class ItemOperation<TState> : DomainOperation<IItem, TState, IItem>
    where TState : class
  {
    public override IItem Transform(IItem item, TState? state) => item switch
    {
      UnvalidatedItem unvalidatedItem => OnUnvalidated(unvalidatedItem, state),
      ValidatedItem validItem => OnValid(validItem, state),
      InvalidItem invalidItem => OnInvalid(invalidItem, state),
      CalculatedItem calculatedItem => OnCalculated(calculatedItem, state),
      PublishedItem publishedItem => OnPublished(publishedItem, state),
      _ => throw new InvalidItemStateException(item.GetType().Name)
    };

    protected virtual IItem OnUnvalidated(UnvalidatedItem unvalidatedItem, TState? state) => unvalidatedItem;

    protected virtual IItem OnValid(ValidatedItem validItem, TState? state) => validItem;

    protected virtual IItem OnPublished(PublishedItem publishedItem, TState? state) => publishedItem;

    protected virtual IItem OnCalculated(CalculatedItem calculatedItem, TState? state) => calculatedItem;

    protected virtual IItem OnInvalid(InvalidItem invalidItem, TState? state) => invalidItem;
  }

  internal abstract class ItemOperation : ItemOperation<object>
  {
    internal IItem Transform(IItem Item) => Transform(Item, null);

    protected sealed override IItem OnUnvalidated(UnvalidatedItem unvalidatedItem, object? state) => OnUnvalidated(unvalidatedItem);

    protected virtual IItem OnUnvalidated(UnvalidatedItem unvalidatedItem) => unvalidatedItem;

    protected sealed override IItem OnValid(ValidatedItem validItem, object? state) => OnValid(validItem);

    protected virtual IItem OnValid(ValidatedItem validItem) => validItem;

    protected sealed override IItem OnPublished(PublishedItem publishedItem, object? state) => OnPublished(publishedItem);

    protected virtual IItem OnPublished(PublishedItem publishedItem) => publishedItem;

    protected sealed override IItem OnCalculated(CalculatedItem calculatedItem, object? state) => OnCalculated(calculatedItem);

    protected virtual IItem OnCalculated(CalculatedItem calculatedItem) => calculatedItem;

    protected sealed override IItem OnInvalid(InvalidItem invalidItem, object? state) => OnInvalid(invalidItem);

    protected virtual IItem OnInvalid(InvalidItem invalidItem) => invalidItem;
  }
}