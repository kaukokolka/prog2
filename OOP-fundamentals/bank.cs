using System;
using System.Collections.Generic;

public class Bank
{
    public static void Main(string[] args)
    {
        Console.WriteLine ("Welcome to your Bank!");

        var client1 = new Client(1234, "Anna", "Berzina");
		var client2 = new Client(1235, "Oskars", "Andersons");

		client1.AddAccount(new Account("LV1234567899876", "EUR"));
		client1.AddAccount(new Account("US1234567899875", "USD"));

		client1.AccountList[1].Deposit(300);
		client1.AccountList[0].Deposit(1200);
		client1.AccountList[0].Withdraw(6);
		client1.AccountList[0].Withdraw(45);
		client1.AccountList[0].Withdraw(270);
		client1.AccountList[0].Withdraw(2000);

		client2.AddAccount(new Account("LV1234567899147", "USD"));
		client2.AddAccount(new Account("LV1234567899147", "JPY"));

		var transfer = new Transfer(client1.AccountList[1], client2.AccountList[0], 300);
		//transfer.Execute();

		client1.PrintAccounts();
		client2.PrintAccounts();
    }
}

public class Client
{
    public static int numberOfClients = 0;

    private int _id;
    private string _name;
    private string _surname;
	private List<Account> _accountList = new List<Account>();

	public List<Account> AccountList
	{
		get
		{
			return _accountList;
		}
	}


    public Client(int id, string name, string surname)
    {
        _id = id;
        _name = name;
        _surname = surname;
    }

    public void RequestInfo()
    {
        Console.WriteLine($"{_id} {_name} {_surname}");
    }

	public void AddAccount(Account account)
	{
		_accountList.Add(account);
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
}


public class Account
{
	private string _accountNumber;
	private string _accountCurrency;
	private List<Transaction> _transactionList = new List<Transaction>();

	public string AccountCurrency
	{
		get
			{
				return _accountCurrency;
			}
	}


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
			else if(value.Length != 15)
			{
				Console.WriteLine("Error: The account number must be exactly 15 characters long!");
			}
            else
            {
                _accountNumber = value;
            }
        }
    }


	public Account(string accountNumber, string accountCurrency)
	{
		AccountNumber = accountNumber;
		_accountCurrency = accountCurrency;
	}

	public void Deposit(double amount)
	{
		_transactionList.Add(new Transaction(amount, "deposit"));
	}

	public void Withdraw(double amount)
	{
		double balance = CalculateBalance();
		if (amount > balance)
		{
			Console.WriteLine($"You do not have sufficient funds, you can withdraw up to {balance} {_accountCurrency}");
		}
		else
		{
			_transactionList.Add(new Transaction(amount, "withdrawal"));
		}
	}

	public void PrintTransactions()
	{
		Console.WriteLine($"Transactions for the account {_accountNumber}");
		foreach(var transaction in _transactionList)
		{
			Console.WriteLine($"{transaction.Timestamp} {transaction.Type} {transaction.Amount}");
		}
		// Print balance
		Console.WriteLine("-----------------------");
		Console.WriteLine($"Balance: {CalculateBalance()}");
	}

	public double CalculateBalance()
	{
	    double balance = 0;
	    foreach(var transaction in _transactionList)
	    {
	        if(transaction.Type == "deposit")
	        {
	            balance += transaction.Amount;
	        }
	        else if(transaction.Type == "withdrawal")
	        {
	            balance -= transaction.Amount;
	        }
	    }
	    return balance;
	}
}

public class Transaction
{
	private DateTime _timestamp;
	private double _amount;
	private string _type;

	public DateTime Timestamp{ get{return _timestamp;} }
	public double Amount { get{return _amount;} }
	public string Type { get{return _type;} }

	public Transaction(double amount, string type)
	{
		_timestamp = DateTime.Now;
		_amount = amount;
		_type = type;
	}
}

public class Transfer
{
    // Define object data fields/variables
    private Account _sourceAccount;
    private Account _targetAccount;
    private double _amount;

    // Define constructor
    public Transfer(Account sourceAccount, Account targetAccount, double amount)
    {
        _sourceAccount = sourceAccount;
        _targetAccount = targetAccount;
        _amount = amount;
        Execute();
    }

    // Define the procedure of money transfer from one account to another
    public void Execute()
    {
        if (_sourceAccount.AccountCurrency != _targetAccount.AccountCurrency)
        {
            Console.WriteLine($"Error: account currencies are not the same! The attempted transfer is {_sourceAccount.AccountCurrency}, meanwhile the target account is {_targetAccount.AccountCurrency}");
        }

        else if (_amount > _sourceAccount.CalculateBalance())
        {
            Console.WriteLine($"Error: Insufficient funds for transfer of {_amount} {_sourceAccount.AccountCurrency}.");
        }
        else
        {
            // Withdraw from source account and deposit to target account
            _sourceAccount.Withdraw(_amount);
            _targetAccount.Deposit(_amount);
            Console.WriteLine($"Successfully transferred {_amount} {_sourceAccount.AccountCurrency} from account {_sourceAccount.AccountNumber} to account {_targetAccount.AccountNumber}.");
        }
    }
}
