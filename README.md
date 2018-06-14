# Xer.DomainDriven
Domain Driven Design entities and marker interfaces.

Below are super basic examples on how to use the DDD types:

Aggregate Root:
```csharp
public class OrderAggregateRoot : AggregateRoot
{
    private readonly List<LineItem> _lineItems;
  
    public OrderAggregateRoot (Guid orderId, IEnumerable<LineItem> lineItems)
        : base(orderId) // Pass ID to base.
    {
        _lineItems = lineItems.ToList();
    }

    public void Approve()
    {
        // Apply domain events to be committed and published by repository.
        // Include line item IDs in event.
        ApplyDomainEvent(new OrderApprovedEvent(Id, _lineItems.Select(li => li.Id)));
    }

    public void Cancel()
    {
        // Apply domain events to be committed and published by repository.
        // Include line item IDs in event.
        ApplyDomainEvent(new OrderCancelledEvent(Id, _lineItems.Select(li => li.Id)));
    }
}
```

Entity:
```csharp
public class LineItem : Entity
{
    public string ProductName { get; }
    public string Description { get; }
    public Price Price { get; }
    
    public LineItem(Guid lineItemId, string productName, string description, Price price)
        : base(lineItemId) // Pass ID to base.
    {
        ProductName = productName;
        Description = description;
        Price = price;
    }
    
    public void ApplyDiscount(decimal discountPercentage)
    {
        Price = Price.ApplyDiscount(discountPercentage);
    }
}
```

Value Object:
```csharp
public class Price : ValueObject<Price>
{
    public decimal Amount { get; }
    public string Currency { get; }
    
    public Price(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }
    
    public Price AdjustAmount(decimal newAmount)
    {
        // Value objects should be immutable, so create a new instance.
        return new Price(newAmount, Currency);
    }
    
    public Price ApplyDiscount(decimal discountPercentage)
    {
        decimal discountedAmount = Price.Amount - (Price.Amount * discountPercentage);
        // Value objects should be immutable, so create a new instance.
        return new Price(discountedAmount, Currency);
    }
    
    public override bool ValueEquals(Price other)
    {
        return Amount == other.Amount &&
               Currency == other.Currency;
    }
    
    public override HashCode GenerateHashCode()
    {
        return HashCode.New.Combine(Amount)
                           .Combine(Currency);
    }
}
```
