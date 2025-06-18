using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonalFinanceTracker.Services;
using PersonalFinanceTracker.ViewModels;
using PersonalFinanceTracker.Views;
using SkiaSharp.Views.Maui.Controls;

/// <summary>
/// A static class responsible for configuring and building the MAUI application for the Personal Finance Tracker.
/// </summary>
namespace PersonalFinanceTracker
{
    /// <summary>
    /// A static utility class that provides the entry point for configuring the MAUI application.
    /// Includes methods to create and customize the app instance with services, pages, and handlers.
    /// </summary>
    public static class MauiProgram
    {
        /// <summary>
        /// Creates and configures a new MAUI application instance with all necessary services and settings.
        /// </summary>
        /// <returns>A fully configured MauiApp instance ready for execution.</returns>
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register SKCanvasView handler
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler<SKCanvasView, SkiaSharp.Views.Maui.Handlers.SKCanvasViewHandler>();
            });

            // Register services
            builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
            builder.Services.AddSingleton<DataService>();

            // Register view models
            builder.Services.AddSingleton<MainViewModel>();
            builder.Services.AddTransient<AddTransactionViewModel>();
            builder.Services.AddTransient<AddBudgetViewModel>();
            builder.Services.AddTransient<ReportsViewModel>();
            builder.Services.AddTransient<DetailViewModel>();
            builder.Services.AddTransient<EditTransactionViewModel>(); // Added EditTransactionViewModel

            // Register pages
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AddTransactionPage>();
            builder.Services.AddTransient<AddBudgetPage>();
            builder.Services.AddTransient<ReportsPage>();
            builder.Services.AddTransient<DetailPage>();
            builder.Services.AddTransient<EditTransactionPage>(); // Added EditTransactionPage

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}