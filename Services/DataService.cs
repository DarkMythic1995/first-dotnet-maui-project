using SQLite;
using PersonalFinanceTracker.Models;

namespace PersonalFinanceTracker.Services
{
    /// <summary>
    /// Provides data management services for the Personal Finance Tracker application using a SQLite database.
    /// Handles CRUD operations for transactions and budgets, with network connectivity checks.
    /// </summary>
    public class DataService
    {
        /// <summary>
        /// The asynchronous connection to the SQLite database for data operations.
        /// </summary>
        private readonly SQLiteAsyncConnection _database;

        /// <summary>
        /// The connectivity service instance used to check network availability for data operations.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// Initializes a new instance of the DataService with a connectivity service.
        /// Sets up the SQLite database connection and initializes the database schema asynchronously.
        /// </summary>
        /// <param name="connectivity">The connectivity service to check network status.</param>
        public DataService(IConnectivity connectivity)
        {
            _connectivity = connectivity;
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "finance.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _ = InitializeDatabaseAsync();
        }

        /// <summary>
        /// Asynchronously initializes the database by creating the Transaction and Budget tables if they do not exist.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<PersonalFinanceTracker.Models.Transaction>();
            await _database.CreateTableAsync<Budget>();
        }

        /// <summary>
        /// Asynchronously retrieves a list of all transactions from the database.
        /// </summary>
        /// <returns>A Task containing a List of Transaction objects.</returns>
        public async Task<List<PersonalFinanceTracker.Models.Transaction>> GetTransactionsAsync()
        {
            return await _database.Table<PersonalFinanceTracker.Models.Transaction>().ToListAsync();
        }

        /// <summary>
        /// Asynchronously adds a new transaction to the database, requiring an internet connection.
        /// Displays an alert if no internet is available and logs the insertion.
        /// </summary>
        /// <param name="transaction">The Transaction object to add.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task AddTransactionAsync(PersonalFinanceTracker.Models.Transaction transaction)
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
        /// Asynchronously updates an existing transaction in the database, requiring an internet connection.
        /// Displays an alert if no internet is available and logs the update.
        /// </summary>
        /// <param name="transaction">The Transaction object to update.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task UpdateTransactionAsync(PersonalFinanceTracker.Models.Transaction transaction)
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to update transactions.", "OK");
                return;
            }
            await _database.UpdateAsync(transaction);
            System.Diagnostics.Debug.WriteLine($"Updated transaction: {transaction.Id}, {transaction.Category}, Amount: {transaction.Amount}");
        }

        /// <summary>
        /// Asynchronously deletes a transaction from the database, requiring an internet connection.
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
            await _database.DeleteAsync<PersonalFinanceTracker.Models.Transaction>(id);
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
        /// Asynchronously adds a new budget to the database, requiring an internet connection.
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
        /// Asynchronously deletes a budget from the database, requiring an internet connection.
        /// Displays an alert if no internet is available.
        /// </summary>
        /// <param name="id">The GUID of the budget to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task DeleteBudgetAsync(Guid id) // Updated to Guid to match Budget.Id
        {
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to delete budgets.", "OK");
                return;
            }
            await _database.DeleteAsync<Budget>(id);
        }

        /// <summary>
        /// Asynchronously calculates the total spending for a specific category within a given month.
        /// </summary>
        /// <param name="category">The category to filter spending by.</param>
        /// <param name="month">The month to filter spending by.</param>
        /// <returns>A Task containing the total spending amount as a decimal.</returns>
        public async Task<decimal> GetSpendingForCategoryAsync(string category, DateTime month)
        {
            var transactions = await _database.Table<PersonalFinanceTracker.Models.Transaction>()
                .Where(t => !t.IsIncome && t.Category == category &&
                            t.Date.Year == month.Year && t.Date.Month == month.Month)
                .ToListAsync();
            return transactions.Sum(t => t.Amount);
        }
    }
}