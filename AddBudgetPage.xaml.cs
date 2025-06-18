using Microsoft.Maui.Controls;
using PersonalFinanceTracker.ViewModels;

/// <summary>
/// A ContentPage representing the user interface for adding a new budget in the Personal Finance Tracker application.
/// This page binds to an AddBudgetViewModel to manage the budget creation process and display input fields.
/// </summary>
namespace PersonalFinanceTracker.Views
{
    /// <summary>
    /// A page class that provides the UI for adding a budget, utilizing data binding with an AddBudgetViewModel.
    /// </summary>
    public partial class AddBudgetPage : ContentPage
    {
        /// <summary>
        /// Initializes a new instance of the AddBudgetPage with a specified view model.
        /// Sets up the page's UI components and binds the view model for data and command handling.
        /// </summary>
        /// <param name="vm">The AddBudgetViewModel instance to bind to this page.</param>
        public AddBudgetPage(AddBudgetViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}