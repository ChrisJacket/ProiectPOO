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
    public Client(string firstName, string lastName, string password, string emailAddress)
        : base(firstName, lastName, password, emailAddress, UserTypes.Client)
    {
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
            Console.WriteLine("3. Finalizeaza comanda");
            Console.WriteLine("4. Vizualizeaza wishlist");
            Console.WriteLine("5. Iesi");
            Console.Write("Alegeti optiunea: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    // ViewProducts();
                    break;
                case "2":
                    // AddToCart();
                    break;
                case "3":
                    // FinalizeOrder();
                    break;
                case "4":
                    // ViewWishlist();
                    break;
                case "5":
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
            Console.WriteLine("1. Adauga produs");
            Console.WriteLine("2. Modifica produs");
            Console.WriteLine("3. Sterge produs");
            Console.WriteLine("4. Vizualizeaza comenzi");
            Console.WriteLine("5. Genereaza raport de vanzari");
            Console.WriteLine("6. Iesi");
            Console.Write("Alegeti optiunea: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    // AddProduct();
                    break;
                case "2":
                    // EditProduct();
                    break;
                case "3":
                    // DeleteProduct();
                    break;
                case "4":
                    throw new NotImplementedException();
                    break;
                case "5":
                    throw new NotImplementedException();
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Optiune invalida. Incercati din nou.");
                    break;
            }
        }
    }
}

