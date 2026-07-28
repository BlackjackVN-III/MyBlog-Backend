using Blog.Application.DTOs.User;
using Blog.Domain.Entities;
using Blog.Infrastructure.Data;
using Blog.Infrastructure.Identity;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Blog.API.Tests.Controllers
{
    public class ProfileControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        public ProfileControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }
        [Fact]
        public async Task GetProfile_WhenAnonymous_ShouldReturn401Unauthorized()
        {
            // Act: Gọi API xem profile nhưng không đính kèm JWT Token
            var response = await _client.GetAsync("/api/profile");
            // Assert: Hệ thống phải tự động trả về lỗi 401 Unauthorized
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
        [Fact]
        public async Task GetProfile_WhenAuthenticated_ShouldReturnUserProfile()
        {
            // 1. Arrange: Chuẩn bị dữ liệu tài khoản trong In-Memory Database
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                // Đảm bảo DB sạch trước khi chạy test
                await dbContext.Database.EnsureDeletedAsync();
                await dbContext.Database.EnsureCreatedAsync();
                // Tạo AppUser (tài khoản đăng nhập)
                var appUser = new AppUser
                {
                    Id = Guid.NewGuid(),
                    UserName = "bjvn_tester",
                    Email = "tester@test.com"
                };
                await userManager.CreateAsync(appUser, "Password123!!@");
                // Tạo Domain User (chứa Bio/Avatar tương ứng)
                var domainUser = new User
                {
                    Id = appUser.Id,
                    Username = appUser.UserName,
                    Email = appUser.Email,
                    Bio = "Tôi là Integration Tester",
                    AvatarUrl = "http://cloudinary.com/avatar.jpg"
                };
                await dbContext.DomainUsers.AddAsync(domainUser);
                await dbContext.SaveChangesAsync();
                // Lấy Access Token thực cho tài khoản test này
                var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
                {
                    UserName = "bjvn_tester",
                    Password = "Password123!!@"
                });
                loginResponse.EnsureSuccessStatusCode();

                var loginResult = await loginResponse.Content.ReadFromJsonAsync<JsonElement>();
                var token = loginResult.GetProperty("token").GetString();
                // Gán JWT Token vào Header của client cho các request tiếp theo
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            // 2. Act: Gọi API xem profile cá nhân
            var response = await _client.GetAsync("/api/profile");
            // 3. Assert: Kiểm tra dữ liệu HTTP phản hồi
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var profile = await response.Content.ReadFromJsonAsync<UserProfileDto>();
            profile.Should().NotBeNull();
            profile!.Username.Should().Be("bjvn_tester");
            profile.Email.Should().Be("tester@test.com");
            profile.Bio.Should().Be("Tôi là Integration Tester");
            profile.AvatarUrl.Should().Be("http://cloudinary.com/avatar.jpg");
        }
    }
}