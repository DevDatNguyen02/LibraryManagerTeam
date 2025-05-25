using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LibTeam.Models;
using LibTeam.DbContext;

var builder = WebApplication.CreateBuilder(args);

// 1. DbContext
builder.Services.AddDbContext<DataContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("LibraryAPI")));

// 2. Identity (phải gọi AddEntityFrameworkStores)
//    – Bỏ hết mọi yêu cầu về password
builder.Services.AddIdentity<AppUserModel, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
    options.Password.RequiredUniqueChars = 0;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

// 3. MVC
builder.Services.AddControllersWithViews();

// 4. Cookie settings
builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.LoginPath = "/Login/Index";
    opts.AccessDeniedPath = "/Login/Index";
});

var app = builder.Build();

// 5. Create roles on startup
await using (var scope = app.Services.CreateAsyncScope())
{
    var rm = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var um = scope.ServiceProvider.GetRequiredService<UserManager<AppUserModel>>();

    // Tạo roles nếu chưa tồn tại
    foreach (var r in new[] { "QuanTriVien", "NhanVien" })
        if (!await rm.RoleExistsAsync(r))
            await rm.CreateAsync(new IdentityRole(r));

    // Tạo tài khoản QuanTriVien mặc định nếu chưa tồn tại
    var adminUsername = "admin";
    var adminUser = await um.FindByNameAsync(adminUsername);
    if (adminUser == null)
    {
        adminUser = new AppUserModel
        {
            UserName = adminUsername,
            Email = "admin@libteam.com",
            PhoneNumber = "0123456789",
            EmailConfirmed = true
        };
        var result = await um.CreateAsync(adminUser, "Admin@123");
        if (result.Succeeded)
        {
            await um.AddToRoleAsync(adminUser, "QuanTriVien");
        }
    }
}

// 6. Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// 7. Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Index}/{id?}");

app.Run();