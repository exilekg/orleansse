namespace BlackRock.OrleansStockExchange.WebAPI
{
    public static class ServicesExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IWebHostEnvironment environment, IConfiguration config)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            if (environment.IsDevelopment())
            {
                services
                    .AddSignalR();
            }
            else
            {
                services
                    .AddSignalR()
                    .AddAzureSignalR(config["SignalR:ConnectionString"]);
            }


            return services.AddCors(
                options => options.AddDefaultPolicy(
                    policy => policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .SetIsOriginAllowed((host) => true)
                          .AllowCredentials()));
        }
    }
}
