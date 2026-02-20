namespace Brotherhood_Portal.API.Extensions
{
    public static class SecurityMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurity(
            this IApplicationBuilder app,
            IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            return app;
        }
    }

}
