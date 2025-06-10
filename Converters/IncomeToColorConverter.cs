using System.Globalization;

/// <summary>
/// A value converter that transforms a boolean IsIncome property into a corresponding color.
/// </summary>
namespace PersonalFinanceTracker.Converters;

/// <summary>
/// Converts IsIncome boolean to color (Green for income, Red for expense)
/// </summary>
public class IncomeToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a boolean IsIncome value to a color.
    /// Returns Green if the value is true (income), Red if false (expense).
    /// </summary>
    /// <param name="value">The boolean value indicating if the transaction is income.</param>
    /// <param name="targetType">The type of the binding target property (expected to be Color).</param>
    /// <param name="parameter">Optional parameter (not used in this implementation).</param>
    /// <param name="culture">The culture to use in the conversion (not used in this implementation).</param>
    /// <returns>A Color object (Green for true, Red for false).</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (bool)value ? Colors.Green : Colors.Red;
    }

    /// <summary>
    /// Not implemented, as this converter is intended for one-way binding only.
    /// Throws a NotImplementedException if called.
    /// </summary>
    /// <param name="value">The value to convert back (not used).</param>
    /// <param name="targetType">The type to convert back to (not used).</param>
    /// <param name="parameter">Optional parameter (not used).</param>
    /// <param name="culture">The culture to use in the conversion (not used).</param>
    /// <returns>Throws NotImplementedException.</returns>
    /// <exception cref="NotImplementedException">Always thrown, as ConvertBack is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}