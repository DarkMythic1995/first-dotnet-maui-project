using CommunityToolkit.Mvvm.ComponentModel;
using SQLite;

/// <summary>
/// Represents a financial transaction with details such as category, amount, date, income status, and notes.
/// </summary>
namespace PersonalFinanceTracker.Models
{
    /// <summary>
    /// A model class that defines a transaction entry with a unique identifier, category, amount, date, income status, and notes.
    /// Implements ObservableObject for property change notifications and uses SQLite attributes for database mapping.
    /// </summary>
    public partial class Transaction : ObservableObject
    {
        /// <summary>
        /// The primary key for the transaction entry, automatically generated as a new GUID.
        /// Marked with the [PrimaryKey] attribute for SQLite database mapping.
        /// </summary>
        [PrimaryKey]
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// The category of the transaction.
        /// </summary>
        [ObservableProperty]
        private string category;

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        [ObservableProperty]
        private decimal amount;

        /// <summary>
        /// The date and time when the transaction occurred.
        /// </summary>
        [ObservableProperty]
        private DateTime date;

        /// <summary>
        /// A boolean indicating whether the transaction is an income (true) or expense (false).
        /// </summary>
        [ObservableProperty]
        private bool isIncome;

        /// <summary>
        /// Optional notes or description for the transaction, observable for UI updates.
        /// </summary>
        [ObservableProperty]
        private string notes;
    }
}