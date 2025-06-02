using AccountLib.Abstractions;
using AccountLib.Infrastructure.Seed;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


// From AccountLib
builder.Services.AddAccountIdentity(builder.Configuration, "DefaultConnection");

builder.Services.AddSwaggerGen(options =>
{
	options.SwaggerDoc("v1", new()
	{
		Title = "Deployment API",
		Version = "v1"
	});
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	await ApplicationDbContextSeed.SeedAsync(services);
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
	c.SwaggerEndpoint("/swagger/v1/swagger.json", "Deployment API v1");
	c.RoutePrefix = string.Empty;
});

app.UseAuthorization();
app.MapControllers();
app.Run();