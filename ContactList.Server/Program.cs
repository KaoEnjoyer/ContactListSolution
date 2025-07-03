using System.Text;
using ContactList.Users;
using ContactList.Contacts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<IUsersDatabase, UsersDatabase>();
builder.Services.AddSingleton<IContactsDatabase, ContactsDatabase>();

builder.Services.AddSingleton<ITokens, Tokens>();
//builder.Services.AddSingleton<ILogger, FileLogger>();

    

builder.Services.AddCors(c =>
{
    c.AddPolicy("Policy", policy => policy
        .AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "dadawdawdawdaadada",
            ValidAudience = "dadawdwadawdawdawdawda",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("aadawdawdadkadkajdhakdjadadjawdjawdaa"))
        };
    });

WebApplication app = builder.Build();

//app.UseResponseMeasuree();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowAnyOrigin());

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.UseAuthentication();
app.UseAuthorization();


app.Run();

