namespace ProiectPOO;
internal class Program
{
    static void Main(string[] args)
    {
        Magazin magazin = new Magazin();
        RunApplication(magazin);
    }

    public static async void RunApplication(Magazin magazin)
    {
        // rutina pentru incarcarea datelor magazinului
        magazin.LoadUsersFromFile();
        magazin.LoadProductsFromFile();
        magazin.LoadOrdersFromFile();

        Console.WriteLine("Bun venit! Va rugam sa va autentificati");
        while (true)
        {
            User LogInUser = AuthenticateUser(magazin);

            // logica cred ca va fi asa: securitatea va sta in CREAREA unei instante de tip client sau admin
            // deci logica niciuneia din cele doua clase nu va putea avea loc fara ca una din ele sa existe
            // si ne folosim de meniuri pentru a autentifica / crea un cont de fie admin sau client

            if (LogInUser != null)
            {
                Console.WriteLine($"Bun venit, {LogInUser.FirstName}!");

                if (LogInUser.UserType == UserTypes.Admin)
                {
                    Admin currentUser = new Admin(LogInUser.FirstName, LogInUser.LastName, LogInUser.Password, LogInUser.EmailAddress);
                    currentUser.RunMenu(); // in functie de tipul utilizatorului, castez User-ul generic la Admin sau Client
                }
                else if (LogInUser.UserType == UserTypes.Client)
                {
                    Client currentUser = new Client(LogInUser.FirstName, LogInUser.LastName, LogInUser.Password, LogInUser.EmailAddress);
                    currentUser.RunMenu();
                }
            }

            else
            {
                Console.WriteLine("Autentificare esuata. Incercati din nou.");
                await Task.Delay(1000); // asteapta putin inainte sa stearga consola
                Console.Clear();

            }
        }
    }
    public static User AuthenticateUser(Magazin magazin)
    {
        string email, parola;
        while(true)
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
        return magazin.Users.First(u => u.Autentificare(email, parola));
    }

}