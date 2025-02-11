﻿using Microsoft.VisualBasic;
using System.Linq;


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
    protected internal string Password { get; set; }
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
    public List<Produs> Wishlist { get; protected set; }

    public Client(string firstName, string lastName, string password, string emailAddress)
        : base(firstName, lastName, password, emailAddress, UserTypes.Client)
    {
        ShoppingCart = new Dictionary<Produs, int>();
        Wishlist = new List<Produs>();
    }

    public void RunMenu(Client ThisClient, Magazin magazin)
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
            Console.WriteLine("8. Sterge din Wishlist");
            Console.WriteLine("9. Adauga rating unui produs");
            Console.WriteLine("10. Iesi");
            Console.Write("Alegeti optiunea: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    magazin.ViewProducts(ThisClient);
                    break;

                case "2":
                    magazin.AddToCart(ThisClient);
                    break;

                case "3":
                    magazin.EditCart(ThisClient);
                    break;

                case "4":
                    magazin.FinalizeOrder(ThisClient);
                    break;

                case "5":
                    magazin.CancelOrder(ThisClient);
                    break;

                case "6":
                    magazin.AddToWishlist(ThisClient);
                    break;

                case "7":
                    magazin.ViewWishlist(ThisClient);
                    break;

                case "8":
                    magazin.RemoveFromWishlist(ThisClient);
                    break;

                case "9":
                    magazin.RateProduct(ThisClient);
                    break;

                case "10":
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

    public void RunMenu(Admin ThisAdmin, Magazin magazin)
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
                    magazin.ManageStore(ThisAdmin);
                    
                    // functie multifunctionala pentru adaugat, editat si sters produse
                    // inclusiv administrat stocurile
                    // produsele cu stoc redus sunt semnalate
                    // adaugatul de discounturi vine tot aici
                    break;
                case "2":
                    magazin.ManageOrders(ThisAdmin);
                    
                    // pentru vizualizat si modificat statusul comenzilor
                    break;
                case "3":
                    magazin.CreateSalesReport(ThisAdmin);
                    
                    // ofera statistici
                    // precum cifra de afaceri, numarul de comenzi, cele mai vandute produse
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

