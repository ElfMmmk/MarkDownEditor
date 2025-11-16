using Microsoft.Extensions.Logging;
using CSharpMobileApp.Services;
using CSharpMobileApp.ViewModels;
using CSharpMobileApp.Views;

namespace CSharpMobileApp;

public static class MauiProgram
{
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

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddSingleton<TaskService>();

		builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<HomePage>();

		builder.Services.AddTransient<ToDoListViewModel>();
		builder.Services.AddTransient<ToDoListPage>();

		builder.Services.AddTransient<AddToDoViewModel>();
		builder.Services.AddTransient<AddToDoPage>();

		builder.Services.AddTransient<EditToDoViewModel>();
		builder.Services.AddTransient<EditToDoPage>();

		return builder.Build();
	}
}
