using BooksApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.WindowsServices;
using System.Text.Json.Serialization;

var options = new WebApplicationOptions()
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
};
var builder = WebApplication.CreateBuilder(options);
builder.Services.AddDbContext<BooksDbContext>();
builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull ); ;
builder.Host.UseWindowsService();
var app = builder.Build();

{
    try
    {
        using var scope = app.Services.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<BooksDbContext>();
        dbContext.Database.Migrate();
    }
    catch
    {
    }
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
