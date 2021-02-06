using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DMR_API._Services.Interface;
using DMR_API._Services.Services;
using DMR_API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace TodolistScheduleService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //.ConfigureLogging(loggerFactory => {
                //    var path = Directory.GetCurrentDirectory();
                //    loggerFactory.AddFile($"{path}\\Logs\\Log.txt");
                //})
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    Appsettings appsettings = configuration.GetSection("AppSettings").Get<Appsettings>();
                   var _builder = new DbContextOptionsBuilder<DataContext>()
                   .UseSqlServer(appsettings.DefaultConnection);
                           var _context = new DataContext(_builder.Options);
                    services.AddSingleton<DataContext>(_context);
                    services.AddDbContext<DataContext>(options => options.UseSqlServer(appsettings.DefaultConnection));
                    services.AddHostedService<Todolist>();
                });
    }
}
