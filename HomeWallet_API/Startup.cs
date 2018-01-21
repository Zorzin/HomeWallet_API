using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeWallet_API.Logic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using HomeWallet_API.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace HomeWallet_API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
            });
            services.AddCors();
            services.AddDbContext<DBContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DBContext")));
            services.AddScoped<IReceiptHelper, ReceiptHelper>();
            services.AddScoped<IShopHelper, ShopHelper>();
            services.AddScoped<IProductHelper, ProductHelper>();
            services.AddScoped<ICategoryHelper, CategoryHelper>();
            services.AddScoped<IPlanHelper, PlanHelper>();
            services.AddScoped<ISummaryHelper, SummaryHelper>();
            services.AddScoped<ICategorySummaryHelper, CategorySummaryHelper>();
            services.AddScoped<IPlanSummaryHelper, PlanSummaryHelper>();
            services.AddScoped<IProductSummaryHelper, ProductSummaryHelper>();
            services.AddScoped<IShopSummaryHelper, ShopSummaryHelper>();
            services.AddScoped<IDbHelper, DbHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
            app.UseMvc();
        }
    }
}
