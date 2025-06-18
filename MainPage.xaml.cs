using PersonalFinanceTracker.ViewModels;
using Microsoft.Maui.Controls;

namespace PersonalFinanceTracker
{
    /// <summary>
    /// Represents the main page of the Personal Finance Tracker application, handling the display and interaction of budgets and transactions.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        /// The view model instance managing the data and commands for the main page.
        /// </summary>
        private readonly MainViewModel _viewModel;

        /// <summary>
        /// Tracks the current theme state, true for dark mode, false for light mode.
        /// </summary>
        private bool isDarkMode = false;

        /// <summary>
        /// Initializes a new instance of the MainPage with the specified view model.
        /// Sets up the UI components and binds the view model to the page.
        /// </summary>
        /// <param name="viewModel">The MainViewModel instance to bind to the page.</param>
        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        /// <summary>
        /// Called when the page appears, triggering the initialization of the view model's data.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.InitializeAsync();
        }

        /// <summary>
        /// Handles the theme toggle action, switching between light and dark modes.
        /// Updates the background color, text color, box color, and background image based on the current theme.
        /// </summary>
        /// <param name="sender">The object that triggered the event (e.g., the Toggle Theme button).</param>
        /// <param name="e">Event arguments providing additional context.</param>
        private void OnToggleTheme(object sender, EventArgs e)
        {
            var dict = Application.Current.Resources;
            var isLight = (Color)dict["CurrentBackground"] == (Color)dict["BgLight"];
            dict["CurrentBackground"] = isLight ? (Color)dict["BgDark"] : (Color)dict["BgLight"];
            dict["CurrentTextColor"] = isLight ? (Color)dict["TextDark"] : (Color)dict["TextLight"];
            dict["CurrentBoxColor"] = isLight ? (Color)dict["NightBoxColor"] : (Color)dict["BgLight"];

            // Set background image based on theme
            if (isLight)
            {
                // Dark mode
                BackgroundImageSource = ImageSource.FromFile("dark_background_2.png");
            }
            else
            {
                // Light mode
                BackgroundImageSource = ImageSource.FromFile("light_background_1.png"); // Updated filename
            }
        }
    }
}