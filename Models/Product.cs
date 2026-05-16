namespace SyncShoppingList.Mobile.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsPurchased { get; set; }
        public string AddedBy { get; set; } = "";
    }
}