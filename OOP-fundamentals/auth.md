```cs
public class Authenticate
{
    private Dictionary<int, string> _clientCredentials = new Dictionary<int, string>();

    // Register a client with an ID and hashed password
    public void Register(int clientId, string password)
    {
        if (!_clientCredentials.ContainsKey(clientId))
        {
            string hashedPassword = HashPassword(password);
            _clientCredentials.Add(clientId, hashedPassword);
        }
        else
        {
            Console.WriteLine("Client already registered.");
        }
    }

    // Authenticate a client by checking the hashed password
    public bool Login(int clientId, string password)
    {
        if (_clientCredentials.ContainsKey(clientId))
        {
            string hashedPassword = HashPassword(password);
            return _clientCredentials[clientId] == hashedPassword;
        }
        else
        {
            Console.WriteLine("Client not found.");
            return false;
        }
    }

    // Hashing the password using SHA-256
    private string HashPassword(string password)
    {
        using (SHA256 sha256Hash = SHA256.Create())
        {
            // Convert the input string to a byte array and compute the hash
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            // Convert the byte array to a string
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                builder.Append(bytes[i].ToString("x2")); // Converts to hexadecimal
            }
            return builder.ToString();
        }
    }
}
```

```cs
public class Client
{
    public static int numberOfClients = 0;

    protected int _id;
    protected string _name;
    protected string _surname;
    private List<Account> _accountList = new List<Account>();

    public Client(int id, string name, string surname)
    {
        _id = id;
        _name = name;
        _surname = surname;
    }

    // Call the centralized authentication system to register a new client
    public void Register(Authenticate auth, string password)
    {
        auth.Register(_id, password);
    }

    // Authenticate action before opening an account
    public void AddAccount(Account account, Authenticate auth, string password)
    {
        if (auth.Login(_id, password))
        {
            _accountList.Add(account);
            Console.WriteLine("Account added successfully.");
        }
        else
        {
            Console.WriteLine("Authentication failed! Cannot add account.");
        }
    }

    public void PrintAccounts()
    {
        Console.WriteLine($"Client {_name} {_surname} has the following accounts:");
        foreach (var account in _accountList)
        {
            Console.WriteLine($"Number: {account.AccountNumber} ({account.AccountCurrency})");
            account.PrintTransactions();
        }
    }

    public virtual void PrintClientType()
    {
        Console.WriteLine($"This is Client type object");
    }
}
```

```cs
public class Bank
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Welcome to your Bank!");

        // Create a single centralized instance of the authentication system
        var authSystem = new Authenticate();

        // Create clients
        var client1 = new LocalClient(1234, "Anna", "Berzina", "Talsi");
        var client2 = new ForeignClient(1235, "Oskars", "Andersons", "UK");

        // Register clients with the authentication system
        client1.Register(authSystem, "password123");
        client2.Register(authSystem, "securePassword");

        client1.RequestInfo();
        client1.PrintClientType();
        client2.RequestInfo();
        client2.PrintClientType();

        // Authenticate and add accounts (hashed password in use)
        client1.AddAccount(new Account("LV1234567899876", "EUR"), authSystem, "password123"); // Correct password
        client1.AddAccount(new Account("US1234567899875", "USD"), authSystem, "wrongPassword"); // Wrong password

        client1.PrintAccounts();
        client2.PrintAccounts();
    }
}
```
