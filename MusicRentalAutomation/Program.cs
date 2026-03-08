using MusicRentalAutomation.Services;

namespace MusicRentalAutomation;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        var connectionString = Environment.GetEnvironmentVariable("MUSIC_RENTAL_DB")
            ?? "Host=localhost;Port=5432;Database=music_rental;Username=postgres;Password=postgres";

        ApplicationConfiguration.Initialize();

        var service = new RentalService(connectionString);
        service.InitializeDatabase();

        Application.Run(new MainForm(service));
    }
}
