using Microsoft.VisualBasic;
using


using System.Xml.Schema;
using System.Security.Cryptography.X509Certificates;

namespace ProiectPOO;
public enum UserTypes
{
    Client,
    Admin
}
public abstract class User
{
    public string FirstName { get; protected set; }
    public string LastName { get; protected set; }
    protected string Password { get; set; }
    public string EmailAddress { get; protected set; }
    public UserTypes UserType { get; protected set; }

    protected User(string firstName, string lastName, string password, string emailAddress, UserTypes userType)
    {
        FirstName = firstName;
        LastName = lastName;
        Password = password;
        EmailAddress = emailAddress;
        UserType = userType;
        
    }
    
    public bool VerifyUserCredentials(string emailToVerify, string passwordToVerify)
    {
        return Password == passwordToVerify && EmailAddress == emailToVerify; ;
    }

}

public class Client : User
{
    public Dictionary<Produs, int> ShoppingCart { get; protected set; }

    public Client(string firstName, string lastName, string password, string emailAddress)
        : base(firstName, lastName, password, emailAddress, UserTypes.Client)
    {
        ShoppingCart = new Dictionary<Produs, int>();
    }

    public void RunMenu()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("Meniu Client:");
            Console.WriteLine("1. Vizualizeaza produse");
            Console.WriteLine("2. Adauga in cos");
            Console.WriteLine("3. Modifica cosul");
            Console.WriteLine("4. Finalizeaza comanda");
            Console.WriteLine("5. Anuleaza o comanda");
            Console.WriteLine("6. Adauga in wishlist");
            Console.WriteLine("7. Afiseaza wishlist-ul");
            Console.WriteLine("8. Adauga rating unui produs");
            Console.WriteLine("9. Iesi");
            Console.Write("Alegeti optiunea: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    public void ViewProducts()
                    {
                        int i=0;
                        foreach (var kvp in ShoppingCart)
                        {
                            Console.WriteLine($"{i}. {kvp.Key}");
                            i++;
                        }

                    }
                    // ViewProducts();
                    //
                    // listeaza toate produsele, cu un contor care le numeroteaza in afisaj
                    // pentru a le putea selecta
                        break;

                case "2":
                public void AddToCart(Produs produs)
                {
                    Console.WriteLine("Ce produs doriti sa adaugati la cos?");
                    string NumeProd = Console.ReadLine();
                    foreach(Produs.Name in StocMagazin)
                    {
                        if (Produs.Name.Equals(NumeProd))
                        {
                            ShoppingCart.Add(produs, 1);
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
                        ShoppingCart.Add(produs, nr);
                    }
                }   
                    // AddToCart();
                    break;

                case "3":
                    public void EditCart()
                    {
                        if (ShoppingCart.Any())
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
                                        if (mod - 1 >= 0 && mod - 1 < ShoppingCart.Count)
                                        {
                                            ShoppingCart.RemoveAt(mod - 1);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Numar invalid");
                                        }
                                    }
                                    break;
                                case 2:
                                    void ModifyProdNr();
                                    {
                                        Console.WriteLine("Modificati numarul de produse la: ");
                                        int nr = int.Parse(Console.ReadLine());
                                        ShoppingCart[ShoppingCart.ElementAt(mod-1)] = nr;
                                    }
                                    break;
                            }

                        }
                        else
                        Console.WriteLine("Cosul de cumparaturi este gol - nu aveti ce sa modificati.");
                    }
                    // EditCart();
                    break;
                case "4":
                    public void FinalizeOrder()
                    {
                        Console.WriteLine("Introduceti Adressa: ");
                        DeliveryAdress.Address = Console.ReadLine();
                        Console.WriteLine("Introduceti codul postal");
                        DeliveryAddress.PostCode = Console.ReadLine();
                        Console.WriteLine("Introduceti orasul");
                        DeliveryAddress.City = Console.ReadLine();
                        Console.WriteLine("Introduceti tara");
                        DeliveryAddress.Country = Console.ReadLine();
                        

                        magazin.AddOrder(ShoppingCart, this.Client, OrderStatus.BeingProcessed, DeliveryAddress);
                    }
                    // FinalizeOrder();
                    //
                    // Creaza o comanda cu toate produsele din cos
                    break;
                case "5":
                    // CancelOrder();
                    break;
                case "6":
                    // AddToWishlist();
                    break;
                case "7":
                    // ViewWishlist();
                    //
                    // din wishlist produsele trebuie sa se poata adauga in cart
                    break;
                case "8":
                    // RateProduct();
                    //
                    // valabil doar pentru produsele care au fost in comenzi asociate clientului
                    // poate le listam si cerem alegerea pentru ce produs sa fie rate-uit?
                    break;
                case "9":
                    // Exit
                    running = false;
                    break;
                default:
                    Console.WriteLine("Optiune invalida. Incercati din nou.");
                    break;
            }
        }
    }

}

public class Admin : User
{
    public Admin(string firstName, string lastName, string password, string emailAddress)
        : base(firstName, lastName, password, emailAddress, UserTypes.Admin)
    {
    }

    public void RunMenu()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("Meniu Admin:");
            Console.WriteLine("1. Administrare magazin");
            Console.WriteLine("2. Administrare comenzi");
            Console.WriteLine("3. Genereaza raport de vanzari");
            Console.WriteLine("4. Iesi");
            Console.Write("Alegeti optiunea: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    // ManageStore();
                    // 
                    // functie multifunctionala pentru adaugat, editat si sters produse
                    // inclusiv administrat stocurile
                    // produsele cu stoc redus ar trebui semnalate cumva
                    // adaugatul de discounturi probabil vine tot aici
                    break;
                case "2":
                    // ManageOrders();
                    //
                    // pentru vizualizat, editat si modificat statusul comenzilor
                    break;
                case "3":
                    // CreateSalesReport();
                    //
                    // asta ar trebui sa ofere statistici
                    // precum sa treaca prin toate comenzile existente care nu au fost anulate
                    // si sa arate cele mai cumparate produse
                    // sau cifra totala de venituri
                    break;
                case "4":
                    // Exit
                    running = false;
                    break;
                default:
                    Console.WriteLine("Optiune invalida. Incercati din nou.");
                    break;
            }
        }
    }
}

