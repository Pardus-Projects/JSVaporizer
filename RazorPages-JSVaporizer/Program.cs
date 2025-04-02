using JSVaporizer;
using Microsoft.Extensions.FileProviders;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

// BEGIN Serving WASM assemblies ========================================================================

// Required to serve WASM assembly to client.
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true
});

// Extract embedded jsvwasm JavaScript files from JSVaporizer.NET.8 assembly.
Assembly ass = typeof(JSVapor).GetTypeInfo().Assembly;
EmbeddedFileProvider embProv = new EmbeddedFileProvider(ass, "JSVaporizer.NET.8.jsvwasm");
//var verifyFiles = embProv.GetDirectoryContents(""); // For debugging breakpoint
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = embProv
});

// END Serving WASM assemblies ========================================================================

app.Run();
