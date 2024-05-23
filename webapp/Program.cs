var builder = WebApplication.CreateBuilder(args);
builder.WebHost
    .UseKestrel(option => option.AddServerHeader = false);
    

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddHsts(options =>
{
    options.Preload = true;
    options.IncludeSubDomains = true;
    options.MaxAge = TimeSpan.FromDays(365);

});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Xss-Protection", "1");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Cache-control", "public, max-age=86400");
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; object-src 'self'; base-uri 'self'; script-src 'self'; frame-src 'self'; frame-ancestors 'none'; form-action 'none'; report-to none; report-uri https://testwebsite001.sealz-dit.se.com;");
    context.Response.Headers.Add("Feature-Policy",
    "vibrate 'self' ; " +
    "camera 'self' ; " +
    "microphone 'self' ; " +
    "speaker 'self' https://testwebsite001.sealz-dit.se.com ;" +
    "geolocation 'self' ; " +
    "gyroscope 'self' ; " +
    "magnetometer 'self' ; " +
    "midi 'self' ; " +
    "sync-xhr 'self' ; " +
    "push 'self' ; " +
    "notifications 'self' ; " +
    "fullscreen '*' ; " +
    "payment 'self' ; ");

    context.Response.Headers.Add(
    "Content-Security-Policy-Report-Only",
    "default-src 'self'; " +
    "script-src-elem 'self'" +
    "style-src-elem 'self'; " +
    "img-src 'self'; http://www.w3.org/ https://testwebsite001.sealz-dit.se.com" +
    "font-src 'self'" +
    "media-src 'self'" +
    "frame-src 'self'" +
    "connect-src "

    );
    await next();
});


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
