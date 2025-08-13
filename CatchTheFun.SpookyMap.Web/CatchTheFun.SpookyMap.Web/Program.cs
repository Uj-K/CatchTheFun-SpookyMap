using CatchTheFun.SpookyMap.Web.Data;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ✅ 1) 서비스 등록은 전부 Build() 전에해야 하니 이게 제일 먼저옴
builder.Services.AddControllersWithViews();


// 그리고 DbContext 등록
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// 파일 업로드 크기 등 옵션 구성
builder.Services.Configure<FormOptions>(o =>
{
    o.MultipartBodyLengthLimit = 6 * 1024 * 1024;
});

// (선택) 파일 저장 서비스/기타 DI 등록할게 있다면 다 여기에 넣고

// 이제 빌드
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

// 새 템플릿 스타일(정적파일 매핑)
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=EventMap}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
