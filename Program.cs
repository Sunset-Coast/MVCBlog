using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TechnicalBlog.Data;
using TechnicalBlog.Models;
using TechnicalBlog.Services;
using TechnicalBlog.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = DataUtility.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<BlogUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Custom Services
builder.Services.AddScoped<IEmailSender, EmailService>();

builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IBlogPostService, BlogPostService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

//Google Auth
builder.Services.AddAuthentication().AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? Environment.GetEnvironmentVariable("ClientId")!;
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? Environment.GetEnvironmentVariable("ClientSecret")!;
});

//Full control of mvc
builder.Services.AddMvc();


//Add SwashBuckle aka swagger

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Technical Blog",
        Version = "v1",
        Description = "Serve up Blog Data .Net 6 Apis",
        Contact = new OpenApiContact
        {
            Name = "J.Thomas",
            Email = "veparity@gmail.com",
            Url = new Uri("https://www.linkedin.com/in/jacqueline-thomas-0757221b4")
        }
    });
    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});


var app = builder.Build();

//Todo: Call Managed data

var scope = app.Services.CreateScope();
await DataUtility.ManageDataAysnc(scope.ServiceProvider);




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}



app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PublicAPI v1");
    c.InjectStylesheet("/css/swagger.css");
    c.InjectJavascript("/js/swagger.js");

    c.DocumentTitle = "Technical Blog API";
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "custom",
    pattern: "Content/{slug}",
    defaults: new { controller = "BlogPosts", action = "Details" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
