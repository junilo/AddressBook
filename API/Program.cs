using API;
using API.Models;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();

app.MapGet("/contacts", async(IContactService contactService, string? q = "") =>
{
    return await contactService.GetContacts(q);
});

app.MapGet("/contact/{id}", async (IContactService contactService, int id) =>
{
    return await contactService.GetContact(id);
});

app.MapPost("/contact", async (IContactService contactService, Contact request) =>
{
    return await contactService.AddContact(request);
});

app.MapPut("/contact/{id}", async (IContactService contactService, int id, Contact request) =>
{
    request.Id = id;
    return await contactService.UpdateContact(request);
});

app.MapDelete("/contact/{id}", async (IContactService contactService, int id) =>
{
    return await contactService.DeleteContact(id);
});

app.Run();
