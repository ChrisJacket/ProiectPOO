namespace ProiectPOO;

public class Magazin
{
    private List<Comanda> Comenzi { get; set; }
    private List<Produs> StocMagazin { get; set; }
    public List<User> Users { get; private set; }

    public Magazin()
    {
        Comenzi = new List<Comanda>();
        StocMagazin = new List<Produs>();
        Users = new List<User>();
    }

    public List<User> LoadUsersFromFile()
    {
        throw new NotImplementedException();      
    }
    public List<Comanda> LoadOrdersFromFile()
    {
        throw new NotImplementedException();
    }
    public List<Produs> LoadProductsFromFile()
    {
        throw new NotImplementedException();
    }

    private string GenerateNextOrderId()
    {
        Random Randomizer = new Random();
        string randomID;
        do
            randomID = "O" + Randomizer.Next(100000, 999999).ToString(); // O de la order
        while (IdAlreadyExists(randomID));
        return randomID;
    }
    private string GenerateNextProductId()
    {
        Random Randomizer = new Random();
        string randomID;
        do
            randomID = "P" + Randomizer.Next(100000, 999999).ToString(); // O de la order
        while (IdAlreadyExists(randomID));
        return randomID;
    }
    private bool IdAlreadyExists(string proposedId)
    {
        foreach(Comanda comanda in Comenzi)
        {
            if(comanda.ID == proposedId) return true;
        }
        return false;
    }

    public void AddOrder(Dictionary<Produs, int> productsOrdered, Client recipient, OrderStatus status, ShippingAddress deliveryAddress)
    {
        Comanda NewOrder = new Comanda(productsOrdered, GenerateNextOrderId(), recipient, OrderStatus.BeingProcessed, deliveryAddress);
        Comenzi.Add(NewOrder);
    }

    public void AddProduct(string productName, double price, int stock, ProductCategory productCategory)
    {
        Produs NewProduct = new Produs(GenerateNextProductId(), productName, price, stock, productCategory);
        StocMagazin.Add(NewProduct);
    }

    public void EditOrder(Comanda comanda)
    {
        foreach(Comanda comanda_aux in Comenzi)
        {
            if(comanda_aux.ID == comanda.ID)
            {
                // logica de modificare a comenzii
                break;
            }
        }
    }

    public void AddProduct(Admin admin)
    {
        throw new NotImplementedException();
    }

}
