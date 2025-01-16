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
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public double Price { get; private set; }
    public int Stock { get; private set; }
    private int? Rating { get; set; }
    public ProductCategory? Category { get; private set; }
    public Dictionary<DiscountTypes, bool> ThisProductsDiscounts { get; private set; }
    public int? PercentageDiscount { get; private set; }
    public int? ConstantDiscount { get; private set; }
    public Produs(string id, string name, double price, int stock, ProductCategory? category)
    {
        ID = id;
        Name = name;
        Price = price;
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

    public void AddStock(Admin admin, int StockToBeAdded)
    {
        Stock += StockToBeAdded;
    }

    public void ModifyStock(Admin admin, int NewStock)
    {
        Stock = NewStock;
    }

    public void ModifyName(Admin admin, string NewName)
    {
        if(NewName != null)
            Name = NewName;
    }

    public void ModifyPrice(Admin admin, double NewPrice)
    {
        Price = NewPrice;
    }

    public void ModifyDescription(Admin admin, string NewDescription)
    {
        if(NewDescription != null)
            Description = NewDescription;
    }

    public void AddDiscount(DiscountTypes discountType, int DiscountSpecs)
    {
        if(discountType == DiscountTypes.Percentage && DiscountSpecs > 100)
        {
            Console.WriteLine("Procentajul de reducere nu poate fi mai mare de 100%!");
            return;
        }

        if(discountType == DiscountTypes.Constant && DiscountSpecs > Price)
        {
            Console.WriteLine("Reducerea nu poate fi mai mare decat pretul produsului!");
            return;
        }

        if(DiscountSpecs < 0)
        {
            Console.WriteLine("Reducerea nu poate fi negativa!");
            return;
        }

        // reducerea poate fi 0 fiindca aia va fi valoarea lui DiscountSpecs pe care o vom introduce
        // in cazul reducerii TwoPlusOne

        foreach(var key in ThisProductsDiscounts.Keys)
        {
            if (key == discountType)
            {
                if(key == DiscountTypes.TwoPlusOne)
                    ThisProductsDiscounts[key] = true;
                else if (key == DiscountTypes.Percentage)
                {
                    ThisProductsDiscounts[key] = true;
                    PercentageDiscount = DiscountSpecs;
                }
                else if (key == DiscountTypes.Constant)
                {
                    ThisProductsDiscounts[key] = true;
                    ConstantDiscount = DiscountSpecs;
                }
            }
        }
        // seteaza tipul de reducere introdus pe true
        // si adauga valoarea reducerii daca e cazul
    }

    public void RemoveDiscount(DiscountTypes discountType)
    {
        foreach (var key in ThisProductsDiscounts.Keys)
        {
            if (key == discountType)
            {
                ThisProductsDiscounts[key] = false;
            }
        }
    }

    public void RemoveAllDiscounts()
    {
        foreach (var key in ThisProductsDiscounts.Keys)
        {
            ThisProductsDiscounts[key] = false;
        }
    }
}