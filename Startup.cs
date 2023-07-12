using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NWebsec.AspNetCore.Middleware;

namespace webapp.Pages
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(); // Or any other services you have

            services.AddNWebsec(options =>
            {
                options.Xfo.Deny(); // Deny framing by other websites
                options.XContentTypeOptions.NoSniff(); // Prevent MIME type sniffing
                options.XDownloadOptions.NoOpen(); // Prevent file downloads from opening automatically
                // Add more headers and options as needed
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseNWebsecMiddleware(); // Add NWebsec middleware

            app.UseXContentTypeOptions();
            app.UseXfo(options => options.Deny());
            app.UseXXssProtection(options => options.EnabledWithBlockMode());
            app.UseReferrerPolicy(opts => opts.NoReferrer());

            app.UseCsp(options => options
                .BlockAllMixedContent()
                .DefaultSources(s => s.Self())
                .FontSources(s => s.Self()
                    .CustomSources("https://testwebsite001.sealz-dit.se.com")
                )
                .FrameAncestors(s => s.None())
                .FrameSources(s => s.None())
                .UpgradeInsecureRequests()
            );

            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.Always,
                MinimumSameSitePolicy = SameSiteMode.Strict,
                Secure = CookieSecurePolicy.Always
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
