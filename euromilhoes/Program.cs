using euromilhoes.Services;
using euromilhoes.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:" + Environment.GetEnvironmentVariable("PORT"));

// Add services to the container.
builder.Services.AddSingleton<IEuromilhoesService, EuromilhoesService>();
builder.Services.AddSingleton<IEuromilhoesCrawlerService, EuromilhoesCrawlerService>();
builder.Services.AddSingleton<PeriodicHostedService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<PeriodicHostedService>());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient("euromilhoes", c =>
{
    c.BaseAddress = new Uri("https://www.euromillones.com/");
    c.Timeout = TimeSpan.FromSeconds(10);
});

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapRazorPages();
    endpoints.MapControllers();
});

app.Run();
