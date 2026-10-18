using OnlineClassManagementSystem.MvcApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient("WebAPI", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://127.0.0.1:7103/";
    baseUrl = baseUrl.Replace("localhost", "127.0.0.1");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});

builder.Services.AddHttpClient("ApiClient", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:BaseUrl"] ?? "https://127.0.0.1:7103/";
    baseUrl = baseUrl.Replace("localhost", "127.0.0.1");
    client.BaseAddress = new Uri(baseUrl);
    client.Timeout = TimeSpan.FromSeconds(15);
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
});
builder.Services.AddScoped<SubClassApiService>();
builder.Services.AddScoped<TimetableApiService>();
builder.Services.AddScoped<TeachPlanApiService>();
builder.Services.AddScoped<TeachingTrackingApiService>();
builder.Services.AddScoped<EnrollmentApiService>();
builder.Services.AddScoped<UserApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
