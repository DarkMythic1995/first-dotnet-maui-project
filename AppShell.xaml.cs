using PersonalFinanceTracker.ViewModels;
using PersonalFinanceTracker.Views;

/// <summary>
/// The main navigation shell for the Personal Finance Tracker application, inheriting from Shell.
/// This class sets up the application's navigation structure, binds to the MainViewModel, and registers
/// routes for various pages to enable navigation throughout the app.
/// </summary>
namespace PersonalFinanceTracker
{
    /// <summary>
    /// A partial class representing the shell of the application, managing navigation and page routing.
    /// </summary>
    public partial class AppShell : Shell
    {
        /// <summary>
        /// Initializes a new instance of the AppShell with specified view models.
        /// Sets up the shell's UI components, binds the MainViewModel, and registers navigation routes
        /// for all application pages.
        /// </summary>
        /// <param name="mainViewModel">The MainViewModel instance to bind as the shell's context.</param>
        /// <param name="addTransactionViewModel">The AddTransactionViewModel instance (currently unused in this constructor).</param>
        /// <param name="addBudgetViewModel">The AddBudgetViewModel instance (currently unused in this constructor).</param>
        public AppShell(MainViewModel mainViewModel, AddTransactionViewModel addTransactionViewModel, AddBudgetViewModel addBudgetViewModel)
        {
            InitializeComponent();
            BindingContext = mainViewModel;
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("AddTransactionPage", typeof(AddTransactionPage));
            Routing.RegisterRoute("AddBudgetPage", typeof(AddBudgetPage));
            Routing.RegisterRoute("ReportsPage", typeof(ReportsPage));
            Routing.RegisterRoute("DetailPage", typeof(DetailPage));
        }
    }
}