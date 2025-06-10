using Microsoft.Maui.Controls;
using PersonalFinanceTracker.ViewModels;
using PersonalFinanceTracker.Models;
using System.Threading.Tasks;
using System.Diagnostics;

/// <summary>
/// A custom behavior for a ProgressBar that dynamically updates its progress and color based on budget spending data.
/// </summary>
namespace PersonalFinanceTracker.Behaviors
{
    /// <summary>
    /// Represents a behavior that manages the progress and color of a ProgressBar based on budget data.
    /// </summary>
    public class ProgressBehavior : Behavior<ProgressBar>
    {
        /// <summary>
        /// Represents the progress bar used to display the progress of an operation.
        /// </summary>
        private ProgressBar _progressBar;

        /// <summary>
        /// Represents the main view model for the application.
        /// </summary>
        private MainViewModel _viewModel;

        /// <summary>
        /// Represents the category associated with the current instance.
        /// </summary>
        /// <remarks>This field is private and is used internally to store the category value.</remarks>
        private string _category;

        /// <summary>
        /// Called when the behavior is attached to a ProgressBar.
        /// Initializes the ProgressBar reference and attempts to retrieve the MainViewModel from the application context
        /// and sets up the category and progress update loop.
        /// </summary>
        /// <param name="bindable">The ProgressBar to which this behavior is attached.</param>
        protected override void OnAttachedTo(ProgressBar bindable)
        {
            base.OnAttachedTo(bindable);
            _progressBar = bindable;
            _viewModel = Application.Current.MainPage?.BindingContext as MainViewModel;
            Debug.WriteLine($"ProgressBehavior: Attached to ProgressBar at {DateTime.Now}, ViewModel: {_viewModel != null}, BindingContext: {_progressBar.BindingContext?.GetType().Name ?? "Null"}");
            if (_viewModel == null)
            {
                Debug.WriteLine("ProgressBehavior: MainViewModel is null, cannot proceed");
                return;
            }
            if (_progressBar.BindingContext is Budget budget)
            {
                _category = budget.Category;
                Debug.WriteLine($"ProgressBehavior: Category set to {_category}, Amount: {budget.Amount}, Budget: {budget}");
                _ = UpdateProgress();
            }
            else
            {
                Debug.WriteLine($"ProgressBehavior: Failed to get Budget from BindingContext, Context: {_progressBar.BindingContext}");
            }
        }

        /// <summary>
        /// Called when the behavior is detached from a ProgressBar.
        /// Cleans up references to prevent memory leaks.
        /// </summary>
        /// <param name="bindable">The ProgressBar from which this behavior is detached.</param>
        protected override void OnDetachingFrom(ProgressBar bindable)
        {
            base.OnDetachingFrom(bindable);
            _progressBar = null;
            _viewModel = null;
            Debug.WriteLine("ProgressBehavior: Detached from ProgressBar");
        }

        /// <summary>
        /// Asynchronously updates the ProgressBar's progress and color based on the budget spending data.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task UpdateProgress()
        {
            Debug.WriteLine($"ProgressBehavior: Starting UpdateProgress loop for {_category} at {DateTime.Now}");
            while (_progressBar.IsVisible)
            {
                try
                {
                    if (_viewModel == null || string.IsNullOrEmpty(_category))
                    {
                        Debug.WriteLine("ProgressBehavior: Invalid state - ViewModel or category is null");
                        await Task.Delay(5000);
                        continue;
                    }
                    var progress = await _viewModel.GetBudgetProgressAsync(_category);
                    Debug.WriteLine($"ProgressBehavior: Calculated progress for {_category} = {progress}% at {DateTime.Now}");
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        _progressBar.Progress = (float)progress / 100;
                        var colorConverter = (IValueConverter)Application.Current.Resources["ProgressToColorConverter"];
                        if (colorConverter != null)
                        {
                            _progressBar.ProgressColor = (Color)colorConverter.Convert(progress, typeof(Color), null, null);
                        }
                        else
                        {
                            Debug.WriteLine("ProgressBehavior: ColorConverter is null");
                        }
                        Debug.WriteLine($"ProgressBehavior: Updated ProgressBar for {_category} with Progress={_progressBar.Progress}, Color={_progressBar.ProgressColor} at {DateTime.Now}");
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"ProgressBehavior: Error in UpdateProgress - {ex.Message} at {DateTime.Now}");
                }
                await Task.Delay(10000); // Increased to 10 seconds to reduce main thread load
            }
        }
    }
}