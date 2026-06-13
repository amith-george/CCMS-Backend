using CCMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Add API documentation
builder.Services.AddOpenApi();

// 2. Register Controllers
builder.Services.AddControllers();

// 3. Register the Database Context (Connecting it to SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); 
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Map the controller routes
app.MapControllers();

app.Run();