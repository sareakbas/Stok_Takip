namespace Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Barcode { get; set; }= string.Empty;
        public string Name { get; set; }= string.Empty;
        public string Unit { get; set; }=string.Empty;
        public decimal MinStockLevel { get; set; } 
        public bool IsActive { get; set; }= true;

        public int CategoryId { get; set;}
        public Category Category { get; set;}= null!;
    }
}