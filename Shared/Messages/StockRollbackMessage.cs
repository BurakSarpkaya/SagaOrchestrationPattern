namespace Shared.Messages
{
    public class StockRollbackMessage : IStockRollBackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
