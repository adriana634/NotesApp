using Microsoft.Extensions.Logging;
using NotesApp.Repositories;

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
		builder.Services.AddSingleton<NoteRepository>(serviceProvider => ActivatorUtilities.CreateInstance<NoteRepository>(serviceProvider, dbPath));

#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
