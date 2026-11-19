using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebToDoList.Data;

public class AppTodoDbContext : DbContext
{
    public AppTodoDbContext(DbContextOptions<AppTodoDbContext> options)
        : base(options)
    {

    }

    public DbSet<TaskEntity> Tasks { get; set; }
}
