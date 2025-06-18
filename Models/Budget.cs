using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

/// <summary>
/// Represents a monthly budget for a specific category, used to track allocated amounts and time periods.
/// </summary>
namespace PersonalFinanceTracker.Models
{
    /// <summary>
    /// A model class that defines a budget entry with a unique identifier, category, amount, and month.
    /// Implements ObservableObject for property change notifications and uses SQLite attributes for database mapping.
    /// </summary>
    public partial class Budget : ObservableObject
    {
        /// <summary>
        /// The primary key for the budget entry, automatically generated as a new GUID.
        /// Marked with the [PrimaryKey] attribute for SQLite database mapping.
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The category of the budget for example, Groceries and Transportation.
        /// Managed by the ObservableProperty attribute for automatic property change notifications.
        /// </summary>
        [ObservableProperty]
        private string category;

        /// <summary>
        /// The allocated amount for the budget category, observable for UI updates.
        /// </summary>
        [ObservableProperty]
        private decimal amount;

        /// <summary>
        /// The month for which the budget applies.
        /// </summary>
        [ObservableProperty]
        private DateTime month;
    }
}