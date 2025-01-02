namespace ProiectPOO;
public enum DiscountTypes
{
    TwoPlusOne,
    Percentage,
    Constant
}
public enum ProductCategory
{
    // am pus doar cateva ca sa existe
    Electronice,
    Electrocasnice,
    Carti,
    Haine,
    Cosmetice,
    Auto,
    Jucarii
}
public class Produs
{
    public string ID { get; private set; }
    private string Name { get; set; }
    private string? Description { get; set; }
    public double Price { get; private set; }
    private int Stock { get; set; }
    private int? Rating { get; set; }
    public ProductCategory? Category { get; private set; }
    public Dictionary<DiscountTypes, bool> ThisProductsDiscounts { get; private set; } 

    public Produs(string id, string name, double price, int stock, ProductCategory category)
    {
        ID = id;
        Name = name;
        Price = CalculateFinalPrice(price);
        Stock = stock;
        Category = category;
        
        ThisProductsDiscounts = new Dictionary<DiscountTypes, bool>()
        {
            // nu e nicio reducere in curs in mod implicit
            {DiscountTypes.TwoPlusOne, false},
            {DiscountTypes.Percentage, false},
            {DiscountTypes.Constant, false}
        };
    }

    public void AddDiscount(DiscountTypes discountType)
    {
        // vom cauta un produs anume cu un for, si ii vom adauga un tip anume de reducere
        // metoda asta se va aplica doar pe obiectul ala
    }
    private double CalculateFinalPrice(double productOriginalPrice)
    {
        double FinalPrice = productOriginalPrice;

        // logica de calcul a pretului bazat pe ce reduceri active are produsul
        // sau ziceti sa calculam pretul la momentul comenzii?

        return FinalPrice;
    }

}