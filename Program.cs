using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddControllers(); // Add this line to enable controllers
builder.Services.AddHttpClient("VivaWallet", client =>
{
    var vivaWalletSettings = builder.Configuration.GetSection("VivaWalletSettings").Get<VivaWalletSettings>();
    client.BaseAddress = new Uri(vivaWalletSettings.BaseUrl);
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {vivaWalletSettings.ApiKey}");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add this line to map controllers
app.MapControllers();


app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

public class VivaWalletSettings
{
    public string ApiKey { get; set; }
    public string MerchantId { get; set; }
    public string BaseUrl { get; set; }
}