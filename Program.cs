using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Notes;
using Notes.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentityApiEndpoints<IdentityUser>(opt =>
    {
        opt.Password.RequiredLength = 1;
        opt.Password.RequireNonAlphanumeric = false;
        opt.Password.RequireUppercase = false;
        opt.Password.RequireLowercase = false;
        opt.Password.RequiredUniqueChars = 0;
        opt.Password.RequireDigit = false;
        opt.Password.RequiredUniqueChars = 0;
        opt.SignIn.RequireConfirmedEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});


builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Notes API",
        Version = "v1",
        Description = "Notes uygulaması için API dökümantasyonu."
    });
    c.EnableAnnotations();
});

builder.Services.Configure<SecurityStampValidatorOptions>(opt => opt.ValidationInterval = TimeSpan.Zero);

var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();

builder.Services
    .AddFluentEmail(smtpSettings.FromEmail, smtpSettings.FromName)
    .AddRazorRenderer()
    .AddSmtpSender(new SmtpClient(smtpSettings.Host, smtpSettings.Port)
    {
        EnableSsl = true,
        Credentials = new NetworkCredential(smtpSettings.Username, smtpSettings.Password),
    });

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

app.UseCors("AllowFrontend");

app.MapOpenApi();
app.MapControllers();

app.MapScalarApiReference();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await IdentitySeedData.SeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Admin seeding error: {ex.Message}");
    }
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1"); });

app.UseAuthorization();

app.MapGroup("Auth").MapIdentityApi<IdentityUser>().WithTags("Auth");
app.Run();