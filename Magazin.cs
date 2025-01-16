﻿namespace ProiectPOO;

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
        return Users.First(u => u.VerifyUserCredentials(email, parola));
    }

    public void ManageStore(Admin ThisAdmin)
    {
        while (true)
        {
            Console.WriteLine("1. Adauga produs in magazin");
            Console.WriteLine("2. Modifica produs");
            Console.WriteLine("3. Sterge produs");
            Console.WriteLine("4. Actualizeaza stocurile unui produs");
            Console.WriteLine("5. Adauga discount unui produs");
            if (ProductsWithLowStockExist() == true)
                Console.WriteLine("6. Afiseaza produsele cu stoc scazut (IMPORTANT)");

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

    private Produs? FindProduct()
    {
        string IdToSearch;
        Produs? ProductToFind = null;
        while (true)
        {
            Console.WriteLine("ID-ul produsului cautat:");
            IdToSearch = Console.ReadLine();

            if (IdToSearch != null)
                break;
        }

        foreach (Produs produs in StocMagazin)
        {
            if (produs.ID == IdToSearch)
            {
                ProductToFind = produs;
                break;
            }
        }

        if (ProductToFind == null)
        {
            Console.WriteLine("Produsul cu ID-ul specificat nu a fost gasit.");
            return null;
        }

        return ProductToFind;
    }

    private void ModifyProduct(Admin admin)
    {

        Produs? ProductToModify = FindProduct();

        while (true)
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
                    // exit
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

        Console.WriteLine("Introduceti noua valoarea a stocului produsului:");
        int.TryParse(Console.ReadLine(), out int NewStock);

        if (ProductToUpdate != null)
            ProductToUpdate.ModifyStock(admin, NewStock);
    }

    private void AddProductDiscount(Admin admin)
    {
        Produs? ProductToDiscount = FindProduct();
        while (true)
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
    }

    public void ManageOrders(Admin admin)
    {
        while (true)
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
                        count += 1;
                    }
                    break;

                case "2":
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
                break;
            }


        }
    }
}
