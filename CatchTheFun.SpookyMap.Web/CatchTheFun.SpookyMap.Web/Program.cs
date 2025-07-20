using CatchTheFun.SpookyMap.Web.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// appsettings.json에 저장한 Google Maps API 키를 컨트롤러에서 꺼내 쓸 수 있도록 등록하는 코드
builder.Services.Configure<GoogleMapsOptions>(
    builder.Configuration.GetSection("GoogleMaps"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EventMap}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
