namespace FriterieShop.Infrastructure.Data
{
    using FriterieShop.Domain.Entities;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class SampleDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

            if (await db.Products.AnyAsync())
            {
                logger.LogInformation("Products already exist, skipping seed.");
                return;
            }

            logger.LogInformation("Seeding sample data...");

            // Supprimer les anciennes catégories si elles existent mais sans produits
            if (await db.Categories.AnyAsync())
            {
                logger.LogInformation("Removing orphan categories before reseeding...");
                db.Categories.RemoveRange(await db.Categories.ToListAsync());
                await db.SaveChangesAsync();
            }

            // ── Categories ──
            var burgers = new Category { Id = Guid.NewGuid(), Name = "Burgers" };
            var sauces = new Category { Id = Guid.NewGuid(), Name = "Sauces" };
            var frites = new Category { Id = Guid.NewGuid(), Name = "Frites" };
            var boissons = new Category { Id = Guid.NewGuid(), Name = "Boissons" };
            var menus = new Category { Id = Guid.NewGuid(), Name = "Menus" };
            var snacks = new Category { Id = Guid.NewGuid(), Name = "Snacks" };

            db.Categories.AddRange(burgers, sauces, frites, boissons, menus, snacks);

            // ── Burgers ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Barbare",
                    Description = "Notre burger signature ! Double steak haché 150g, cheddar fondu, bacon croustillant, oignons caramélisés, sauce barbare maison. Un incontournable.",
                    Price = 9.50m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1568901346375-23c9450c58cd?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Viking",
                    Description = "Steak haché 150g, fromage bleu, roquette, oignons rouges croustillants et sauce miel-moutarde. Pour les aventuriers du goût.",
                    Price = 10.00m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1553979459-d2229ba7433b?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Classique",
                    Description = "Steak haché 125g, salade, tomate, oignon, cornichons et sauce ketchup-moutarde. Simple mais efficace.",
                    Price = 7.50m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1550547660-d9450f859349?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Chicken Burger",
                    Description = "Filet de poulet pané croustillant, salade iceberg, tomate, sauce curry légère. Le choix volaille.",
                    Price = 8.50m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1606755962773-d324e0a13086?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Double Cheese",
                    Description = "Double steak haché, double cheddar, cornichons, oignons, moutarde et ketchup. Pour les grosses faims.",
                    Price = 11.00m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1572802419224-296b0aeee0d9?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Le Veggie",
                    Description = "Galette de légumes et pois chiches, avocat, tomate, roquette et sauce yaourt-citron. 100% végétarien.",
                    Price = 8.00m,
                    Quantity = 100,
                    CategoryId = burgers.Id,
                    Image = "https://images.unsplash.com/photo-1520072959219-c595dc870360?w=600&h=400&fit=crop"
                }
            );

            // ── Sauces ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sauce Andalouse",
                    Description = "La sauce belge par excellence. Mayonnaise relevée aux poivrons et tomates. Incontournable avec les frites.",
                    Price = 1.50m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1472476443507-c7a5948772fc?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sauce Samouraï",
                    Description = "Mayonnaise pimentée au harissa et paprika. Pour ceux qui aiment le piquant !",
                    Price = 1.50m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1563805042-7684c019e1cb?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sauce Brésilienne",
                    Description = "Sauce cocktail douce aux épices exotiques et pointe de curry. Un voyage en bouche.",
                    Price = 1.50m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1585325701956-60dd9c8553bc?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Mayonnaise Maison",
                    Description = "Notre mayonnaise préparée chaque jour avec des œufs frais. La base de toute bonne friterie.",
                    Price = 1.00m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1613478223719-2ab802602423?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Ketchup",
                    Description = "Ketchup classique aux tomates mûries au soleil. Le grand classique.",
                    Price = 1.00m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1598511726623-d2e9996892f0?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Sauce Américaine",
                    Description = "Mayonnaise douce aux cornichons finement hachés. La préférée des enfants.",
                    Price = 1.50m,
                    Quantity = 200,
                    CategoryId = sauces.Id,
                    Image = "https://images.unsplash.com/photo-1509042239860-f550ce710b93?w=600&h=400&fit=crop"
                }
            );

            // ── Frites ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Petite Frite",
                    Description = "Portion de frites belges croustillantes cuites deux fois dans la graisse de bœuf. Format individuel.",
                    Price = 3.00m,
                    Quantity = 200,
                    CategoryId = frites.Id,
                    Image = "https://images.unsplash.com/photo-1573080496219-bb080dd4f877?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Moyenne Frite",
                    Description = "Portion généreuse de frites belges double cuisson. Idéal pour accompagner un burger.",
                    Price = 4.00m,
                    Quantity = 200,
                    CategoryId = frites.Id,
                    Image = "https://images.unsplash.com/photo-1518013431117-eb1465fa5752?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Grande Frite",
                    Description = "Notre plus grande portion. Parfait à partager ou pour les très grosses faims.",
                    Price = 5.00m,
                    Quantity = 200,
                    CategoryId = frites.Id,
                    Image = "https://images.unsplash.com/photo-1541592106381-b31e9677c0e4?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Frites de Patate Douce",
                    Description = "Frites de patate douce croustillantes à l'extérieur, fondantes à l'intérieur. Une alternative gourmande.",
                    Price = 5.50m,
                    Quantity = 150,
                    CategoryId = frites.Id,
                    Image = "https://images.unsplash.com/photo-1604329760661-e71dc83f8f26?w=600&h=400&fit=crop"
                }
            );

            // ── Boissons ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Coca-Cola 33cl",
                    Description = "Canette de Coca-Cola classique bien fraîche.",
                    Price = 2.50m,
                    Quantity = 300,
                    CategoryId = boissons.Id,
                    Image = "https://images.unsplash.com/photo-1554866585-cd94860890b7?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Fanta Orange 33cl",
                    Description = "Canette de Fanta Orange pétillante et rafraîchissante.",
                    Price = 2.50m,
                    Quantity = 300,
                    CategoryId = boissons.Id,
                    Image = "https://images.unsplash.com/photo-1624517452488-04869289c4ca?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Eau Plate 50cl",
                    Description = "Bouteille d'eau minérale plate.",
                    Price = 1.50m,
                    Quantity = 300,
                    CategoryId = boissons.Id,
                    Image = "https://images.unsplash.com/photo-1548839140-29a749e1cf4d?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Ice Tea Pêche 33cl",
                    Description = "Canette de thé glacé saveur pêche. Doux et désaltérant.",
                    Price = 2.50m,
                    Quantity = 300,
                    CategoryId = boissons.Id,
                    Image = "https://images.unsplash.com/photo-1556679343-c7306c1976bc?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Limonade Artisanale",
                    Description = "Limonade maison au citron frais et menthe. Préparée chaque jour.",
                    Price = 3.00m,
                    Quantity = 150,
                    CategoryId = boissons.Id,
                    Image = "https://images.unsplash.com/photo-1621263764928-df1444c5e859?w=600&h=400&fit=crop"
                }
            );

            // ── Menus ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Menu Barbare",
                    Description = "Burger Le Barbare + Grande Frite + Boisson 33cl + Sauce au choix. Notre menu star !",
                    Price = 14.50m,
                    Quantity = 100,
                    CategoryId = menus.Id,
                    Image = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Menu Viking",
                    Description = "Burger Le Viking + Grande Frite + Boisson 33cl + Sauce au choix. Pour les conquérants.",
                    Price = 15.00m,
                    Quantity = 100,
                    CategoryId = menus.Id,
                    Image = "https://images.unsplash.com/photo-1561758033-d89a9ad46330?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Menu Enfant",
                    Description = "Mini burger + Petite Frite + Jus de fruit + Surprise. Pour les petits barbares en herbe.",
                    Price = 8.50m,
                    Quantity = 100,
                    CategoryId = menus.Id,
                    Image = "https://images.unsplash.com/photo-1565299507177-b0ac66763828?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Menu Mega",
                    Description = "Burger Le Double Cheese + Grande Frite + Grande Boisson + 2 Sauces. Le menu XXL pour les plus affamés.",
                    Price = 18.00m,
                    Quantity = 100,
                    CategoryId = menus.Id,
                    Image = "https://images.unsplash.com/photo-1610970881699-44a5587cabec?w=600&h=400&fit=crop"
                }
            );

            // ── Snacks ──
            db.Products.AddRange(
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Fricadelle",
                    Description = "La fricadelle de friterie, panée et dorée. Un classique du Nord ! Servie avec sauce au choix.",
                    Price = 3.50m,
                    Quantity = 150,
                    CategoryId = snacks.Id,
                    Image = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Boulette de Viande",
                    Description = "Boulette de viande hachée maison, sauce tomate ou liégeoise. Recette traditionnelle.",
                    Price = 4.00m,
                    Quantity = 150,
                    CategoryId = snacks.Id,
                    Image = "https://images.unsplash.com/photo-1529042410759-befb1204b468?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Mitraillette",
                    Description = "Demi-baguette garnie de viande (burger ou fricadelle), frites, crudités et sauce. Le sandwich belge ultime.",
                    Price = 9.00m,
                    Quantity = 100,
                    CategoryId = snacks.Id,
                    Image = "https://images.unsplash.com/photo-1539518429546-845e753e8e40?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Nuggets (6 pcs)",
                    Description = "6 nuggets de poulet croustillants, servis avec sauce au choix.",
                    Price = 5.00m,
                    Quantity = 150,
                    CategoryId = snacks.Id,
                    Image = "https://images.unsplash.com/photo-1562967914-608f82629710?w=600&h=400&fit=crop"
                },
                new Product
                {
                    Id = Guid.NewGuid(),
                    Name = "Onion Rings (8 pcs)",
                    Description = "Rondelles d'oignon panées et frites. Croustillantes à souhait.",
                    Price = 4.50m,
                    Quantity = 150,
                    CategoryId = snacks.Id,
                    Image = "https://images.unsplash.com/photo-1639024471283-03518883512d?w=600&h=400&fit=crop"
                }
            );

            await db.SaveChangesAsync();
            logger.LogInformation("Sample data seeded successfully: 6 categories, 31 products.");
        }
    }
}
