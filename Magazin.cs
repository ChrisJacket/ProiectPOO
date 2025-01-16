namespace ProiectPOO;
using System;
using System.Collections.Generic;
using System.IO;
public class Magazin
{
    private List<Comanda> Comenzi { get; set; }
    public List<Produs> StocMagazin { get; private set; }
    public List<User> Users { get; private set; }

    public Magazin()
    {
        Comenzi = LoadOrdersFromFile();
        StocMagazin = LoadProductsFromFile();
        Users = LoadUsersFromFile();
    }
    
    public List<User> LoadUsersFromFile()
    {
        string filePath = "users.txt";
        List<User> aux_users = new List<User>();
        // Verificăm dacă fișierul există
        if (!File.Exists(filePath))
        {
            Console.WriteLine("Fișierul cu utilizatori nu a fost găsit.");
            return null;
        }

        try
        {
            var lines = File.ReadAllLines(filePath);
            
            foreach (var line in lines)
            {
                var userData = line.Split(',');

                // Asigurăm că avem suficiente date
                if (userData.Length == 5)
                {
                    string surname = userData[0].Trim();
                    string name = userData[1].Trim();
                    string password = userData[2].Trim();
                    string email = userData[3].Trim();
                    string userTypeString = userData[4].Trim();

                    // Încercăm să convertim userTypeString în UserTypes
                    UserTypes userType;
                    if (Enum.TryParse(userTypeString, true, out userType))
                    {
                        // Creăm un utilizator de tipul corespunzător
                        User user;
                        if (userType == UserTypes.Admin)
                        {
                            user = new Admin(surname, name, password, email);
                        }
                        else
                        {
                            user = new Client(surname, name, password, email);
                        }

                        // Adăugăm utilizatorul în lista de utilizatori
                        aux_users.Add(user);
                    }
                    else
                    {
                        Console.WriteLine($"Tip utilizator invalid: {userTypeString}. Ignorăm această linie.");
                    }
                }
            }

            Console.WriteLine("Utilizatorii au fost încărcați cu succes.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului de utilizatori: {ex.Message}");
        }
        return aux_users;
    }
    
    public List<Comanda> LoadOrdersFromFile()
    {
        List<Comanda> OrdersToBeLoaded = new List<Comanda>();

        try
        {
            string filePath = "orders.txt";

            // Verificăm dacă fișierul există, dacă nu, îl creăm
            if (!File.Exists(filePath))
            {
                // Creăm fișierul gol
                File.Create(filePath).Dispose();
                Console.WriteLine($"Fișierul {filePath} nu există. A fost creat.");
            }

            var lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                var orderData = line.Split(',');

                if (orderData.Length < 9)
                {
                    Console.WriteLine("Linie incorectă în fișierul de comenzi: " + line);
                    continue;
                }

                string orderId = orderData[0];
                DateOnly orderDate = DateOnly.Parse(orderData[1]);
                OrderStatus orderStatus = Enum.Parse<OrderStatus>(orderData[2]);
                string customerName = orderData[3];
                string customerEmail = orderData[4];
                string city = orderData[5];
                string country = orderData[6];
                string postcode = orderData[7];
                
                ShippingAddress address = new ShippingAddress
                {
                    City = city,
                    Country = country,
                    Postcode = postcode,
                    Address = $"{orderData[5]} {orderData[6]}"
                };

                // Extragem produsele comandate
                Dictionary<Produs, int> productsOrdered = new Dictionary<Produs, int>();
                
                for (int i = 8; i < orderData.Length; i += 2)
                {
                    string productName = orderData[i];
                    int quantity = int.Parse(orderData[i + 1]);

                    // Vom crea un produs cu un preț fictiv, deoarece nu avem prețul real în fișier
                    // În mod normal, ar trebui să obținem produsul dintr-o bază de date sau să-l găsim într-o listă preexistentă
                    Produs product = new Produs(productName, productName, 10.0, 100, ProductCategory.Auto); // Exemplu de produs cu preț de 10.0
                    productsOrdered.Add(product, quantity);
                }

                // Creăm comanda
                Comanda order = new Comanda(productsOrdered, orderId, new Client(customerName, customerName, "password", customerEmail), orderStatus, address);
                OrdersToBeLoaded.Add(order);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea fișierului de comenzi: {ex.Message}");
        }

        return OrdersToBeLoaded;
    }
    public List<Produs> LoadProductsFromFile()
    {
        string filePath = "products.txt";
        List<Produs> products = new List<Produs>();
        
        try
        {
            if (File.Exists(filePath))
            {
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    var fields = line.Split('|');
                    string id = fields[0];
                    string name = fields[1];
                    double price = double.Parse(fields[2]);
                    string description = fields[3];
                    int stock = int.Parse(fields[4]);
                    ProductCategory category = Enum.Parse<ProductCategory>(fields[5]);

                    Produs product = new Produs(id, name, price, stock, category);

                    // Aplica reducerile
                    if (!string.IsNullOrEmpty(fields[6]))
                    {
                        var discounts = fields[6].Split(',');
                        foreach (var discount in discounts)
                        {
                            DiscountTypes discountType = Enum.Parse<DiscountTypes>(discount);
                            int discountValue = 0;

                            if (discountType == DiscountTypes.Percentage)
                                discountValue = int.Parse(fields[7]);
                            else if (discountType == DiscountTypes.Constant)
                                discountValue = int.Parse(fields[8]);

                            product.AddDiscount(discountType, discountValue);
                        }
                    }

                    products.Add(product);
                }
            }
            else
            {
                Console.WriteLine("Fișierul de produse nu există. Se va crea unul nou la prima salvare.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Eroare la citirea produselor: {ex.Message}");
        }

        return products;
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
        Comenzi.Add(NewOrder);
    }
    private bool ProductAlreadyExists(Admin admin, Produs produs)
    {
        foreach (Produs produs_aux in StocMagazin)
        {
            if (produs.Name == produs_aux.Name)
            {
                produs_aux.AddStock(admin, produs.Stock);
                return true;
            }
            // aici poate putem face o chestie, daca produsul deja exista
            // astfel incat sa adauge stocul produsului nou la produsul care deja exista
        }
        return false;
    }
    public void EditOrder(Comanda comanda)
    {
        foreach (Comanda comanda_aux in Comenzi)
        {
            if (comanda_aux.ID == comanda.ID)
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
        return Users.FirstOrDefault(u => u.VerifyUserCredentials(email, parola), null);
    }

    public void ManageStore(Admin ThisAdmin)
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("1. Adauga produs in magazin");
            Console.WriteLine("2. Modifica produs");
            Console.WriteLine("3. Sterge produs");
            Console.WriteLine("4. Actualizeaza stocurile unui produs");
            Console.WriteLine("5. Adauga discount unui produs");
            if (ProductsWithLowStockExist() == true)
                Console.WriteLine("6. Afiseaza produsele cu stoc scazut (IMPORTANT)");
            Console.WriteLine("0. Iesire");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    AddProduct(ThisAdmin);
                    break;

                case "2":
                    ModifyProduct(ThisAdmin);
                    break;

                case "3":
                    DeleteProduct(ThisAdmin);
                    break;

                case "4":
                    UpdateProductStock(ThisAdmin);
                    break;

                case "5":
                    AddProductDiscount(ThisAdmin);
                    break;

                case "6":
                    if (ProductsWithLowStockExist() == true)
                        ShowProductsWithLowStock();
                    else
                        Console.WriteLine("There are no products with low stock!");
                    break;
                case "0":
                    running = false;
                    break;

            }

        }
    }
    private void AddProduct(Admin admin)
    {
        string ProposedProductName;
        double ProposedProductPrice;
        int ProposedProductInitialStock;
        int ProposedProductCategory;
        bool conversion1, conversion2;
        while (true)
        {
            Console.WriteLine("Numele produsului:");
            ProposedProductName = Console.ReadLine();

            Console.WriteLine("Pretul produsului:");
            conversion1 = double.TryParse(Console.ReadLine(), out ProposedProductPrice);

            Console.WriteLine("Stocul initial al produsului:");
            conversion2 = int.TryParse(Console.ReadLine(), out ProposedProductInitialStock);

            Console.WriteLine("Categoria produsului:");
            int.TryParse(Console.ReadLine(), out ProposedProductCategory);

            if (!conversion1 || !conversion2)
                Console.WriteLine("Una sau mai multe valori nu a putut fi citita corect. Incercati din nou.");
            else if (ProposedProductName == null)
                Console.WriteLine("Produsul trebuie sa aiba un nume!");
            else
                break;
        }

        Produs NewProduct = new Produs(GenerateNextProductId(), ProposedProductName, ProposedProductPrice, ProposedProductInitialStock, (ProductCategory)ProposedProductCategory);
        if (ProductAlreadyExists(admin, NewProduct))
        {
            Console.WriteLine("Exista deja un produs cu acelasi nume! S-a adaugat stocul produsului de la intrare la produsul deja existent.");
        }
        else
            StocMagazin.Add(NewProduct);
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

    private Produs? FindProduct()
    {
        string StringToSearch;
        Produs? ProductToFind = null;
        while (true)
        {
            Console.WriteLine("ID-ul sau numele produsului cautat:");
            StringToSearch = Console.ReadLine();

            if (StringToSearch != null)
                break;
        }

        foreach (Produs produs in StocMagazin)
        {
            if (produs.ID == StringToSearch || produs.Name == StringToSearch)
            {
                ProductToFind = produs;
                break;
            }
        }

        if (ProductToFind == null)
        {
            Console.WriteLine("Produsul cu ID-ul sau numele specificat nu a fost gasit.");
            return null;
        }

        return ProductToFind;
    }

    private void ModifyProduct(Admin admin)
    {

        Produs? ProductToModify = FindProduct();
        if (ProductToModify == null)
            return;

        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce doriti sa modificati la produs?");
            Console.WriteLine("1. Numele");
            Console.WriteLine("2. Pretul");
            Console.WriteLine("3. Descrierea");
            Console.WriteLine("4. Iesire");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    Console.WriteLine("Introduceti noul nume pentru produs:");
                    string NewProductName = Console.ReadLine();
                    ProductToModify.ModifyName(admin, NewProductName);
                    break;

                case "2":
                    Console.WriteLine("Introduceti noul pret al produsului:");
                    int.TryParse(Console.ReadLine(), out int NewProductPrice);
                    ProductToModify.ModifyPrice(admin, NewProductPrice);
                    break;

                case "3":
                    Console.WriteLine("Introduceti noua descriere a produsului:");
                    string NewProductDescription = Console.ReadLine();
                    ProductToModify.ModifyDescription(admin, NewProductDescription);
                    break;

                case "4":
                    running = false;
                    break;

                default:
                    Console.WriteLine("Alegere invalida!");

                    break;
            }
        }
    }

    private void DeleteProduct(Admin admin)
    {
        Produs? ProductToDelete = FindProduct();

        if (ProductToDelete != null)
        {
            StocMagazin.Remove(ProductToDelete);
            Console.WriteLine($"Produsul {ProductToDelete.Name} cu ID-ul {ProductToDelete.ID} a fost sters!");
        }
    }

    private void UpdateProductStock(Admin admin)
    {
        Produs? ProductToUpdate = FindProduct();
        if (ProductToUpdate == null)
            return;

        Console.WriteLine("Introduceti noua valoarea a stocului produsului:");
        int.TryParse(Console.ReadLine(), out int NewStock);

        if (ProductToUpdate != null)
            ProductToUpdate.ModifyStock(admin, NewStock);
    }

    private void AddProductDiscount(Admin admin)
    {
        Produs? ProductToDiscount = FindProduct();
        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce tip de reducere doriti sa adaugati produsului?");
            Console.WriteLine("1. Oferta 2 + 1 gratis");
            Console.WriteLine("2. Procentaj din pretul actual");
            Console.WriteLine("3. Valoare fixa");
            Console.WriteLine("4. Anulare");
            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    ProductToDiscount.AddDiscount(DiscountTypes.TwoPlusOne, 0);
                    Console.WriteLine($"Oferta 2 + 1 gratis activata pentru produsul {ProductToDiscount.Name}, ID: {ProductToDiscount.ID}");
                    break;

                case "2":
                    int PercentageToDiscount;
                    Console.WriteLine("Ce procentaj din pretul produsului vreti sa fie redus?");
                    int.TryParse(Console.ReadLine(), out PercentageToDiscount);
                    ProductToDiscount.AddDiscount(DiscountTypes.Percentage, PercentageToDiscount);
                    break;

                case "3":
                    int FlatValueToDiscount;
                    Console.WriteLine("Ce valoare vreti sa fie redusa din pretul produsului?");
                    int.TryParse(Console.ReadLine(), out FlatValueToDiscount);
                    ProductToDiscount.AddDiscount(DiscountTypes.Constant, FlatValueToDiscount);
                    break;

                case "4":
                    running = false;
                    // exit
                    break;
            }
        }
    }
    private bool ProductsWithLowStockExist()
    {
        bool LowStockProductsExist = false;
        foreach (Produs produs in StocMagazin)
        {
            if (produs.Stock < 10)
            {
                LowStockProductsExist = true;
            }
        }
        return LowStockProductsExist;
    }

    private void ShowProductsWithLowStock()
    {
        foreach (Produs produs in StocMagazin)
        {
            if (produs.Stock < 10)
            {
                Console.WriteLine($"Produsul {produs.Name}, ID: {produs.ID} are stoc redus: {produs.Stock} bucati ramase");
            }
        }
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    public void ManageOrders(Admin admin)
    {
        bool running = true;
        while (running)
        {
            Console.WriteLine("Ce actiune doriti sa aplicati asupra comenzilor?");
            Console.WriteLine("1. Vizualizare");
            Console.WriteLine("2. Editare");
            Console.WriteLine("3. Niciuna - iesire");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    foreach (Comanda comanda in Comenzi)
                    {
                        int count = 1;
                        Console.WriteLine($"{count}. ID: {comanda.ID}\n " +
                            $"Produse comandate: {comanda.ProductsOrdered.Keys}, valoare totala {comanda.OrderPrice}\n " +
                            $"Plasata la data {comanda.PlacementDate}, status: {comanda.Status}");
                        count ++;
                    }
                    break;

                case "2":
                    bool localRunning = true;
                    string IdToSearch;
                    Comanda? OrderToFind = null;
                    while (true)
                    {
                        Console.WriteLine("ID-ul comenzii cautate:");
                        IdToSearch = Console.ReadLine();

                        if (IdToSearch != null)
                            break;
                    }

                    foreach (Comanda comanda in Comenzi)
                    {
                        if (comanda.ID == IdToSearch)
                        {
                            OrderToFind = comanda;
                            break;
                        }
                    }

                    if (OrderToFind == null)
                    {
                        Console.WriteLine("Comanda cu ID-ul specificat nu a fost gasita.");
                        return;
                    }

                    if (OrderToFind.Status == OrderStatus.Canceled)
                    {
                        Console.WriteLine("Aceasta comanda nu mai poate fi modificata, deoarece a fost anulata");
                    }
                    while (localRunning) { 
                    Console.WriteLine($"Statusul curent al comenzii este {OrderToFind.Status}");
                    Console.WriteLine("Ce status doriti sa ii dati?");
                    Console.WriteLine("1. Being processed");
                    Console.WriteLine("2. Sent");
                    Console.WriteLine("3. Delivered");
                    Console.WriteLine("4. Niciunul - anulare");
                    var StatusInput = Console.ReadLine();
                        switch (StatusInput)
                        {
                            case "1":
                                if (OrderToFind.Status == OrderStatus.BeingProcessed)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                    OrderToFind.UpdateStatus(admin, OrderStatus.BeingProcessed);
                                break;

                            case "2":
                                if (OrderToFind.Status == OrderStatus.Sent)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                    OrderToFind.UpdateStatus(admin, OrderStatus.Sent);
                                break;
                            case "3":
                                if (OrderToFind.Status == OrderStatus.Delivered)
                                    Console.WriteLine("Comanda are deja acest status!");
                                else
                                {
                                    OrderToFind.UpdateStatus(admin, OrderStatus.Delivered);
                                    OrderToFind.SetDeliveryDate();
                                }
                                break;
                            case "4":
                                // exit
                                break;
                        }
                    }
                break;
            }
        }
    }
    
    //Adăugare clienti dacă nu sunt deja în fișier
    public void SaveUsersToFile()
    {
        string filePath = "./users.txt";
        foreach (User user in Users)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    // Formatează linia pentru fișier
                    string userLine =
                        $"{user.LastName},{user.FirstName},{user.Password},{user.EmailAddress},{user.UserType}";
                    sw.WriteLine(userLine);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la scrierea în fișier: {ex.Message}");
            }
            
        }
        Console.WriteLine("Utilizatorii a fost adaugati in fisier.");
    }
    public void SaveOrdersToFile()
    {
        string filePath = "./orders.txt";
        File.WriteAllText(filePath, string.Empty);
        foreach (Comanda comanda in Comenzi)
        {
            try
            {
                // Construim manual linia pentru fișier
                string products = string.Join(";", comanda.ProductsOrdered.Select(p => $"{p.Key.ID}:{p.Value}"));
                string address = $"{comanda.DeliveryAddress.City},{comanda.DeliveryAddress.Country},{comanda.DeliveryAddress.Postcode},{comanda.DeliveryAddress.Address}";

                string line = $"{comanda.ID}|{comanda.PlacementDate}|{comanda.Recipient.EmailAddress}|{comanda.Status}|{address}|{comanda.OrderPrice}|{products}";

                // Adăugăm linia în fișier
                File.AppendAllText(filePath, line + Environment.NewLine);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la salvarea comenzii: {ex.Message}");
            }
        }
        Console.WriteLine("Comenzile au fost salvate cu succes in fisier.");
    }
    public void SaveProductsToFile()
    {
        string filePath = "./products.txt";
        File.WriteAllText(filePath, string.Empty);
        foreach (Produs produs in StocMagazin)
        {
            try
            {
                // Scrie produsul în format text în fișier
                File.AppendAllText(filePath, produs.ToFileFormat() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la salvarea produsului: {ex.Message}");
            }
        }
        Console.WriteLine("Produsele au fost salvate cu succes in fisier.");
    }

    public void CreateSalesReport(Admin admin)
    {
        double TotalSalesFigure = 0, LastMonthSalesFigure = 0;
        int CanceledOrders = 0;
        Dictionary<Produs, int> ProductPopularityHashmap = new Dictionary<Produs, int>();

        foreach(Comanda comanda in Comenzi)
        {
            TotalSalesFigure += comanda.OrderPrice;
            if (comanda.PlacementDate > DateOnly.FromDateTime(DateTime.Now.AddDays(-30)) && comanda.Status != OrderStatus.Canceled)
                LastMonthSalesFigure += comanda.OrderPrice;

            if (comanda.Status == OrderStatus.Canceled)
                CanceledOrders++;

            foreach(var ProductIntPair in comanda.ProductsOrdered)
            {
                if (ProductPopularityHashmap.ContainsKey(ProductIntPair.Key))
                    ProductPopularityHashmap[ProductIntPair.Key] += ProductIntPair.Value;
                else
                    ProductPopularityHashmap.Add(ProductIntPair.Key, ProductIntPair.Value);
            }

        }
        Console.WriteLine($"Suma totala de vanzari a magazinului este {TotalSalesFigure}.\n");
        Console.WriteLine($"Magazinul a avut {Comenzi.Count} comenzi, dintre care doar {CanceledOrders} anulate.\n");
        Console.WriteLine($"Valoarea medie a unei comenzi este de {TotalSalesFigure / Comenzi.Count}.\n");
        Console.WriteLine($"Doar in ultimele 30 de zile, valoarea comenzilor a fost de {LastMonthSalesFigure}.\n");

        int MaxValue = ProductPopularityHashmap.Values.Max();

        if ( MaxValue > 0 )
            Console.WriteLine($"Cele mai vandute produse, cu vanzarea totala de {MaxValue} bucati:\n");
        foreach (var kvp in ProductPopularityHashmap)
        {
            if (kvp.Value == MaxValue && MaxValue != 0 )
                Console.WriteLine($"{kvp.Key.Name}, ID: {kvp.Key.ID}");
        }
            
    }

}
