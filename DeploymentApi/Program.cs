using AccountLib.Abstractions;
using AccountLib.Configuration;
using AccountLib.Contracts;
using AccountLib.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Account Identity -> From AccountLib
AccountIdentityParams accountIdentityParams = new()
{
	JwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()!
};
builder.Services.AddAccountIdentity(accountIdentityParams);



builder.Services.AddSwaggerGen(setup =>
{
	var jwtSecurityScheme = new OpenApiSecurityScheme
	{
		BearerFormat = "JWT",
		Name = "JWT Authentication",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = JwtBearerDefaults.AuthenticationScheme,
		Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

		Reference = new OpenApiReference
		{
			Id = JwtBearerDefaults.AuthenticationScheme,
			Type = ReferenceType.SecurityScheme
		}
	};

	setup.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

	setup.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{ jwtSecurityScheme, Array.Empty<string>() }
	});
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient(s => s.GetService<IHttpContextAccessor>().HttpContext.User);

var origins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
{
	builder.AllowAnyMethod()
			.AllowAnyHeader().WithExposedHeaders("X-Content-Type-Options")
			.SetIsOriginAllowed(origin => origins.Contains("all") || origins
			.Select(x => x.ToLower()).Contains(origin.ToLower()))
			.AllowCredentials();
}));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthorization();
app.MapControllers();
app.Run();