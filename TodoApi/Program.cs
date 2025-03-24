using TodoApi;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;

//יצירת תשתית להגדרת כל השירותים והתצורות ליצירת האפליקציה
var builder = WebApplication.CreateBuilder(args);


// הוספת שירותי DbContext עם מחרוזת החיבור
var connectionString = builder.Configuration.GetConnectionString("ToDoDB");
//הוספת השירות שיהיה זמין בכל מקום באפליקציה
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddSingleton<Item>();
//
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(x => x.AddPolicy("all", a => a.AllowAnyHeader()
.AllowAnyMethod().AllowAnyOrigin()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("all");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapGet("/", async (ToDoDbContext db) =>
{
    return await db.Items.ToListAsync();
});
// app.MapPost("/Register", async (string Name, string Password, ToDoDbContext db) =>
// {
//     User u = new User() { Name = Name, Password = Password };
//     db.Users.Add(u);
//     await db.SaveChangesAsync();
//    return  Results.Ok(u.Id);
// });
app.MapPost("/register/{Name}/{Password}", async (string Name, string Password, ToDoDbContext db) =>
{
    if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Password))
    {
        return Results.BadRequest("Name and password are required.");
    }

    User u = new User() { Name = Name, Password = Password };
    db.Users.Add(u);
    await db.SaveChangesAsync();
    return Results.Ok(u.Id);
});
app.MapPost("/login/{password}", async (string password, ToDoDbContext db) =>
{
    if (string.IsNullOrEmpty(password))
    {
        return Results.BadRequest("Name and password are required.");
    }

    var user = await db.Users.FirstOrDefaultAsync(x => x.Password == password);
    if (user == null)
    {
        return Results.Unauthorized();
    }

    return  Results.Ok(user.Id);
});



// app.MapGet("/sayH", () => ItemF.sayHellow());

app.MapGet("/allTasks", async (ToDoDbContext db , int userId) =>
{

    return await db.Items.Where(x => x.UserId == userId).ToListAsync();
});

app.MapPost("/addTask/{NameT}/{userId}", async (string NameT, int userId, ToDoDbContext db) =>
{
//0 == false
    var user = await db.Users.FindAsync(userId);
    if (user == null)
    {
        return Results.NotFound("User not found");
    }

    Item t = new Item() { Name = NameT, IsComplete = false  , UserId = userId};
    db.Items.Add(t);
    await db.SaveChangesAsync();
    return Results.Ok(t);
});
app.MapDelete("/deleteTask/{id}", async (int id, ToDoDbContext db) =>
{
    
    var item = await db.Items.FindAsync(id);
    if (item != null)
    {
        db.Items.Remove(item);
        await db.SaveChangesAsync();
    }
});

app.MapPut("/updateTask/{id}", async (int id, ToDoDbContext db) =>
{
    var Item = await db.Items.FindAsync(id);
    if (Item.IsComplete == true)
    {
        Item.IsComplete = false;
    }
    else
    {
        Item.IsComplete = true;
    }
    db.Items.Update(Item);
    await db.SaveChangesAsync();
    return Item;
});

app.Run();