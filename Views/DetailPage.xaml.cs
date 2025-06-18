using Microsoft.Maui.Controls;
using PersonalFinanceTracker.ViewModels;

/// <summary>
/// A ContentPage representing the user interface for displaying the details of a specific transaction
/// in the Personal Finance Tracker application.
/// This page binds to a DetailViewModel to manage and display transaction information.
/// </summary>
namespace PersonalFinanceTracker.Views
{
    /// <summary>
    /// A page class that provides the UI for viewing transaction details, utilizing data binding with a DetailViewModel.
    /// </summary>
    public partial class DetailPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the DetailPage with a specified view model.
        /// Sets up the page's UI components and binds the view model for data and command handling.
        /// </summary>
        /// <param name="vm">The DetailViewModel instance to bind to this page.</param>
        public DetailPage(DetailViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}