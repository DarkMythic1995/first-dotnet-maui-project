using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.Maui.Dispatching;

namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// ViewModel serving as the main controller for the Personal Finance Tracker application.
    /// Manages data and commands for displaying and manipulating budgets and transactions.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        /// <summary>
        /// The singleton instance of the MainViewModel to ensure a single point of control.
        /// </summary>
        private static MainViewModel _instance;

        /// <summary>
        /// The data service instance for database operations related to transactions and budgets.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// The connectivity service to check network availability for data operations.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// Observable collection of transactions displayed on the main page.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Transaction> transactions = new();

        /// <summary>
        /// Observable collection of budgets displayed on the main page.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Budget> budgets = new();

        /// <summary>
        /// The current month for which budgets and transactions are displayed.
        /// </summary>
        [ObservableProperty]
        private DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        /// <summary>
        /// Initializes a new instance of the MainViewModel with the specified data and connectivity services.
        /// Sets up the collections and establishes the singleton instance.
        /// </summary>
        /// <param name="dataService">The data service for database interactions.</param>
        /// <param name="connectivity">The connectivity service for network checks.</param>
        public MainViewModel(DataService dataService, IConnectivity connectivity)
        {
            _dataService = dataService;
            _connectivity = connectivity;
            transactions = new ObservableCollection<Transaction>();
            budgets = new ObservableCollection<Budget>();
            _instance = this;
        }

        /// <summary>
        /// Returns the singleton instance of the MainViewModel, creating it if it doesn’t exist.
        /// </summary>
        /// <returns>The singleton MainViewModel instance.</returns>
        public static MainViewModel GetInstance()
        {
            return _instance ??= new MainViewModel(null, null);
        }

        /// <summary>
        /// Asynchronously initializes the view model by setting up the database and loading initial data.
        /// </summary>
        public async Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                await _dataService.InitializeDatabaseAsync();
                await LoadDataAsync();
            });
        }

        /// <summary>
        /// Asynchronously loads transactions and budgets from the database and updates the observable collections on the main thread.
        /// </summary>
        private async Task LoadDataAsync()
        {
            var transactionList = await _dataService.GetTransactionsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Transactions.Clear();
                foreach (var transaction in transactionList.OrderByDescending(t => t.Date))
                {
                    Transactions.Add(transaction);
                }
            });
            var budgetList = await _dataService.GetBudgetsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Budgets.Clear();
                foreach (var budget in budgetList)
                {
                    Budgets.Add(budget);
                }
                Debug.WriteLine($"Loaded {Transactions.Count} transactions and {Budgets.Count} budgets");
            });
        }

        /// <summary>
        /// Command to navigate to the Add Transaction page.
        /// </summary>
        [RelayCommand]
        private async Task AddTransaction() => await Shell.Current.GoToAsync("//AddTransactionPage");

        /// <summary>
        /// Command to navigate to the Detail page for a specific transaction.
        /// </summary>
        /// <param name="transaction">The transaction to view details for.</param>
        [RelayCommand]
        private async Task ViewTransaction(Transaction transaction) => await Shell.Current.GoToAsync($"//DetailPage?TransactionId={transaction.Id}");

        /// <summary>
        /// Command to delete a transaction, with checks for null and internet connectivity.
        /// Displays an error if the deletion fails.
        /// </summary>
        /// <param name="transaction">The transaction to delete.</param>
        [RelayCommand]
        private async Task DeleteTransaction(Transaction transaction)
        {
            if (transaction == null)
            {
                Debug.WriteLine("DeleteTransaction: Transaction is null");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to delete transactions.", "OK");
                return;
            }

            try
            {
                Debug.WriteLine($"Deleting transaction with Id: {transaction.Id}");
                await _dataService.DeleteTransactionAsync(transaction.Id);
                Transactions.Remove(transaction);
                Debug.WriteLine($"Transaction with Id {transaction.Id} deleted successfully");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete transaction: {ex.Message}", "OK");
                Debug.WriteLine($"DeleteTransaction error: {ex}");
            }
        }

        /// <summary>
        /// Command to navigate to the Add Budget page.
        /// </summary>
        [RelayCommand]
        private async Task AddBudget() => await Shell.Current.GoToAsync("//AddBudgetPage");

        /// <summary>
        /// Command to navigate to the Reports page.
        /// </summary>
        [RelayCommand]
        private async Task ViewReports() => await Shell.Current.GoToAsync("//ReportsPage");

        /// <summary>
        /// Command to navigate to the Edit Transaction page for a specific transaction.
        /// </summary>
        /// <param name="transaction">The transaction to edit.</param>
        [RelayCommand]
        private async Task EditTransaction(Transaction transaction) => await Shell.Current.GoToAsync($"//EditTransactionPage?TransactionId={transaction.Id}");

        /// <summary>
        /// Command to show a context menu with options to edit or view transaction details.
        /// </summary>
        /// <param name="transaction">The transaction to show options for.</param>
        [RelayCommand]
        private async Task ShowTransactionOptions(Transaction transaction)
        {
            string action = await Application.Current.MainPage.DisplayActionSheet(
                "Choose an action",
                "Cancel",
                null,
                "Edit Details",
                "View Details");

            switch (action)
            {
                case "Edit Details":
                    await EditTransaction(transaction);
                    break;
                case "View Details":
                    await ViewTransaction(transaction);
                    break;
                default:
                    // Cancel or null, do nothing
                    break;
            }
        }

        /// <summary>
        /// Command to delete a budget, with confirmation and checks for null and internet connectivity.
        /// Displays an error if the deletion fails.
        /// </summary>
        /// <param name="budget">The budget to delete.</param>
        [RelayCommand]
        private async Task DeleteBudget(Budget budget)
        {
            if (budget == null)
            {
                Debug.WriteLine("DeleteBudget: Budget is null");
                return;
            }

            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("No Internet", "You need an internet connection to delete budgets.", "OK");
                return;
            }

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Confirm Deletion",
                $"Are you sure you want to delete the budget for {budget.Category}? This action cannot be undone.",
                "Yes",
                "No");

            if (!confirm) return;

            try
            {
                Debug.WriteLine($"Deleting budget with Id: {budget.Id}");
                await _dataService.DeleteBudgetAsync(budget.Id);
                var budgetToRemove = Budgets.FirstOrDefault(b => b.Id == budget.Id);
                if (budgetToRemove != null)
                {
                    Budgets.Remove(budgetToRemove);
                }
                else
                {
                    Debug.WriteLine($"Budget with Id {budget.Id} not found in collection");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Failed to delete budget: {ex.Message}", "OK");
                Debug.WriteLine($"DeleteBudget error: {ex}");
            }
        }

        /// <summary>
        /// Calculates the progress percentage of spending against a budget for a given category.
        /// </summary>
        /// <param name="category">The budget category to calculate progress for.</param>
        /// <returns>The progress percentage as a decimal value (0-100).</returns>
        public async Task<decimal> GetBudgetProgressAsync(string category)
        {
            var budget = Budgets.FirstOrDefault(b => b.Category == category &&
                                                    b.Month.Year == CurrentMonth.Year &&
                                                    b.Month.Month == CurrentMonth.Month);
            if (budget == null)
            {
                Debug.WriteLine($"GetBudgetProgressAsync: No budget found for {category}");
                return 0;
            }
            var spent = await _dataService.GetSpendingForCategoryAsync(category, CurrentMonth);
            var progress = budget.Amount == 0 ? 0 : Math.Min(100, (spent / budget.Amount) * 100);
            Debug.WriteLine($"GetBudgetProgressAsync: {category} - Budget: {budget.Amount}, Spent: {spent}, Progress: {progress}%");
            return progress;
        }
    }
}