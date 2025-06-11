using AccountLib.Abstractions;
using AccountLib.Configuration;
using AccountLib.Contracts;
using AccountLib.Infrastructure.Seed;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Account Identity -> From AccountLib
AccountIdentityParams accountIdentityParams = new()
{
	ConfigurationManager = builder.Configuration,
	ConnectionString = "DefaultConnection",
	JwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!
};
builder.Services.AddAccountIdentity(accountIdentityParams);

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