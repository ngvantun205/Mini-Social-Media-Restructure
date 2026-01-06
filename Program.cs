using CloudinaryDotNet;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Mini_Social_Media.Repository;
using Mini_Social_Media.Services.Background;
using System.Security.Claims;
using System.Text;

namespace Mini_Social_Media {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            var cloudinarySettings = builder.Configuration.GetSection("CloudinarySettings");

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddControllersWithViews();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IPostRepository, PostRepository>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ILikeRepository, LikeRepository>();
            builder.Services.AddScoped<IFollowRepository, FollowRepository>();
            builder.Services.AddScoped<IHashtagRepository, HashtagRepository>();
            builder.Services.AddScoped<IPostMediaRepository, PostMediaRepository>();
            builder.Services.AddScoped<INotificationsRepository, NotificationsRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IStoryRepository, StoryRepository>();
            builder.Services.AddScoped<IStoryArchiveRepository, StoryArchiveRepository>();
            builder.Services.AddScoped<IShareRepository, ShareRepository>();
            builder.Services.AddScoped<IAdRepository, AdRepository>();

            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<IUploadService, UploadService>();
            builder.Services.AddScoped<ILikeService, LikeService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<INotificationsService, NotificationsService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<IHashtagService, HashtagService>();
            builder.Services.AddScoped<IFollowService, FollowService>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IReportService, ReportService>();
            builder.Services.AddScoped<IStoryService, StoryService>();
            builder.Services.AddScoped<IShareService, ShareService>();
            builder.Services.AddScoped<IAdService, AdService>();

            builder.Services.AddTransient<IEmailService, EmailService>();

            builder.Services.AddHostedService<StoryArchiverService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options => {
                options.IdleTimeout = TimeSpan.FromMinutes(15);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


            //builder.Services.Configure<SecurityStampValidatorOptions>(options =>
            //{
            //    options.ValidationInterval = TimeSpan.Zero;
            //});

            builder.Services.AddSignalR();

            builder.Services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                        )
                    };
                    options.Events = new JwtBearerEvents {
                        OnMessageReceived = context => {
                            context.Token = context.Request.Cookies["jwt"];
                            return Task.CompletedTask;
                        }
                    };
                });
            builder.Services.AddSingleton(x => {
                var config = cloudinarySettings.Get<CloudinarySettings>();
                return new Cloudinary(new Account(
                    config.CloudName,
                    config.ApiKey,
                    config.ApiSecret
                ));
            });
            builder.Services.AddIdentity<User, IdentityRole<int>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            // .AddRoles<IdentityRole<int>>()

            builder.Services.ConfigureApplicationCookie(options => {
                options.LoginPath = "/Auth/Login";
                options.AccessDeniedPath = "/Auth/Login";
            });

            builder.Services.Configure<IdentityOptions>(options => {
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.";

                options.User.RequireUniqueEmail = true;
            });
            builder.Services.AddCors(options => {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => {
                        builder
                            .SetIsOriginAllowed(origin => true)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
                    });
            });
            var app = builder.Build();

            using (var scope = app.Services.CreateScope()) {
                var services = scope.ServiceProvider;
                try {
                    await DbSeeder.SeedRolesAndAdminAsync(services);
                }
                catch (Exception ex) {
                    Console.WriteLine("Lỗi tạo Admin: " + ex.Message);
                }
            }

            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.Use(async (context, next) =>
            //{
            //    if (context.User.Identity != null && context.User.Identity.IsAuthenticated) {
            //        var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
            //        var user = await userManager.GetUserAsync(context.User);
            //        if (user == null) {
            //            var signInManager = context.RequestServices.GetRequiredService<SignInManager<User>>();
            //            await signInManager.SignOutAsync();
            //            context.Session.Clear();
            //            context.Response.Redirect("/Auth/Login");
            //            return; 
            //        }
            //    }

            //    await next();
            //});

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCors("SignalRCorsPolicy");

            app.UseSession();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapHub<NotificationsHub>("/notificationsHub");
            app.MapHub<Mini_Social_Media.Hubs.ChatHub>("/chatHub");

            app.MapStaticAssets();

            app.UseStatusCodePagesWithReExecute("/Error/{0}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
