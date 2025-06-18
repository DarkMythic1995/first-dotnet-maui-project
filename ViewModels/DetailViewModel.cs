using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// ViewModel responsible for managing the details of a specific transaction in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that provides the logic for displaying and managing transaction details.
    /// </summary>
    [QueryProperty(nameof(TransactionId), "TransactionId")]
    public partial class DetailViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the data service used for accessing and managing application data.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// The unique identifier of the transaction.
        /// </summary>
        [ObservableProperty]
        private string _transactionId;

        /// <summary>
        /// The category of the transaction.
        /// </summary>
        [ObservableProperty]
        private string category;

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        [ObservableProperty]
        private decimal amount;

        /// <summary>
        /// The formatted date of the transaction.
        /// </summary>
        [ObservableProperty]
        private string date;

        /// <summary>
        /// The notes or description of the transaction.
        /// </summary>
        [ObservableProperty]
        private string notes;

        /// <summary>
        /// The type of the transaction (example: "Income" or "Expense").
        /// </summary>
        [ObservableProperty]
        private string transactionType;

        /// <summary>
        /// Initializes a new instance of the DetailViewModel with a DataService dependency.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        public DetailViewModel(DataService dataService)
        {
            _dataService = dataService;
            LoadTransactionData();
        }

        /// <summary>
        /// Handles changes to the TransactionId property by updating the value and reloading transaction data.
        /// </summary>
        /// <param name="value">The new TransactionId value.</param>
        partial void OnTransactionIdChanged(string value)
        {
            _transactionId = value;
            LoadTransactionData();
        }

        /// <summary>
        /// Loads transaction data based on the current TransactionId.
        /// Retrieves transactions from the DataService, populates observable properties, and handles cases where
        /// the transaction is not found.
        /// </summary>
        private async void LoadTransactionData()
        {
            if (!string.IsNullOrEmpty(TransactionId) && Guid.TryParse(TransactionId, out var id))
            {
                var transactions = await _dataService.GetTransactionsAsync();
                var transaction = transactions.FirstOrDefault(t => t.Id == id);
                if (transaction != null)
                {
                    Category = transaction.Category;
                    Amount = transaction.Amount;
                    Date = transaction.Date.ToString("MMM dd, yyyy");
                    Notes = transaction.Notes ?? "None";
                    TransactionType = transaction.IsIncome ? "Income" : "Expense";
                }
                else
                {
                    Category = "Not Found";
                    Amount = 0;
                    Date = "N/A";
                    Notes = "N/A";
                    TransactionType = "N/A";
                }
            }
        }

        /// <summary>
        /// Asynchronously handles the "Go Back" action by attempting to navigate up the stack or falling back to MainPage.
        /// Logs the navigation stack and any errors for debugging purposes.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task GoBack()
        {
            System.Diagnostics.Debug.WriteLine("GoBack command executed");
            try
            {
                var currentNavigationStack = Shell.Current.Navigation.NavigationStack;
                System.Diagnostics.Debug.WriteLine($"Current navigation stack count: {currentNavigationStack.Count}");
                foreach (var page in currentNavigationStack)
                {
                    System.Diagnostics.Debug.WriteLine($"Stack page: {page.GetType().Name}");
                }
                await Shell.Current.GoToAsync("..");
                System.Diagnostics.Debug.WriteLine("Navigation to parent successful");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation failed: {ex.Message}");
                // Fallback to MainPage explicitly
                await Shell.Current.GoToAsync("//MainPage");
                System.Diagnostics.Debug.WriteLine("Fell back to MainPage");
            }
        }
    }
}