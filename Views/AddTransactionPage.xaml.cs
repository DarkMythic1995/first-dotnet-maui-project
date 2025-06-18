using Microsoft.Maui.Controls;
using PersonalFinanceTracker.ViewModels;

/// <summary>
/// A ContentPage representing the user interface for adding a new transaction in the Personal Finance Tracker application.
/// This page binds to an AddTransactionViewModel to manage the transaction creation process and display input fields.
/// </summary>
namespace PersonalFinanceTracker.Views
{
    /// <summary>
    /// A page class that provides the UI for adding a transaction, utilizing data binding with an AddTransactionViewModel.
    /// </summary>
    public partial class AddTransactionPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the AddTransactionPage with a specified view model.
        /// Sets up the page's UI components and binds the view model for data and command handling.
        /// </summary>
        /// <param name="vm">The AddTransactionViewModel instance to bind to this page.</param>
        public AddTransactionPage(AddTransactionViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}