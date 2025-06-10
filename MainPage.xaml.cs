using PersonalFinanceTracker.ViewModels;

/// <summary>
/// A ContentPage representing the main user interface of the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker
{
    /// <summary>
    /// A page class that provides the main UI for the application, utilizing data binding with a MainViewModel
    /// and handling data initialization on appearance.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the MainPage with a specified view model.
        /// Sets up the page's UI components, stores the view model, and binds it for data and command handling.
        /// </summary>
        /// <param name="viewModel">The MainViewModel instance to bind to this page.</param>
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Called when the page appears, asynchronously initializes the view model's data.
        /// Invokes the base OnAppearing method and triggers data loading via the view model.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }
    }
}