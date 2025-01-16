using System.Runtime.ConstrainedExecution;
namespace ProiectPOO;

internal class Program
{
    static void Main(string[] args)
    {
        Magazin magazin = new Magazin();
        magazin.Users.Add(new Admin("ion", "palasca", "admin", "admin"));
        StartApp(magazin);
    }

    public static void StartApp(Magazin magazin)
    {
        // rutina pentru incarcarea datelor magazinului
        magazin.LoadUsersFromFile();
        magazin.LoadProductsFromFile();
        magazin.LoadOrdersFromFile();

        bool ProgramRunning = true;
        while (ProgramRunning)
        {
            bool LogInRunning = true;
            while (LogInRunning)
            {
                Console.Clear();
                Console.WriteLine("Bun venit! Aveti deja cont sau doriti sa creati unul?");
                Console.WriteLine("1. Autentificare");
                Console.WriteLine("2. Cont nou");
                Console.WriteLine("0. Iesire");
                var input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        LogInRunning = false;
                        break;
                    case "2":
                        SignUp(magazin);
                        break;
                    case "0":
                        magazin.SaveUsersToFile();
                        magazin.SaveProductsToFile();
                        magazin.SaveOrdersToFile();
                        Environment.Exit(0);
                        break;

                }
            }
            while (true)
            {
                User? LogInUser = magazin.AuthenticateUser();

                // logica cred ca va fi asa: securitatea va sta in CREAREA unei instante de tip client sau admin
                // deci logica niciuneia din cele doua clase nu va putea avea loc fara ca una din ele sa existe
                // si ne folosim de meniuri pentru a autentifica / crea un cont de fie admin sau client

                if (LogInUser != null)
                {
                    Console.WriteLine($"Bun venit, {LogInUser.FirstName}!");

                    // in functie de tipul utilizatorului, castez la Admin sau Client si rulez meniul specific

                    if (LogInUser.UserType == UserTypes.Admin)
                    {
                        Admin currentUser = (Admin)LogInUser;
                        currentUser.RunMenu(currentUser, magazin);
                        break;
                    }
                    else if (LogInUser.UserType == UserTypes.Client)
                    {
                        Client currentUser = (Client)LogInUser;
                        currentUser.RunMenu(currentUser, magazin);
                        break;
                    }
                }
                else
                    Console.WriteLine("Autentificare esuata. Incercati din nou.");

            }
        }
    }

    public static void SignUp(Magazin magazin)
    {
        string SignUpLastName, SignUpFirstName, SignUpEmail, SignUpPassword;
        bool FailedAttempt = false;
        do
        {
            if (FailedAttempt)
            {
                Console.WriteLine("A aparut o eroare. Asigurati-va ca nu lasati niciun camp gol si incercati din nou");
                Thread.Sleep(2000);
                Console.Clear();
            }

            Console.WriteLine("Introduceti-va numele de familie:");
            SignUpLastName = Console.ReadLine();
            Console.WriteLine("Introduceti-va prenumele");
            SignUpFirstName = Console.ReadLine();
            Console.WriteLine("Introduceti-va adresa de email:");
            SignUpEmail = Console.ReadLine();
            Console.WriteLine("Introduceti parola dorita. Asigurati-va ca este una sigura!");
            SignUpPassword = Console.ReadLine();

            if (!EmailAvailable(magazin, SignUpEmail))
                Console.WriteLine("Deja exista un cont cu acest email!");

            FailedAttempt = true;
        } while (SignUpLastName == null || SignUpFirstName == null || SignUpEmail == null || SignUpPassword == null || EmailAvailable(magazin, SignUpEmail) == false);

        Client ClientNou = new Client(SignUpFirstName, SignUpLastName, SignUpPassword, SignUpEmail);
        magazin.SignUpClient(ClientNou);
    }

    private static bool EmailAvailable(Magazin magazin, string ProposedEmail)
    {
        if (ProposedEmail == null)
            return false;

        foreach(User user in magazin.Users)
        {
            if (user.EmailAddress == ProposedEmail)
            {
                return false;
            }
        }
        return true;
    }
}