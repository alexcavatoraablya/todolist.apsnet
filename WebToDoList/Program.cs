using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebToDoList.Data;
using WebToDoList.Data.Identity;

var builder = WebApplication.CreateBuilder(args);

//налаштовує підключення до бази даних PostgreSQL
string strConn = builder.Configuration.GetConnectionString("MyConnectionToDoList");

builder.Services.AddDbContext<AppTodoDbContext>(opt =>
    opt.UseNpgsql(strConn));

builder.Services.AddIdentity<UserEntity, RoleEntity>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
})
    .AddEntityFrameworkStores<AppTodoDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

using(var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider
        .GetService<UserManager<UserEntity>>();

    var roleManager = scope.ServiceProvider
        .GetService<RoleManager<RoleEntity>>();
    if(roleManager.Roles.Any() == false)
    {
        string[] roles = { "Admin", "User" };
        foreach (string name in roles)
        {
            RoleEntity role = new RoleEntity
            {
                Name = name
            };
            var result = await roleManager.CreateAsync(role);
            if (result.Succeeded)
                Console.WriteLine($"--Роль {name} успішно створено--");
            else
                Console.WriteLine($"!!!Помилка при створені ролі {name}!!!");
        }
    }

    // Отримуємо UserManager з DI-контейнера (наприклад, поруч із roleManager)
    if (!userManager.Users.Any())
    {
        var adminUser = new UserEntity
        {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            EmailConfirmed = true // Одразу підтверджуємо пошту, щоб не було проблем зі входом
        };

        var result = await userManager.CreateAsync(adminUser, "Admin123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
            Console.WriteLine($"--Користувача {adminUser.Email} успішно створено з роллю Admin--");
        }
        else
        {
            Console.WriteLine($"!!!Помилка створення користувача {adminUser.Email}!!!");
        }

        var regularUser = new UserEntity
        {
            UserName = "user@gmail.com",
            Email = "user@gmail.com",
            EmailConfirmed = true
        };

        var userResult = await userManager.CreateAsync(regularUser, "User123!");
        if (userResult.Succeeded)
        {
            await userManager.AddToRoleAsync(regularUser, "User");
            Console.WriteLine($"--Користувача {regularUser.Email} успішно створено з роллю User--");
        }
        else
        {
            Console.WriteLine($"!!!Помилка створення користувача {regularUser.Email}!!!");
        }
    }

}


app.Run();
