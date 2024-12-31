namespace ProiectPOO;
enum DiscountTypes
{
    TwoPlusOne,
    Percentage,
    Constant
}
public class Produs
{
    private string ID { get; set; }
    private string Name { get; set; }
    private string? Description { get; set; }
    private int Price { get; set; }
    private int? Stock { get; set; }
    private int? Rating { get; set; }
    private string? Category { get; set; }
    private Dictionary<DiscountTypes, bool> Discounts { get; set; } 

    public Produs(string id, string name, int price)
    {
        ID = id;
        Name = name;
        Price = price;

        Discounts = new Dictionary<DiscountTypes, bool>()
        {
            // nu e nicio reducere in curs in mod default
            {DiscountTypes.TwoPlusOne, false},
            {DiscountTypes.Percentage, false},
            {DiscountTypes.Constant, false}
        };
    }

    private void AddDiscount(DiscountTypes discountType)
    {

    }
    private int CalculatePrice(DiscountTypes discountType, int? AmountBought)
    {
        int FinalPrice;
        if (AmountBought != null)
        {
            FinalPrice = Price * (int)AmountBought;
        }
        else
        {
            FinalPrice = Price;
        }

        // reducerile se pot cumula
        // fara else intre tipurile de reducere
        if (discountType == DiscountTypes.TwoPlusOne && AmountBought > 2)
        {
            FinalPrice /= 3;
        }

        return FinalPrice;
    }

}