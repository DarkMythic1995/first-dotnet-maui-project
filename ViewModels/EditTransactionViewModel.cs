using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;

/// <summary>
/// ViewModel responsible for managing the editing of transactions in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that provides the logic for editing an existing transaction entry.
    /// Includes observable properties for data binding and relay commands for save and cancel actions.
    /// </summary>
    [QueryProperty(nameof(TransactionId), "TransactionId")]
    public partial class EditTransactionViewModel : ObservableObject
    {
        /// <summary>
        /// The data service instance used to perform database operations for transactions.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// The connectivity service instance used to check network availability for data operations.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// The main view model instance used to refresh the main page data after transaction updates.
        /// </summary>
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// The unique identifier of the transaction being edited.
        /// </summary>
        [ObservableProperty]
        private string transactionId;

        /// <summary>
        /// The transaction object being edited, observable for UI updates.
        /// </summary>
        [ObservableProperty]
        private PersonalFinanceTracker.Models.Transaction selectedTransaction; // Fully qualified to avoid ambiguity

        /// <summary>
        /// A collection of predefined transaction categories available for selection.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> categories = new()
        {
            "Groceries", "Transport", "Salary", "Dining Out", "Entertainment", "Utilities"
        };

        /// <summary>
        /// A message displayed to the user when validation fails, observable for UI updates.
        /// </summary>
        [ObservableProperty]
        private string validationMessage;

        /// <summary>
        /// Initializes a new instance of the EditTransactionViewModel with required services.
        /// Sets up the DataService, connectivity, and MainViewModel dependencies.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        /// <param name="connectivity">The service for checking network connectivity.</param>
        /// <param name="mainViewModel">The main view model for refreshing data after changes.</param>
        public EditTransactionViewModel(DataService dataService, IConnectivity connectivity, MainViewModel mainViewModel)
        {
            _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
            _connectivity = connectivity ?? throw new ArgumentNullException(nameof(connectivity));
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
            Debug.WriteLine("EditTransactionViewModel: Initialized");
        }

        /// <summary>
        /// Handles changes to the TransactionId property by loading the transaction data.
        /// </summary>
        /// <param name="value">The new TransactionId value.</param>
        partial void OnTransactionIdChanged(string value)
        {
            TransactionId = value;
            Debug.WriteLine($"EditTransactionViewModel: TransactionId changed to {value}");
            LoadTransactionAsync();
        }

        /// <summary>
        /// Asynchronously loads the transaction data based on the TransactionId.
        /// </summary>
        private async void LoadTransactionAsync()
        {
            Debug.WriteLine("EditTransactionViewModel: Entering LoadTransactionAsync");
            try
            {
                if (!string.IsNullOrEmpty(TransactionId) && Guid.TryParse(TransactionId, out var id))
                {
                    var transactions = await _dataService.GetTransactionsAsync();
                    SelectedTransaction = transactions.FirstOrDefault(t => t.Id == id) ?? new PersonalFinanceTracker.Models.Transaction
                    {
                        Id = id,
                        Date = DateTime.Today,
                        IsIncome = false
                    };
                    Debug.WriteLine($"EditTransactionViewModel: Loaded transaction {SelectedTransaction?.Id}, Category: {SelectedTransaction?.Category}");
                }
                else
                {
                    Debug.WriteLine("EditTransactionViewModel: Invalid or empty TransactionId");
                    SelectedTransaction = new PersonalFinanceTracker.Models.Transaction { Date = DateTime.Today, IsIncome = false }; // Default if ID is invalid
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditTransactionViewModel: Error in LoadTransactionAsync - {ex.Message}");
                SelectedTransaction = new PersonalFinanceTracker.Models.Transaction { Date = DateTime.Today, IsIncome = false }; // Fallback
            }
        }

        /// <summary>
        /// Saves the edited transaction if validation and connectivity checks pass, then navigates back.
        /// Validates that the amount is positive and a category is selected, checks for internet access,
        /// updates the transaction, refreshes the main view model, and navigates back.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Save()
        {
            Debug.WriteLine("EditTransactionViewModel: Save command triggered");
            try
            {
                ValidationMessage = null;
                if (SelectedTransaction == null)
                {
                    ValidationMessage = "No transaction selected.";
                    Debug.WriteLine("EditTransactionViewModel: Save failed - SelectedTransaction is null");
                    return;
                }
                if (SelectedTransaction.Amount <= 0)
                {
                    ValidationMessage = "Please enter a valid amount greater than zero.";
                    Debug.WriteLine("EditTransactionViewModel: Save failed - Invalid amount");
                    return;
                }
                if (string.IsNullOrEmpty(SelectedTransaction.Category))
                {
                    ValidationMessage = "Please select a category.";
                    Debug.WriteLine("EditTransactionViewModel: Save failed - No category selected");
                    return;
                }
                Debug.WriteLine($"EditTransactionViewModel: NetworkAccess is {_connectivity.NetworkAccess}");
                if (_connectivity.NetworkAccess != NetworkAccess.Internet)
                {
                    ValidationMessage = "No internet connection available.";
                    Debug.WriteLine("EditTransactionViewModel: Displaying no internet alert");
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("No Internet", "An internet connection is required to save.", "OK");
                    });
                    Debug.WriteLine("EditTransactionViewModel: No internet alert displayed");
                    return;
                }
                Debug.WriteLine($"EditTransactionViewModel: Updating transaction: {SelectedTransaction.Category}, {SelectedTransaction.Amount}, {SelectedTransaction.Date}, IsIncome: {SelectedTransaction.IsIncome}");
                await _dataService.UpdateTransactionAsync(SelectedTransaction);
                Debug.WriteLine("EditTransactionViewModel: Transaction updated in database");
                await _mainViewModel.InitializeAsync();
                Debug.WriteLine("EditTransactionViewModel: MainViewModel refreshed");
                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                ValidationMessage = "An error occurred while saving.";
                Debug.WriteLine($"EditTransactionViewModel: Save error - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Cancels the transaction edit and navigates back.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Cancel()
        {
            Debug.WriteLine("EditTransactionViewModel: Cancel command triggered");
            try
            {
                await NavigateBackAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditTransactionViewModel: Cancel error - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Navigates back to MainPage using Shell navigation.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task NavigateBackAsync()
        {
            Debug.WriteLine("EditTransactionViewModel: Attempting navigation back");
            try
            {
                await Shell.Current.GoToAsync("//MainPage");
                Debug.WriteLine("EditTransactionViewModel: Navigation to MainPage successful");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"EditTransactionViewModel: Navigation failed - {ex.Message}\nStackTrace: {ex.StackTrace}");
            }
        }
    }
}