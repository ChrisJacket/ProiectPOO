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

    public void SignUpClient(Client ClientNou)
    {
        Users.Add(ClientNou);
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
        if (proposedId[0] == 'O')
            foreach (Comanda comanda in Comenzi)
            {
                if (comanda.ID == proposedId) return true;
            }
        else if (proposedId[0] == 'P')
            foreach (Produs produs in StocMagazin)
            {
                if (produs.ID == proposedId) return true;
            }
        return false;
    }
    public void AddOrder(Dictionary<Produs, int> productsOrdered, Client recipient, OrderStatus status, ShippingAddress deliveryAddress)
    {
        Comanda NewOrder = new Comanda(productsOrdered, GenerateNextOrderId(), recipient, OrderStatus.BeingProcessed, deliveryAddress);
        if (OrderAlreadyExists(NewOrder))
            Console.WriteLine("Order has already been added!");
        else 
            Comenzi.Add(NewOrder);
    }
    private bool OrderAlreadyExists(Comanda comanda)
    {
        foreach(Comanda comanda_aux in Comenzi)
            if(comanda.ID == comanda_aux.ID) return true;
        return false;
    }
    public void AddProduct(string productName, double price, int stock, ProductCategory? productCategory)
    {
        Produs NewProduct = new Produs(GenerateNextProductId(), productName, price, stock, productCategory);
        if (ProductAlreadyExists(NewProduct))
            Console.WriteLine("Product has already been added!");
        else
            StocMagazin.Add(NewProduct);
    }
    private bool ProductAlreadyExists(Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs.ID == produs_aux.ID) return true;
            // aici poate putem face o chestie, daca produsul deja exista
            // astfel incat sa adauge stocul produsului nou la produsul care deja exista
        }
        return false;
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
    public void EditProduct(Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs_aux.ID == produs.ID)
            {
                // logica de modificare a produsului
                break;
            }
        }
    }
    public User AuthenticateUser()
    {
        string email, parola;
        while (true)
        {
            Console.Write("Email: ");
            email = Console.ReadLine();

            Console.Write("Parola: ");
            parola = Console.ReadLine();

            if (email != null && parola != null)
                break;
            else
                Console.WriteLine("Emailul sau parola nu pot fi campuri goale!\n");
        }
        Console.Clear();

        // Verifica daca exista utilizatorul in lista de utilizatori
        // "=>" a se citi "cu proprietatea ca"
        return Users.First(u => u.VerifyUserCredentials(email, parola));
    }

}
