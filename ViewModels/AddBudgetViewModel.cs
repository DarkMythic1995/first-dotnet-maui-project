using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PersonalFinanceTracker.Models;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;

/// <summary>
/// ViewModel responsible for managing the addition of budgets in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that provides the logic for adding a new budget entry.
    /// Includes observable properties for data binding and relay commands for save and cancel actions.
    /// </summary>
    public partial class AddBudgetViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the data service used for accessing and managing application data.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// Represents the connectivity service used to monitor and query network connectivity status.
        /// </summary>
        private readonly IConnectivity _connectivity;

        /// <summary>
        /// Represents the main view model used to manage the application's primary state and behavior.
        /// </summary>
        private readonly MainViewModel _mainViewModel;

        /// <summary>
        /// The budget object being edited or created.
        /// </summary>
        [ObservableProperty]
        private Budget budget;

        /// <summary>
        /// A collection of predefined budget categories available for selection.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<string> categories = new() { "Groceries", "Transport", "Dining Out", "Entertainment", "Utilities" };

        /// <summary>
        /// Initializes a new instance of the AddBudgetViewModel with required services.
        /// Sets up the DataService, connectivity, and MainViewModel dependencies, and initializes a new Budget object.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        /// <param name="connectivity">The service for checking network connectivity.</param>
        /// <param name="mainViewModel">The main view model for refreshing data after changes.</param>
        public AddBudgetViewModel(DataService dataService, IConnectivity connectivity, MainViewModel mainViewModel)
        {
            _dataService = dataService;
            _connectivity = connectivity;
            _mainViewModel = mainViewModel;
            Budget = new Budget { Id = Guid.NewGuid(), Month = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1) };
        }

        /// <summary>
        /// Saves the budget if validation passes, then navigates back to the MainPage.
        /// Checks for a valid amount and category, displays an error if invalid, and refreshes the main view model.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Save()
        {
            if (Budget.Amount <= 0 || string.IsNullOrEmpty(Budget.Category))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter a valid amount and category.", "OK");
                return;
            }
            await _dataService.AddBudgetAsync(Budget);
            await _mainViewModel.InitializeAsync();
            await Shell.Current.GoToAsync("//MainPage");
        }

        /// <summary>
        /// Cancels the budget addition and navigates back to the MainPage.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}