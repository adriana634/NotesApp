using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using NotesApp.DatabaseContext;
using NotesApp.Repositories;
using NotesApp.Services;
using NotesApp.ViewModels;
using NotesApp.Views;

namespace NotesApp;

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

		string dbPath = Path.Combine(FileSystem.AppDataDirectory, "notes.db3");
        builder.Services.AddSingleton<IDbContext>(serviceProvider => ActivatorUtilities.CreateInstance<DbContext>(serviceProvider, dbPath));
        builder.Services.AddSingleton<INoteRepository, NoteRepository>();

        builder.Services.AddSingleton<IAppInfoService, AppInfoService>();
        builder.Services.AddSingleton<ILauncherService, LauncherService>();
        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<INoteSelectionService, NoteSelectionService>();
        builder.Services.AddSingleton<IUpdateNoteViewModelFactory, UpdateNoteViewModelFactory>();

        builder.Services.AddSingleton<IMessenger>(WeakReferenceMessenger.Default);

        builder.Services.AddSingleton<AllNotesPage>();
        builder.Services.AddSingleton<NotesViewModel>();

        builder.Services.AddTransient<NewNotePage>();
        builder.Services.AddTransient<NewNoteViewModel>();

        builder.Services.AddTransient<UpdateNotePage>();
        builder.Services.AddTransient<UpdateNoteViewModel>();

        builder.Services.AddSingleton<AboutPage>();
        builder.Services.AddSingleton<AboutViewModel>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
