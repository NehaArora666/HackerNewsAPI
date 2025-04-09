var builder = WebApplication.CreateBuilder(args);



builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<HackerNewsService>();  
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevClient",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")  
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddLogging();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();


app.Logger.LogInformation("Application started.");


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngularDevClient");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();