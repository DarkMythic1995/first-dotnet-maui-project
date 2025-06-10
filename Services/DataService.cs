using SQLite;
using PersonalFinanceTracker.Models;

/// <summary>
/// Provides data management services for the Personal Finance Tracker application using a SQLite database.
/// </summary>
namespace PersonalFinanceTracker.Services
{
    /// <summary>
    /// A service class that manages the SQLite database for storing and retrieving transactions and budgets.
    /// Implements asynchronous methods to interact with the database and enforces connectivity requirements.
    /// </summary>
    public class DataService
    {
        /// <summary>
        /// Represents the asynchronous connection to the SQLite database.
        /// </summary>
        private readonly SQLiteAsyncConnection _database;

        /// <summary>
        /// Represents the connectivity service used to check network availability and status.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// Initializes a new instance of the DataService with a SQLite database connection.
        /// The database path is set to a file in the app's data directory, and tables are created asynchronously.
        /// </summary>
        /// <param name="connectivity">The connectivity service to check network status.</param>
        public DataService(IConnectivity connectivity)
        {
            _connectivity = connectivity;
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "finance.db");
            _database = new SQLiteAsyncConnection(dbPath);
            // Ensure tables are created asynchronously during startup
            _ = InitializeDatabaseAsync();
        }

        /// <summary>
        /// Initializes the database by creating the Transaction and Budget tables if they do not exist.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<Transaction>();
            await _database.CreateTableAsync<Budget>();
        }

        /// <summary>
        /// Asynchronously retrieves a list of all transactions from the database.
        /// </summary>
        /// <returns>A Task containing a List of Transaction objects.</returns>
        public async Task<List<Transaction>> GetTransactionsAsync()
        {
            return await _database.Table<Transaction>().ToListAsync();
        }

        /// <summary>
        /// Asynchronously adds a new transaction to the database, requiring an internet connection.
        /// Displays an alert if no internet is available.
        /// </summary>
        /// <param name="transaction">The Transaction object to add.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AddTransactionAsync(Transaction transaction)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to add transactions.", "OK");
                return;
            }
            await _database.InsertAsync(transaction);
            System.Diagnostics.Debug.WriteLine($"Inserted transaction: {transaction.Id}, {transaction.Category}, Notes: {transaction.Notes ?? "None"}");
        }

        /// <summary>
        /// Displays an alert if no internet is available.
        /// </summary>
        /// <param name="id">The GUID of the transaction to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task DeleteTransactionAsync(Guid id)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to delete transactions.", "OK");
                return;
            }
            await _database.DeleteAsync<Transaction>(id);
        }

        /// <summary>
        /// Asynchronously retrieves a list of all budgets from the database.
        /// </summary>
        /// <returns>A Task containing a List of Budget objects.</returns>
        public async Task<List<Budget>> GetBudgetsAsync()
        {
            return await _database.Table<Budget>().ToListAsync();
        }

        /// <summary>
        /// Adds a new budget to the database, requiring an internet connection.
        /// Displays an alert if no internet is available.
        /// </summary>
        /// <param name="budget">The Budget object to add.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AddBudgetAsync(Budget budget)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to add budgets.", "OK");
                return;
            }
            await _database.InsertAsync(budget);
        }

        /// <summary>
        /// Calculates the total spending for a specific category within a given month.
        /// </summary>
        /// <param name="category">The category to filter spending by.</param>
        /// <param name="month">The month to filter spending by.</param>
        /// <returns>A Task containing the total spending amount as a decimal.</returns>
        public async Task<decimal> GetSpendingForCategoryAsync(string category, DateTime month)
        {
            var transactions = await _database.Table<Transaction>()
                .Where(t => !t.IsIncome && t.Category == category &&
                            t.Date.Year == month.Year && t.Date.Month == month.Month)
                .ToListAsync();
            return transactions.Sum(t => t.Amount);
        }
    }
}