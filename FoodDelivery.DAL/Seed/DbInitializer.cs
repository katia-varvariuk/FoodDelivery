using FoodDelivery.DAL.Data;
using FoodDelivery.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodDelivery.DAL.Seed
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Міграції вже застосовані в Program.cs

            // Створюємо ролі
            await SeedRoles(roleManager);

            // Створюємо користувачів
            await SeedUsers(userManager);

            // Створюємо категорії
            await SeedCategories(context);

            // Створюємо ресторани
            await SeedRestaurants(context);

            // Створюємо страви
            await SeedMenuItems(context);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "Admin", "User", "Restaurant" };

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        private static async Task SeedUsers(UserManager<User> userManager)
        {
            // Створюємо адміністратора
            if (await userManager.FindByEmailAsync("admin@fooddelivery.com") == null)
            {
                var admin = new User
                {
                    UserName = "admin@fooddelivery.com",
                    Email = "admin@fooddelivery.com",
                    FirstName = "Admin",
                    LastName = "User",
                    EmailConfirmed = true,
                    Address = "Admin Street 1",
                    DateOfBirth = new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Створюємо звичайного користувача
            if (await userManager.FindByEmailAsync("user@fooddelivery.com") == null)
            {
                var user = new User
                {
                    UserName = "user@fooddelivery.com",
                    Email = "user@fooddelivery.com",
                    FirstName = "Regular",
                    LastName = "User",
                    EmailConfirmed = true,
                    Address = "User Street 1",
                    DateOfBirth = new DateTime(1995, 5, 5, 0, 0, 0, DateTimeKind.Utc)
                };

                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            // Створюємо користувача-ресторан
            if (await userManager.FindByEmailAsync("restaurant@fooddelivery.com") == null)
            {
                var restaurant = new User
                {
                    UserName = "restaurant@fooddelivery.com",
                    Email = "restaurant@fooddelivery.com",
                    FirstName = "Restaurant",
                    LastName = "Owner",
                    EmailConfirmed = true,
                    Address = "Restaurant Street 1",
                    DateOfBirth = new DateTime(1985, 10, 10, 0, 0, 0, DateTimeKind.Utc)
                };

                var result = await userManager.CreateAsync(restaurant, "Restaurant123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(restaurant, "Restaurant");
                }
            }
        }

        private static async Task SeedCategories(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Піца", Description = "Італійська страва, що представляє собою коржик із дріжджового тіста з різними начинками" },
                    new Category { Name = "Бургери", Description = "Популярна фаст-фуд страва, що складається з м'ясної котлети між двома половинками булочки" },
                    new Category { Name = "Суші", Description = "Традиційна японська страва, що зазвичай складається з рису та інших інгредієнтів" },
                    new Category { Name = "Салати", Description = "Страви, що складаються з мікс різних овочів та інших інгредієнтів" },
                    new Category { Name = "Десерти", Description = "Солодкі страви, що подаються в кінці прийому їжі" },
                    new Category { Name = "Напої", Description = "Різноманітні безалкогольні та алкогольні напої" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedRestaurants(AppDbContext context)
        {
            if (!context.Restaurants.Any())
            {
                var restaurants = new List<Restaurant>
                {
                    new Restaurant
                    {
                        Name = "Піца Плюс",
                        Description = "Найкраща піцерія міста",
                        Address = "вул. Центральна, 1, Львів",
                        Phone = "+380123456789",
                        Email = "pizza@plus.com",
                        LogoUrl = "https://example.com/logo-pizza-plus.jpg",
                        Rating = 4.7,
                        IsActive = true
                    },
                    new Restaurant
                    {
                        Name = "Бургер Кінг",
                        Description = "Соковиті бургери та картопля фрі",
                        Address = "вул. Шевченка, 10, Львів",
                        Phone = "+380987654321",
                        Email = "burger@king.com",
                        LogoUrl = "https://example.com/logo-burger-king.jpg",
                        Rating = 4.5,
                        IsActive = true
                    },
                    new Restaurant
                    {
                        Name = "Суші Вок",
                        Description = "Японська та китайська кухня",
                        Address = "вул. Франка, 15, Львів",
                        Phone = "+380567891234",
                        Email = "sushi@wok.com",
                        LogoUrl = "https://example.com/logo-sushi-wok.jpg",
                        Rating = 4.8,
                        IsActive = true
                    }
                };

                await context.Restaurants.AddRangeAsync(restaurants);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedMenuItems(AppDbContext context)
        {
            if (!context.MenuItems.Any())
            {
                // Отримуємо категорії та ресторани
                var pizzaCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Піца");
                var burgerCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Бургери");
                var sushiCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Суші");
                var dessertCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Десерти");
                var drinksCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Напої");

                var pizzaRestaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Name == "Піца Плюс");
                var burgerRestaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Name == "Бургер Кінг");
                var sushiRestaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Name == "Суші Вок");

                var menuItems = new List<MenuItem>();

                // Піца
                if (pizzaCategory != null && pizzaRestaurant != null)
                {
                    menuItems.AddRange(new List<MenuItem>
                    {
                        new MenuItem
                        {
                            Name = "Маргарита",
                            Description = "Томатний соус, моцарела, базилік",
                            Price = 150.00M,
                            ImageUrl = "https://example.com/margarita.jpg",
                            IsAvailable = true,
                            CategoryId = pizzaCategory.Id,
                            RestaurantId = pizzaRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Пепероні",
                            Description = "Томатний соус, моцарела, пепероні",
                            Price = 180.00M,
                            ImageUrl = "https://example.com/pepperoni.jpg",
                            IsAvailable = true,
                            CategoryId = pizzaCategory.Id,
                            RestaurantId = pizzaRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Гавайська",
                            Description = "Томатний соус, моцарела, шинка, ананас",
                            Price = 190.00M,
                            ImageUrl = "https://example.com/hawaiian.jpg",
                            IsAvailable = true,
                            CategoryId = pizzaCategory.Id,
                            RestaurantId = pizzaRestaurant.Id
                        }
                    });
                }

                // Бургери
                if (burgerCategory != null && burgerRestaurant != null)
                {
                    menuItems.AddRange(new List<MenuItem>
                    {
                        new MenuItem
                        {
                            Name = "Класичний бургер",
                            Description = "Яловича котлета, сир чеддер, салат, помідор, цибуля, соус",
                            Price = 120.00M,
                            ImageUrl = "https://example.com/classic-burger.jpg",
                            IsAvailable = true,
                            CategoryId = burgerCategory.Id,
                            RestaurantId = burgerRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Чізбургер",
                            Description = "Яловича котлета, подвійний сир чеддер, салат, помідор, цибуля, соус",
                            Price = 140.00M,
                            ImageUrl = "https://example.com/cheeseburger.jpg",
                            IsAvailable = true,
                            CategoryId = burgerCategory.Id,
                            RestaurantId = burgerRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Бекон бургер",
                            Description = "Яловича котлета, сир чеддер, бекон, салат, помідор, цибуля, соус",
                            Price = 160.00M,
                            ImageUrl = "https://example.com/bacon-burger.jpg",
                            IsAvailable = true,
                            CategoryId = burgerCategory.Id,
                            RestaurantId = burgerRestaurant.Id
                        }
                    });
                }

                // Суші
                if (sushiCategory != null && sushiRestaurant != null)
                {
                    menuItems.AddRange(new List<MenuItem>
                    {
                        new MenuItem
                        {
                            Name = "Каліфорнія",
                            Description = "Роли з крабовим м'ясом, авокадо, огірком та тобіко",
                            Price = 200.00M,
                            ImageUrl = "https://example.com/california.jpg",
                            IsAvailable = true,
                            CategoryId = sushiCategory.Id,
                            RestaurantId = sushiRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Філадельфія",
                            Description = "Роли з лососем, вершковим сиром та авокадо",
                            Price = 220.00M,
                            ImageUrl = "https://example.com/philadelphia.jpg",
                            IsAvailable = true,
                            CategoryId = sushiCategory.Id,
                            RestaurantId = sushiRestaurant.Id
                        },
                        new MenuItem
                        {
                            Name = "Унагі макі",
                            Description = "Роли з копченим вугром, огірком та унагі соусом",
                            Price = 240.00M,
                            ImageUrl = "https://example.com/unagi.jpg",
                            IsAvailable = true,
                            CategoryId = sushiCategory.Id,
                            RestaurantId = sushiRestaurant.Id
                        }
                    });
                }

                // Додаємо напої до всіх ресторанів
                if (drinksCategory != null)
                {
                    var restaurants = await context.Restaurants.ToListAsync();
                    foreach (var restaurant in restaurants)
                    {
                        menuItems.AddRange(new List<MenuItem>
                        {
                            new MenuItem
                            {
                                Name = "Кола",
                                Description = "Газований напій, 0.5л",
                                Price = 30.00M,
                                ImageUrl = "https://example.com/cola.jpg",
                                IsAvailable = true,
                                CategoryId = drinksCategory.Id,
                                RestaurantId = restaurant.Id
                            },
                            new MenuItem
                            {
                                Name = "Мінеральна вода",
                                Description = "Негазована/газована, 0.5л",
                                Price = 25.00M,
                                ImageUrl = "https://example.com/water.jpg",
                                IsAvailable = true,
                                CategoryId = drinksCategory.Id,
                                RestaurantId = restaurant.Id
                            }
                        });
                    }
                }

                await context.MenuItems.AddRangeAsync(menuItems);
                await context.SaveChangesAsync();
            }
        }
    }
}