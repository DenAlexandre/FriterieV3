namespace FriterieShop.Application.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(Guid id)
            : base($"Item with ({id}) not found.")
        {
        }
    }
}
