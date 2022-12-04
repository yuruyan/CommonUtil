using SimpleFileSystemServer;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

var app = builder.Build();
Global.Initialize(app.Configuration);
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/error");
    app.UseDeveloperExceptionPage();
    app.Use(async (context, next) => {
        context.Response.Headers["Access-Control-Allow-Origin"] = "*";
        await next();
    });
}
app.Run();
