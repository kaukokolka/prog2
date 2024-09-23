// Developing a banking app

using System;

public class Bank
{
    public static void Main(string[] args)
    {
        Console.WriteLine ("Welcome to your Bank!");
        
        var client1 = new Client(1234, "Anna", "Berzina", "LV987654321");
        
        Console.WriteLine(client1.AccountNumber);
        client1.AccountNumber = "";
        Console.WriteLine(client1.AccountNumber);
        
    }
}

public class Client
{
    public static int numberOfClients = 0;
    
    private int _id;
    private string _name;
    private string _surname;
    private string _accountNumber;
    
    public string AccountNumber
    {
        get
        {
            return _accountNumber;
        }
        
        set
        {
            if(string.IsNullOrEmpty(value))
            {
                Console.WriteLine("Error, you cannot assign, empty value!");
            }
            else
            {
                _accountNumber = value;    
            }
        }
    }
    
    public Client(int id, string name, string surname, string accountNumber)
    {
        _id = id;
        _name = name;
        _surname = surname;
        _accountNumber = accountNumber;
    }
    
    public void RequestInfo()
    {
        Console.WriteLine($"{_id} {_name} {_surname} {_accountNumber}");
    }
    
    public static void Greetings()
    {
        Console.WriteLine("Hello, I am a Client class!");
    }
}
