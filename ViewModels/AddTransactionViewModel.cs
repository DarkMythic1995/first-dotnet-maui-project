using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.Views;
using System.Collections.ObjectModel;

/// <summary>
/// ViewModel responsible for managing the addition of transactions in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that provides the logic for adding a new transaction entry.
    /// Includes observable properties for data binding and relay commands for save and cancel actions.
    /// </summary>
    public partial class AddTransactionViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the data service used for accessing and managing application data.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// Represents the connectivity service used to monitor and check network status.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// Represents the main view model used to manage the application's primary state and behavior.
        /// </summary>
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// The transaction object being edited or created, observable for UI updates.
        /// </summary>
        [ObservableProperty]
        private Transaction selectedTransaction;

        /// <summary>
        /// A collection of predefined transaction categories available for selection.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> categories = new() { "Groceries", "Transport", "Salary", "Dining Out", "Entertainment", "Utilities" };

        /// <summary>
        /// Initializes a new instance of the AddTransactionViewModel with required services.
        /// Sets up the DataService, connectivity, and MainViewModel dependencies, and initializes a new Transaction object.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        /// <param name="connectivity">The service for checking network connectivity.</param>
        /// <param name="mainViewModel">The main view model for refreshing data after changes.</param>
        public AddTransactionViewModel(DataService dataService, IConnectivity connectivity, MainViewModel mainViewModel)
        {
            _dataService = dataService;
            _connectivity = connectivity;
            _mainViewModel = mainViewModel;
            SelectedTransaction = new Transaction { Id = Guid.NewGuid(), Date = DateTime.Today, IsIncome = false }; // Explicitly set IsIncome
        }

        /// <summary>
        /// Saves the transaction if validation and connectivity checks pass, then navigates to the DetailPage.
        /// Validates that the amount is positive and a category is selected, checks for internet access,
        /// saves the transaction, refreshes the main view model, and pushes a new DetailPage modally.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Save()
        {
            if (SelectedTransaction.Amount <= 0 || string.IsNullOrEmpty(SelectedTransaction.Category))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid amount and category.", "OK");
                return;
            }
            if (_connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert("No Internet", "An internet connection is required to save.", "OK");
                return;
            }
            System.Diagnostics.Debug.WriteLine($"Saving transaction: {SelectedTransaction.Category}, {SelectedTransaction.Amount}, {SelectedTransaction.Date}, IsIncome: {SelectedTransaction.IsIncome}");
            await _dataService.AddTransactionAsync(SelectedTransaction);
            System.Diagnostics.Debug.WriteLine("Transaction saved to database");
            await _mainViewModel.InitializeAsync();
            System.Diagnostics.Debug.WriteLine("MainViewModel refreshed");
            // Use modal navigation to ensure a back stack
            await Shell.Current.Navigation.PushAsync(new DetailPage(new DetailViewModel(_dataService)) { BindingContext = new DetailViewModel(_dataService) { TransactionId = SelectedTransaction.Id.ToString() } });
        }

        /// <summary>
        /// Cancels the transaction addition and navigates back to the MainPage.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}