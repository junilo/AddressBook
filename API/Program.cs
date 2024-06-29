using API;
using API.Models;
using Microsoft.EntityFrameworkCore;

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

app.MapGet("/contacts", async(AppDbContext dbContext, string? q = "") =>
{
    var query = dbContext.Contacts.Where(w => !w.IsDeleted);

    if (!string.IsNullOrEmpty(q))
    {
        query = query.Where(
            w => (w.Name != null && EF.Functions.Like(w.Name.ToLower(), $"%{q.ToLower()}%")) ||
            (w.Email != null && EF.Functions.Like(w.Email.ToLower(), $"%{q.ToLower()}%")) ||
            (w.Phone != null && EF.Functions.Like(w.Phone.ToLower(), $"%{q.ToLower()}%")) ||
            (w.Address != null && EF.Functions.Like(w.Address.ToLower(), $"%{q.ToLower()}%"))
        );
    }

    return await query.OrderBy(o => o.Id)
        .Select(s => new Contact
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address
        })
        .ToListAsync();
});

app.MapGet("/contact/{id}", async (AppDbContext dbContext, int id) =>
{
    if (id <= 0)
    {
        return null;
    }

    var contact = await dbContext.Contacts
        .Where(w => w.Id == id && !w.IsDeleted)
        .OrderBy(o => o.Id)
        .Select(s => new Contact
        {
            Id = s.Id,
            Name = s.Name,
            Email = s.Email,
            Phone = s.Phone,
            Address = s.Address
        })
        .FirstOrDefaultAsync();

    return contact;
});

app.MapPost("/contact", async (AppDbContext dbContext, Contact contact) =>
{
    dbContext.Contacts.Add(contact);
    await dbContext.SaveChangesAsync();

    return contact;
});

app.MapPut("/contact/{id}", async (AppDbContext dbContext, int id, Contact request) =>
{
    if (id == 0)
    {
        return null;
    }

    var contact = await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    if (contact == null)
    {
        return null;
    }

    contact.Address = request.Address;
    contact.Phone = request.Phone;
    contact.Email = request.Email;
    contact.Name = request.Name;
    
    await dbContext.SaveChangesAsync();

    return contact;
});

app.MapDelete("/contact/{id}", async (AppDbContext dbContext, int id) =>
{
    if (id == 0)
    {
        return false;
    }

    var contact = await dbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
    if (contact == null)
    {
        return false;
    }

    // We are using soft-delete approach instead of hard deletion
    contact.IsDeleted = true;

    await dbContext.SaveChangesAsync();

    return true;
});

app.Run();
