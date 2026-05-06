using SignalRChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddScoped<ReservationController>();
builder.Services.AddHostedService<DailyReminderService>();
var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();            
 

app.MapGet("/", () => Results.Redirect("/index.html"));

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");

app.Run();
