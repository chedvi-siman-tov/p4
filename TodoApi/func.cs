using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TodoApi;
using Microsoft.EntityFrameworkCore;

namespace TodoApi;
public class Func
{
    public static async Task<List<Item>> SelectAll()
    {
        ToDoDbContext context = new ToDoDbContext();
        return await context.Items.ToListAsync();
    }

    public static async Task Insert(Item t)
    {
        ToDoDbContext context = new ToDoDbContext();
        context.Items.Add(t);
        await context.SaveChangesAsync();
    }

    public static async Task Deleat(int id)
    {
        ToDoDbContext context = new ToDoDbContext();
        var t = await context.Items.FindAsync(id);
        context.Items.Remove(t);
        await context.SaveChangesAsync();
    }

    public static async Task Update(Item t)
    {
        ToDoDbContext context = new ToDoDbContext();
        context.Items.Update(t);
        await context.SaveChangesAsync();
    }

}

