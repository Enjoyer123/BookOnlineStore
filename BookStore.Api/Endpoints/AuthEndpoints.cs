using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Bookstore.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth");

        group.MapPost("/register", async (AppDbContext db, RegisterDto dto) =>
        {
            var exists = await db.Users.AnyAsync(u => u.Email == dto.Email);
            if (exists) return Results.BadRequest("Email already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = hashedPassword
            };

            db.Users.Add(user);
            await db.SaveChangesAsync();

            return Results.Created($"/api/users/{user.Id}", new AuthResponseDto(user.Email));
        });

        group.MapPost("/login", async (AppDbContext db, HttpContext http, IConfiguration config, LoginDto dto) =>
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(user, config);

            http.Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, // Set to true in production
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return Results.Ok(new AuthResponseDto(user.Email));
        });

        group.MapPost("/logout", (HttpContext http) =>
       {
           http.Response.Cookies.Delete("jwt", new CookieOptions
           {
               HttpOnly = true,
               Secure = false,
               SameSite = SameSiteMode.Strict
           });
           return Results.Ok();
       });
    }

    private static string GenerateJwtToken(User user, IConfiguration config)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}