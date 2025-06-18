using Microsoft.Maui.Controls;
using PersonalFinanceTracker.ViewModels;

/// <summary>
/// A ContentPage representing the user interface for editing an existing transaction in the Personal Finance Tracker application.
/// This page binds to an EditTransactionViewModel to manage the transaction editing process and display input fields.
/// </summary>
namespace PersonalFinanceTracker.Views
{
    /// <summary>
    /// A page class that provides the UI for editing a transaction, utilizing data binding with an EditTransactionViewModel.
    /// </summary>
    public partial class EditTransactionPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the EditTransactionPage with a specified view model.
        /// Sets up the page's UI components and binds the view model for data and command handling.
        /// </summary>
        /// <param name="vm">The EditTransactionViewModel instance to bind to this page.</param>
        public EditTransactionPage(EditTransactionViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}