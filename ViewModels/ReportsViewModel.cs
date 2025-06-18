using CommunityToolkit.Mvvm.ComponentModel;
using PersonalFinanceTracker.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input; // Added for RelayCommand

/// <summary>
/// ViewModel responsible for generating and managing financial reports in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.ViewModels
{
    /// <summary>
    /// A ViewModel class that provides the logic for displaying financial reports, including category and monthly spending.
    /// </summary>
    public partial class ReportsViewModel : ObservableObject
    {
        /// <summary>
        /// Represents the data service used for accessing and managing application data.
        /// </summary>
        private readonly DataService _dataService;

        /// <summary>
        /// Represents the current month as a <see cref="DateTime"/> value.
        /// </summary>
        private readonly DateTime _currentMonth;

        /// <summary>
        /// A collection of tuples representing category spending, with each tuple containing a category name and amount.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<(string Category, decimal Amount)> categorySpendings;

        /// <summary>
        /// A collection of tuples representing monthly spending, with each tuple containing a month and amount.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<(DateTime Month, decimal Amount)> monthlySpendings;

        /// <summary>
        /// Initializes a new instance of the ReportsViewModel with a DataService dependency.
        /// Sets up the data service, initializes the current month, and starts loading report data asynchronously.
        /// </summary>
        /// <param name="dataService">The service for database operations.</param>
        public ReportsViewModel(DataService dataService)
        {
            _dataService = dataService;
            _currentMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            CategorySpendings = new ObservableCollection<(string Category, decimal Amount)>();
            MonthlySpendings = new ObservableCollection<(DateTime Month, decimal Amount)>();
            Debug.WriteLine("ReportsViewModel: Initialized, starting LoadReportDataAsync");
            _ = LoadReportDataAsync();
        }

        /// <summary>
        /// Asynchronously loads report data, including category and monthly spending, from the database.
        /// Filters transactions for the current month, calculates spending per category, and aggregates spending
        /// for the past six months, updating observable collections on success or logging errors.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task LoadReportDataAsync()
        {
            Debug.WriteLine("ReportsViewModel: Starting LoadReportDataAsync");
            try
            {
                var budgets = await _dataService.GetBudgetsAsync();
                Debug.WriteLine($"ReportsViewModel: Retrieved {budgets.Count} budgets");
                var transactions = await _dataService.GetTransactionsAsync();
                Debug.WriteLine($"ReportsViewModel: Retrieved {transactions.Count} transactions");

                var currentMonthTransactions = transactions.Where(t => t.Date.Month == _currentMonth.Month && t.Date.Year == _currentMonth.Year && !t.IsIncome).ToList();
                Debug.WriteLine($"ReportsViewModel: Filtered {currentMonthTransactions.Count} transactions for current month {_currentMonth:MMM yyyy}");

                var categories = budgets.Select(b => b.Category).Distinct();
                CategorySpendings.Clear();
                foreach (var category in categories)
                {
                    var spent = currentMonthTransactions.Where(t => t.Category == category).Sum(t => t.Amount);
                    CategorySpendings.Add((Category: category, Amount: spent));
                    Debug.WriteLine($"ReportsViewModel: Added category spending - {category}: {spent}");
                }

                var startMonth = _currentMonth.AddMonths(-5);
                MonthlySpendings.Clear();
                for (var month = startMonth; month <= _currentMonth; month = month.AddMonths(1))
                {
                    var monthlySpent = transactions.Where(t => t.Date >= month && t.Date < month.AddMonths(1) && !t.IsIncome).Sum(t => t.Amount);
                    MonthlySpendings.Add((Month: month, Amount: monthlySpent));
                    Debug.WriteLine($"ReportsViewModel: Added monthly spending - {month:MMM yyyy}: {monthlySpent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"ReportsViewModel: Error in LoadReportDataAsync - {ex.Message}");
            }
            Debug.WriteLine("ReportsViewModel: LoadReportDataAsync completed");
        }

        /// <summary>
        /// Navigates back to the main page of the application.
        /// </summary>
        /// <remarks>This method uses the Shell navigation system to navigate to the route "//MainPage".
        /// Ensure that the route "//MainPage" is registered in the Shell configuration.</remarks>
        /// <returns>A task that represents the asynchronous navigation operation.</returns>
        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}