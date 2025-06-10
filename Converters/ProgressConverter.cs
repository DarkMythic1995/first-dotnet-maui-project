using PersonalFinanceTracker.ViewModels;
using System.Diagnostics;
using System.Globalization;

/// <summary>
/// A value converter that transforms a budget category string into a progress value (0-1) for use in a ProgressBar.
/// </summary>
namespace PersonalFinanceTracker.Converters
{
    /// <summary>
    /// Converts a category string to a progress value based on budget data from the MainViewModel.
    /// </summary>
    public class ProgressConverter : IValueConverter
    {
        /// <summary>
        /// Converts a category string to a progress value (0-1) by querying the MainViewModel.
        /// </summary>
        /// <param name="value">The category string representing the budget category.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The MainViewModel instance used to calculate progress.</param>
        /// <param name="culture">The culture to use in the conversion.</param>
        /// <returns>A decimal value between 0 and 1 representing the progress, or 0 on error.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string category)
            {
                Debug.WriteLine($"ProgressConverter: Category = {category}");
                if (parameter is MainViewModel vm)
                {
                    Debug.WriteLine($"ProgressConverter: Found MainViewModel, calculating progress for {category}");
                    try
                    {
                        var progressTask = Task.Run(() => vm.GetBudgetProgressAsync(category));
                        var progress = progressTask.Result; // Blocking for now, will switch to Behavior
                        Debug.WriteLine($"ProgressConverter: Progress for {category} = {progress}%");
                        return progress / 100; // Convert to 0-1 range for ProgressBar
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"ProgressConverter: Error calculating progress - {ex.Message}");
                        return 0m;
                    }
                }
                else
                {
                    Debug.WriteLine("ProgressConverter: MainViewModel not passed via parameter");
                    return 0m;
                }
            }
            Debug.WriteLine("ProgressConverter: Invalid value type");
            return 0m;
        }

        /// <summary>
        /// Not implemented, as this converter is intended for one-way binding only.
        /// Throws a NotImplementedException if called.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">The type to convert back to.</param>
        /// <param name="parameter">Optional parameter.</param>
        /// <param name="culture">The culture to use in the conversion.</param>
        /// <returns>Throws NotImplementedException.</returns>
        /// <exception cref="NotImplementedException">Always thrown, as ConvertBack is not supported.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}