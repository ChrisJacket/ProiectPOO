namespace ProiectPOO;

internal class Program
{
    static void Main(string[] args)
    {
        Magazin magazin = new Magazin();
        StartApp(magazin);
    }

    public static async void StartApp(Magazin magazin)
    {
        // rutina pentru incarcarea datelor magazinului
        magazin.LoadUsersFromFile();
        magazin.LoadProductsFromFile();
        magazin.LoadOrdersFromFile();

        Console.WriteLine("Bun venit! Va rugam sa va autentificati");
        while (true)
        {
            User LogInUser = magazin.AuthenticateUser();

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
                    currentUser.RunMenu();
                }
                else if (LogInUser.UserType == UserTypes.Client)
                {
                    Client currentUser = (Client)LogInUser;
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
}