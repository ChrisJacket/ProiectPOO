namespace ProiectPOO;

public class Magazin
{
    private List<Comanda> Comenzi { get; set; }
    public List<Produs> StocMagazin { get; private set; }
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
    public void AddProduct(string productName, double price, int stock, ProductCategory productCategory)
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

    public void ViewProducts(Client client)
    {
        int i=1;
        foreach (var kvp in client.ShoppingCart)
        {
            Console.WriteLine($"{i}. {kvp.Key}");
            i++;
        }

    }
    // listeaza toate produsele, cu un contor care le numeroteaza in afisaj
    // pentru a le putea selecta

    public void AddToCart(Client client)
    {
        Console.WriteLine("Ce produs doriti sa adaugati la cos?");
        string NumeProd = Console.ReadLine();
        foreach(Produs prod in StocMagazin)
        {
            if (prod.Name.Equals(NumeProd))
            {
                client.ShoppingCart.Add(prod, 1);
                break;
            }
        }
        Console.WriteLine("Doriti sa adaugati produsul de mai multe ori la cos?");
        string mProd = Console.ReadLine();
        if (mProd.Equals("da"))
        {
            Console.WriteLine("De cate ori adaugati produsul la cos?");
            string nrProd = Console.ReadLine();
            int nr = int.Parse(nrProd);
            client.ShoppingCart.Add(produs, nr);
        }
    }   
    // AddToCart();


    public void EditCart(Client client)
    {
        if (client.ShoppingCart.Any())
        {
            Console.WriteLine("Ce produs doriti sa modificati?");
            int mod = int.Parse(Console.ReadLine());
            Console.WriteLine("1. Stergeti produsul din cos");
            Console.WriteLine("2. Modificati numarul de produse");
            switch (mod)
            {
                case 1:
                    void RemoveFromCart()
                    {
                        if (mod - 1 >= 0 && mod - 1 < client.ShoppingCart.Count)
                        {
                            client.ShoppingCart.Remove(client.ShoppingCart.ElementAt(mod-1).Key);
                        }
                        else
                        {
                            Console.WriteLine("Numar invalid");
                        }
                    }
                    break;
                case 2:
                    void ModifyProdNr()
                    {
                        Console.WriteLine("Modificati numarul de produse la: ");
                        int nr = int.Parse(Console.ReadLine());
                        client.ShoppingCart[client.ShoppingCart.ElementAt(mod-1).Key] = nr;
                    }
                    break;
            }

        }
        else
        Console.WriteLine("Cosul de cumparaturi este gol - nu aveti ce sa modificati.");
    }
    // EditCart();

    public void FinalizeOrder(Client client)
    {
        var deliveryAddress = new DeliveryAddress();
        Console.WriteLine("Introduceti Adressa:");
        DeliveryAddress.Address = Console.ReadLine();
        Console.WriteLine("Introduceti codul postal:");
        DeliveryAddress.PostCode = Console.ReadLine();
        Console.WriteLine("Introduceti orasul:");
        DeliveryAddress.City = Console.ReadLine();
        Console.WriteLine("Introduceti tara:");
        DeliveryAddress.Country = Console.ReadLine();
        

        AddOrder(client.ShoppingCart, client, OrderStatus.BeingProcessed, DeliveryAddress);
    }
    // FinalizeOrder();
    //
    // Creaza o comanda cu toate produsele din cos

    public void CancelOrder(Client client)
    {
        List <Comanda> ComenziClient = new List <Comanda>();
        foreach (Comanda com in Comenzi)
        {
            if (com.Recipient.Equals (client))
            {
                ComenziClient.Add(com);
            }
        }
        if(ComenziClient.Count == 1)
        {
            ComenziClient[0].OrderStatus = OrderStatus.Cancelled;
        }
        else
        {
            Console.WriteLine ("Care comanda doriti sa o anulati?");
            int i=1;
            foreach(Comanda com in ComenziClient)
            {
                if(com.Status != Status.Cancelled)
                {
                    Console.WriteLine($"{i}. {com.ID}");
                    i++;
                }
            }
        }
    }
    // CancelOrder();


    public void AddToWishlist(Client client, Produs produs)
    {
        Console.WriteLine("Ce produs doriti sa adaugati la Wishlist?");
        string NumeProd = Console.ReadLine();
        foreach(Produs prod in StocMagazin)
        {
            if (prod.Name.Equals(NumeProd))
            {
                if (client.Wishlist.Contains(produs))
                {
                    Console.WriteLine("Produsul este deja in wishlist!");
                }
                else
                {
                    client.Wishlist.Add(produs);
                    Console.WriteLine("Produsul a fost adaugat in wishlist!");
                }
    }
            }
        }


    public void ViewWishlist(Client client)
    {
        int i = 1;
        foreach (var produs in client.Wishlist)
        {
            Console.WriteLine($"{i}. {produs.Name} - {produs.Price} RON");
            i++;
        }

        Console.WriteLine("Doriti sa adaugati un produs in cos? (da/nu)");
        string response = Console.ReadLine();
        if (response.ToLower() == "da")
        {
            Console.WriteLine("Introduceti numarul produsului pe care doriti sa-l adaugati in cos:");
            int productNumber = int.Parse(Console.ReadLine());
            if (productNumber > 0 && productNumber <= client.Wishlist.Count)
            {
                Produs selectedProduct = client.Wishlist[productNumber - 1];
                foreach(Produs prod in StocMagazin)
                {
                    if (prod.Name.Equals(selectedProduct))
                    {
                        client.ShoppingCart.Add(prod, 1);
                        break;
                    }
                }
                Console.WriteLine("Produsul a fost adaugat in cos.");
            }
            else
            {
                Console.WriteLine("Numar invalid.");
            }
        }
    }
    // din wishlist produsele trebuie sa se poata adauga in cart


    public void RemoveFromWishlist(Client client)
    {
        if (client.Wishlist.Any())
        {
            Console.WriteLine("Selectati produsul pe care doriti sa-l eliminati din wishlist:");
            for (int i = 0; i < client.Wishlist.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {client.Wishlist[i].Name} - {client.Wishlist[i].Price} RON");
            }

            int productNumber = int.Parse(Console.ReadLine());
            if (productNumber > 0 && productNumber <= client.Wishlist.Count)
            {
                Produs selectedProduct = client.Wishlist[productNumber - 1];
                client.Wishlist.Remove(selectedProduct);
                Console.WriteLine("Produsul a fost eliminat din wishlist!");
            }
            else
            {
                Console.WriteLine("Numar invalid.");
            }
        }
        else
        {
            Console.WriteLine("Wishlist-ul este gol.");
        }
    }


    public void RateProduct(Client client)
    {
        List<Produs> productsToRate = new List<Produs>();

        foreach (Comanda comanda in Comenzi)
        {
            if (comanda.Recipient.Equals(client) && comanda.Status == OrderStatus.Delivered)
            {
                foreach (var product in comanda.ProductsOrdered.Keys)
                {
                    if (!productsToRate.Contains(product))
                    {
                        productsToRate.Add(product);
                    }
                }
            }
        }

        if (productsToRate.Any())
        {
            Console.WriteLine("Selectati produsul pe care doriti sa-l evaluati:");
            for (int i = 0; i < productsToRate.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {productsToRate[i].Name}");
            }

            int productNumber = int.Parse(Console.ReadLine());
            if (productNumber > 0 && productNumber <= productsToRate.Count)
            {
                Produs selectedProduct = productsToRate[productNumber - 1];
                Console.WriteLine($"Introduceti rating-ul pentru produsul {selectedProduct.Name} (1-5):");
                int rating = int.Parse(Console.ReadLine());

                if (rating >= 1 && rating <= 5)
                {
                    selectedProduct.AddRating(rating);
                    Console.WriteLine("Rating-ul a fost adaugat cu succes!");
                }
                else
                {
                    Console.WriteLine("Rating invalid. Introduceti un numar intre 1 si 5.");
                }
            }
            else
            {
                Console.WriteLine("Numar invalid.");
            }
        }
        else
        {
            Console.WriteLine("Nu aveti produse livrate pentru a le evalua.");
        }
    }

    // valabil doar pentru produsele care au fost in comenzi asociate clientului
}
