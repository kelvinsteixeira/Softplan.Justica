using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context;

namespace Softplan.Justica.GerenciadorProcessos.Helpers
{
    public static class DbInitializerHelper
    {
        public static void EnsureDatabase(IServiceScope serviceScope)
        {
            Task.Run(async () =>
            {
                var dbContext = serviceScope.ServiceProvider.GetService<IGerenciadorProcessosDbContext>();
                int attempts = 0;
                int maxAttempts = 10;

                async Task TryEnsureDatabase()
                {
                    try
                    {
                        await Task.Delay(10000);
                        dbContext.EnsureDatabase();
                    }
                    catch (Exception e)
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Falha ao inicializar o DbContext. O banco de dados subjacente parece não estar inicializado.");
                        sb.AppendLine($"Exception: {e.Message}");
                        Console.WriteLine(sb.ToString());

                        attempts++;
                        if (attempts < maxAttempts)
                        {
                            await TryEnsureDatabase();
                        }
                    }
                };

                await TryEnsureDatabase();
            });
        }
    }
}