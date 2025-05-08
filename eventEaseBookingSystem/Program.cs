using eventEaseBookingSystem.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register MongoDbContext with dependency injection
builder.Services.AddSingleton(sp => new MongoDbContext(
    "mongodb+srv://cldvDotNetApp:453sONHSos6enfAP@adad0701.w5forrw.mongodb.net/?retryWrites=true&w=majority&appName=ADAD0701",
    "EventEaseDB"));

var app = builder.Build();

// Seed the database with sample venues
var dbContext = app.Services.GetRequiredService<MongoDbContext>();
dbContext.SeedDatabase();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Venues}/{action=Index}/{id?}");

app.Run();
