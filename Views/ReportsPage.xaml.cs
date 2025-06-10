using PersonalFinanceTracker.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Diagnostics;

/// <summary>
/// A ContentPage representing the user interface for displaying financial reports in the Personal Finance Tracker application.
/// </summary>
namespace PersonalFinanceTracker.Views
{
    /// <summary>
    /// A page class that provides the UI for viewing financial reports, including category and monthly spending charts.
    /// </summary>
    public partial class ReportsPage : ContentPage
    {
        private readonly ReportsViewModel _viewModel;

        /// <summary>
        /// Initializes a new instance of the ReportsPage with a specified view model.
        /// Sets up the page's UI components, binds the view model, and triggers asynchronous data loading and chart invalidation.
        /// </summary>
        /// <param name="vm">The ReportsViewModel instance to bind to this page.</param>
        public ReportsPage(ReportsViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            BindingContext = _viewModel;
            _ = LoadDataAndInvalidateAsync();
        }

        /// <summary>
        /// Asynchronously loads report data and sets up event handling for chart invalidation.
        /// Introduces a delay to allow data to load, subscribes to property changes, and initially invalidates chart surfaces.
        /// </summary>
        /// <returns>A Task representing the asynchronous operation.</returns>
        private async Task LoadDataAndInvalidateAsync()
        {
            Debug.WriteLine("ReportsPage: Waiting for data to load");
            await Task.Delay(100);
            _viewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(ReportsViewModel.CategorySpendings) ||
                    e.PropertyName == nameof(ReportsViewModel.MonthlySpendings))
                {
                    CategoryChart.InvalidateSurface();
                    MonthlyChart.InvalidateSurface();
                    Debug.WriteLine($"ReportsPage: Invalidated surfaces for {e.PropertyName}");
                }
            };
            CategoryChart.InvalidateSurface();
            MonthlyChart.InvalidateSurface();
            Debug.WriteLine("ReportsPage: Initial surfaces invalidated");
        }

        /// <summary>
        /// Handles the paint surface event for the category chart, rendering a bar chart of category spending.
        /// Clears the canvas, calculates bar heights based on maximum spending, and draws bars with category labels.
        /// </summary>
        /// <param name="sender">The sender of the event (typically the SKCanvasView).</param>
        /// <param name="e">The event arguments containing the surface and info for painting.</param>
        private void OnCategoryChartPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Debug.WriteLine("ReportsPage: OnCategoryChartPaintSurface called");
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);
            var info = e.Info;
            var width = info.Width;
            var height = info.Height;

            if (!_viewModel.CategorySpendings.Any())
            {
                Debug.WriteLine("ReportsPage: No category spendings data");
                return;
            }

            var maxAmount = _viewModel.CategorySpendings.Max(cs => cs.Amount);
            var barWidth = width / (_viewModel.CategorySpendings.Count() * 2);
            var colors = new[] { SKColors.Blue, SKColors.Green, SKColors.Red, SKColors.Purple };

            using var paint = new SKPaint { IsAntialias = true };
            for (int i = 0; i < _viewModel.CategorySpendings.Count(); i++)
            {
                var spending = _viewModel.CategorySpendings.ElementAt(i);
                var barHeight = maxAmount == 0 ? 0 : Math.Max(5, (float)(spending.Amount / maxAmount * (height - 70)));
                paint.Color = colors[i % colors.Length];
                canvas.DrawRect(i * barWidth * 2, height - barHeight, barWidth, barHeight, paint);
                paint.Color = SKColors.Black;
                using var font = new SKFont(SKTypeface.Default, 20);
                var x = i * barWidth * 2 + barWidth / 2;
                canvas.DrawText(spending.Category, x, height - 20, SKTextAlign.Center, font, paint);
            }
            Debug.WriteLine($"ReportsPage: Rendered CategoryChart with {_viewModel.CategorySpendings.Count()} items");
        }

        /// <summary>
        /// Handles the paint surface event for the monthly chart, rendering a line chart of monthly spending.
        /// Clears the canvas, calculates points based on maximum spending, draws lines, and adds rotated month labels.
        /// </summary>
        /// <param name="sender">The sender of the event (typically the SKCanvasView).</param>
        /// <param name="e">The event arguments containing the surface and info for painting.</param>
        private void OnMonthlyChartPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            Debug.WriteLine("ReportsPage: OnMonthlyChartPaintSurface called");
            var canvas = e.Surface.Canvas;
            canvas.Clear(SKColors.White);
            var info = e.Info;
            var width = info.Width;
            var height = info.Height;

            if (!_viewModel.MonthlySpendings.Any())
            {
                Debug.WriteLine("ReportsPage: No monthly spendings data");
                return;
            }

            var maxAmount = _viewModel.MonthlySpendings.Max(ms => ms.Amount);
            if (maxAmount == 0)
            {
                Debug.WriteLine("ReportsPage: Max amount is zero, no scaling possible");
                return;
            }
            var stepX = _viewModel.MonthlySpendings.Count() > 1 ? width / (_viewModel.MonthlySpendings.Count() - 1) : width;
            var points = _viewModel.MonthlySpendings
                .Select((ms, i) => new SKPoint(i * stepX, height - (float)(ms.Amount / maxAmount * (height - 70))))
                .ToArray();

            using var paint = new SKPaint { IsAntialias = true, Color = SKColors.Blue, StrokeWidth = 2 };
            for (int i = 1; i < points.Length; i++)
            {
                canvas.DrawLine(points[i - 1], points[i], paint);
            }
            paint.Color = SKColors.Black;
            using var font = new SKFont(SKTypeface.Default, 20);
            for (int i = 0; i < _viewModel.MonthlySpendings.Count(); i++)
            {
                var x = i * stepX;
                var monthText = _viewModel.MonthlySpendings.ElementAt(i).Month.ToString("MMM");
                canvas.Save();
                canvas.RotateDegrees(-45, x, height - 20);
                canvas.DrawText(monthText, x, height - 20, SKTextAlign.Left, font, paint);
                canvas.Restore();
            }
            Debug.WriteLine($"ReportsPage: Rendered MonthlyChart with {_viewModel.MonthlySpendings.Count()} items");
        }
    }
}