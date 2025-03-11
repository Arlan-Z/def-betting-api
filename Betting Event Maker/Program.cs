using Betting_Event_Maker.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<JsonFileService>();

builder.Services.AddSingleton<EventService>();
builder.Services.AddHttpClient<EventService>();

builder.Services.AddControllers();
builder.Services.AddMvc(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});

var app = builder.Build();


// Configure the HTTP request pipeline.

app.MapControllers();
app.UseHttpsRedirection();

app.Run();
