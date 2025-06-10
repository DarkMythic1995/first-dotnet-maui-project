using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

/// <summary>
/// ViewModel serving as the main controller for the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that acts as the central hub for managing application data and navigation.
    /// </summary>
    public partial class MainViewModel : ObservableObject
    {
        private static MainViewModel _instance;
        private readonly DataService _dataService;
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// A collection of transactions displayed in the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Transaction> transactions = new();

        /// <summary>
        /// A collection of budgets displayed in the UI.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<Budget> budgets = new();

        /// <summary>
        /// The current month for which data is displayed.
        /// </summary>
        [ObservableProperty]
        private DateTime currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        /// <summary>
        /// Initializes a new instance of the MainViewModel with required services.
        /// Sets up the DataService and connectivity dependencies, initializes collections,
        /// and assigns the instance to a static reference.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        /// <param name="connectivity">The service for checking network connectivity.</param>
        public MainViewModel(DataService dataService, IConnectivity connectivity)
        {
            _dataService = dataService;
            _connectivity = connectivity;
            transactions = new ObservableCollection<Transaction>();
            budgets = new ObservableCollection<Budget>();
            _instance = this;
        }

        /// <summary>
        /// Provides a static instance of the MainViewModel, creating a new one with null services if not initialized.
        /// </summary>
        /// <returns>The singleton instance of MainViewModel.</returns>
        public static MainViewModel GetInstance()
        {
            return _instance ??= new MainViewModel(null, null);
        }

        /// <summary>
        /// Asynchronously initializes the application data by setting up the database and loading initial data.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await Task.Run(async () =>
            {
                await _dataService.InitializeDatabaseAsync();
                await LoadDataAsync();
            });
        }

        /// <summary>
        /// Loads transactions and budgets from the database.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task LoadDataAsync()
        {
            var transactionList = await _dataService.GetTransactionsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Transactions.Clear();
                foreach (var transaction in transactionList.OrderByDescending(t => t.Date)) // Already correct
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
        /// Asynchronously navigates to the AddTransactionPage for adding a new transaction.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task AddTransaction() => await Shell.Current.GoToAsync("//AddTransactionPage");

        /// <summary>
        /// Asynchronously navigates to the DetailPage for viewing a specific transaction's details.
        /// </summary>
        /// <param name="transaction">The transaction to view.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewTransaction(Transaction transaction) => await Shell.Current.GoToAsync($"//DetailPage?TransactionId={transaction.Id}");

        /// <summary>
        /// Deletes a transaction from the database and removes it from the collection.
        /// </summary>
        /// <param name="transaction">The transaction to delete.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task DeleteTransaction(Transaction transaction)
        {
            await _dataService.DeleteTransactionAsync(transaction.Id);
            Transactions.Remove(transaction);
        }

        /// <summary>
        /// Navigates to the AddBudgetPage for adding a new budget.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task AddBudget() => await Shell.Current.GoToAsync("//AddBudgetPage");

        /// <summary>
        /// Navigates to the ReportsPage for viewing financial reports.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task ViewReports() => await Shell.Current.GoToAsync("//ReportsPage");

        /// <summary>
        /// Calculates the progress percentage for a given budget category based on current spending.
        /// Returns 0 if no budget is found or the amount is zero, otherwise computes the percentage spent.
        /// </summary>
        /// <param name="category">The budget category to calculate progress for.</param>
        /// <returns>A Task containing the progress percentage as a decimal.</returns>
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