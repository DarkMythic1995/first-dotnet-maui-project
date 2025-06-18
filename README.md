# Personal Finance Tracker - Week 2 Intro to Mobile Class

## Overview
I created this Personal Finance Tracking application for my Intro to Mobile class at Mid-State Technical College (MSTC) this summer. It’s a .NET MAUI application designed to help users manage their finances by tracking transactions and budgets. The goal is to provide a user-friendly interface to add transactions and budgets, view detailed reports with custom charts, and navigate between pages seamlessly. This Week 2 update includes enhancements based on instructor feedback and new features developed today.

## Features
- Add and manage transactions (income and expenses) with categories and notes, now with improved accessibility and validation feedback.
- Set and track monthly budgets for various categories, with a stable "Recent Transactions" label alignment.
- View detailed transaction information on the DetailPage.
- Generate reports with bar charts for category spending and line charts for monthly trends.
- Responsive design across Android, iOS, macCatalyst, and Windows platforms, with enhanced button layouts.
- Theme (light/dark) toggle with updated background images for a more financial app-like aesthetic.

## Usage
- Navigate to the **MainPage** to view transactions and budgets, with a static "Recent Transactions" label.
- Use **Add Transaction** and **Add Budget** pages to input new entries, featuring wider "Save" and "Cancel" buttons with increased spacing.
- Check **ReportsPage** for visual insights into your finances.
- View transaction details on the **DetailPage** and edit them with improved validation.

## Changes from Week 1
### Instructor Feedback Addressed
- **Switch Labeling for Accessibility**: Improved the Expense/Income switch with `AutomationProperties.Name` for screen readers, clarifying its function ("Toggle Income/Expense: Off for Expense, On for Income"). Added a visible description label for better user understanding.
- **Enhance Button Layout and Spacing**: Wrapped "Save" and "Cancel" buttons in a `HorizontalStackLayout` with increased `Spacing` (20) and `Padding` (15), and added `WidthRequest` (120) to buttons on the Add Transaction page for a clearer separation and consistent appearance.
- **Consistent Input Validation Feedback**: Added validation messages near inputs on the Add Transaction and Edit Transaction pages (e.g., under `Entry` for amount), using a `Label` bound to `ValidationMessage` to display errors immediately. Integrated `IsValid` binding cues are planned for future updates.

### Week 2 Enhancements
- **UI Improvements**: Adjusted the "Recent Transactions" label alignment on MainPage with `Margin` and `VerticalOptions` for a static position. Increased width and spacing for buttons on AddTransactionPage.
- **Code Documentation**: Added comprehensive XML comments to `MainPage`, `MainViewModel`, `EditTransactionViewModel`, and `DataService` classes for better maintainability.
- **Accessibility**: Enhanced switch accessibility with descriptive labels and automation properties.
- **Theme Updates**: Replaced background images with financial app-inspired designs ("dark_background_2.png" and "light_background_1.png") for a more creative and professional look.

## Demo
Here are links to my YouTube walkthroughs:
- [Week 1 Personal Finance Tracker Demo](https://youtu.be/LqpPsed4ooM)
- [Week 2 Personal Finance Tracker Update](https://youtu.be/ZOax53qnHuU)

## Technologies Used
- **.NET MAUI**: Cross-platform framework for building the app.
- **CommunityToolkit.Mvvm**: For MVVM pattern implementation.
- **SQLite-net-pcl**: For local database management.
- **SkiaSharp**: For custom chart rendering in reports.
- **Microsoft.Maui.Networking**: For network connectivity checks.

## License
[MIT License]

## Acknowledgments
- Thanks to my instructor Brent Presley at Mid-State Technical College for valuable feedback.
- Additional thanks to the xAI, Codecademy, and other resources for guidance during development.
