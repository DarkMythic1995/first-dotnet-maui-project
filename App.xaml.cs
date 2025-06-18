using Microsoft.Extensions.Logging;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;

/// <summary>
/// The main application class for the Personal Finance Tracker, serving as the entry point and managing window creation.
/// </summary>
namespace PersonalFinanceTracker
{
    /// <summary>
    /// A partial class representing the core application logic, inheriting from Application.
    /// Handles the creation of the main window and dependency setup for the app.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the App class.
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates and configures the main application window with the AppShell and necessary view models.
        /// </summary>
        /// <param name="activationState">The activation state of the application (not used in this implementation).</param>
        /// <returns>A Window instance containing the AppShell or an error page on failure.</returns>
        protected override Window CreateWindow(IActivationState activationState)
        {
            try
            {
                var connectivity = Connectivity.Current;
                var dataService = new DataService(connectivity);
                var mainViewModel = new MainViewModel(dataService, connectivity);
                var addTransactionViewModel = new AddTransactionViewModel(dataService, connectivity, mainViewModel);
                var addBudgetViewModel = new AddBudgetViewModel(dataService, connectivity, mainViewModel);
                var reportsViewModel = new ReportsViewModel(dataService); // Add ReportsViewModel
                return new Window(new AppShell(mainViewModel, addTransactionViewModel, addBudgetViewModel));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Window Creation Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return new Window(new ContentPage
                {
                    Content = new Label
                    {
                        Text = $"Error: {ex.Message}",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center
                    }
                });
            }
        }
    }
}