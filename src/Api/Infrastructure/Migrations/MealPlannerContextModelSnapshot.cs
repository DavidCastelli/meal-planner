﻿// <auto-generated />
using System;
using Api.Domain.Meals;
using Api.Domain.Tags;
using Api.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    [DbContext(typeof(MealPlannerContext))]
    partial class MealPlannerContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False")
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "schedule", new[] { "none", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday" });
            NpgsqlModelBuilderExtensions.HasPostgresEnum(modelBuilder, "tag_type", new[] { "carnivore", "vegetarian", "vegan", "breakfast", "lunch", "dinner", "supper" });
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Api.Domain.Images.Image", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("DisplayFileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int>("ManageableEntityId")
                        .HasColumnType("integer");

                    b.Property<string>("StorageFileName")
                        .IsRequired()
                        .HasMaxLength(12)
                        .HasColumnType("character varying(12)");

                    b.HasKey("Id");

                    b.HasIndex("ManageableEntityId")
                        .IsUnique();

                    b.HasIndex("StorageFileName")
                        .IsUnique();

                    b.ToTable("Image");
                });

            modelBuilder.Entity("Api.Domain.ManageableEntities.ManageableEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationUserId")
                        .HasColumnType("integer");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)")
                        .UseCollation("case_insensitive");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId", "Title")
                        .IsUnique();

                    b.ToTable("ManageableEntity");

                    b.UseTptMappingStrategy();
                });

            modelBuilder.Entity("Api.Domain.MealRecipes.MealRecipe", b =>
                {
                    b.Property<int>("MealId")
                        .HasColumnType("integer");

                    b.Property<int>("RecipeId")
                        .HasColumnType("integer");

                    b.HasKey("MealId", "RecipeId");

                    b.HasIndex("RecipeId");

                    b.ToTable("MealRecipe");
                });

            modelBuilder.Entity("Api.Domain.ShoppingItems.ShoppingItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("ApplicationUserId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsChecked")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsGenerated")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsLocked")
                        .HasColumnType("boolean");

                    b.Property<string>("Measurement")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<decimal?>("Price")
                        .HasPrecision(12, 2)
                        .HasColumnType("numeric(12,2)");

                    b.Property<int?>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("ShoppingItem");
                });

            modelBuilder.Entity("Api.Domain.Tags.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<TagType>("Type")
                        .HasColumnType("tag_type");

                    b.HasKey("Id");

                    b.ToTable("Tag");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Type = TagType.Carnivore
                        },
                        new
                        {
                            Id = 2,
                            Type = TagType.Vegetarian
                        },
                        new
                        {
                            Id = 3,
                            Type = TagType.Vegan
                        },
                        new
                        {
                            Id = 4,
                            Type = TagType.Breakfast
                        },
                        new
                        {
                            Id = 5,
                            Type = TagType.Lunch
                        },
                        new
                        {
                            Id = 6,
                            Type = TagType.Dinner
                        },
                        new
                        {
                            Id = 7,
                            Type = TagType.Supper
                        });
                });

            modelBuilder.Entity("Api.Infrastructure.Identity.ApplicationUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("integer");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("text");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("text");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("text");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("boolean");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("MealTag", b =>
                {
                    b.Property<int>("MealsId")
                        .HasColumnType("integer");

                    b.Property<int>("TagsId")
                        .HasColumnType("integer");

                    b.HasKey("MealsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("MealTag");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ClaimType")
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("text");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<int>("RoleId")
                        .HasColumnType("integer");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .HasColumnType("text");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("Api.Domain.Meals.Meal", b =>
                {
                    b.HasBaseType("Api.Domain.ManageableEntities.ManageableEntity");

                    b.Property<Schedule>("Schedule")
                        .HasColumnType("schedule");

                    b.ToTable("Meal");
                });

            modelBuilder.Entity("Api.Domain.Recipes.Recipe", b =>
                {
                    b.HasBaseType("Api.Domain.ManageableEntities.ManageableEntity");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.ToTable("Recipe");
                });

            modelBuilder.Entity("Api.Domain.Images.Image", b =>
                {
                    b.HasOne("Api.Domain.ManageableEntities.ManageableEntity", "ManageableEntity")
                        .WithOne("Image")
                        .HasForeignKey("Api.Domain.Images.Image", "ManageableEntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ManageableEntity");
                });

            modelBuilder.Entity("Api.Domain.ManageableEntities.ManageableEntity", b =>
                {
                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Api.Domain.MealRecipes.MealRecipe", b =>
                {
                    b.HasOne("Api.Domain.Meals.Meal", null)
                        .WithMany()
                        .HasForeignKey("MealId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Api.Domain.Recipes.Recipe", null)
                        .WithMany()
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Api.Domain.ShoppingItems.ShoppingItem", b =>
                {
                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("ApplicationUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("MealTag", b =>
                {
                    b.HasOne("Api.Domain.Meals.Meal", null)
                        .WithMany()
                        .HasForeignKey("MealsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Api.Domain.Tags.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<int>", b =>
                {
                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<int>", b =>
                {
                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<int>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole<int>", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<int>", b =>
                {
                    b.HasOne("Api.Infrastructure.Identity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Api.Domain.Meals.Meal", b =>
                {
                    b.HasOne("Api.Domain.ManageableEntities.ManageableEntity", null)
                        .WithOne()
                        .HasForeignKey("Api.Domain.Meals.Meal", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Api.Domain.Recipes.Recipe", b =>
                {
                    b.HasOne("Api.Domain.ManageableEntities.ManageableEntity", null)
                        .WithOne()
                        .HasForeignKey("Api.Domain.Recipes.Recipe", "Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsMany("Api.Domain.Recipes.Direction", "Directions", b1 =>
                        {
                            b1.Property<int>("RecipeId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.Property<int>("Number")
                                .HasColumnType("integer");

                            b1.HasKey("RecipeId", "Id");

                            b1.HasIndex("RecipeId", "Number")
                                .IsUnique();

                            b1.ToTable("Direction");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");
                        });

                    b.OwnsOne("Api.Domain.Recipes.RecipeDetails", "RecipeDetails", b1 =>
                        {
                            b1.Property<int>("RecipeId")
                                .HasColumnType("integer");

                            b1.Property<int?>("CookTime")
                                .HasColumnType("integer");

                            b1.Property<int?>("PrepTime")
                                .HasColumnType("integer");

                            b1.Property<int?>("Servings")
                                .HasColumnType("integer");

                            b1.HasKey("RecipeId");

                            b1.ToTable("Recipe");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");
                        });

                    b.OwnsOne("Api.Domain.Recipes.RecipeNutrition", "RecipeNutrition", b1 =>
                        {
                            b1.Property<int>("RecipeId")
                                .HasColumnType("integer");

                            b1.Property<int?>("Calories")
                                .HasColumnType("integer");

                            b1.Property<int?>("Carbs")
                                .HasColumnType("integer");

                            b1.Property<int?>("Fat")
                                .HasColumnType("integer");

                            b1.Property<int?>("Protein")
                                .HasColumnType("integer");

                            b1.HasKey("RecipeId");

                            b1.ToTable("Recipe");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");
                        });

                    b.OwnsMany("Api.Domain.Recipes.SubIngredient", "SubIngredients", b1 =>
                        {
                            b1.Property<int>("RecipeId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Name")
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)")
                                .UseCollation("case_insensitive");

                            b1.HasKey("RecipeId", "Id");

                            b1.HasIndex("RecipeId", "Name")
                                .IsUnique();

                            b1.ToTable("SubIngredient");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");

                            b1.OwnsMany("Api.Domain.Recipes.Ingredient", "Ingredients", b2 =>
                                {
                                    b2.Property<int>("SubIngredientRecipeId")
                                        .HasColumnType("integer");

                                    b2.Property<int>("SubIngredientId")
                                        .HasColumnType("integer");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("Measurement")
                                        .IsRequired()
                                        .HasMaxLength(20)
                                        .HasColumnType("character varying(20)");

                                    b2.Property<string>("Name")
                                        .IsRequired()
                                        .HasMaxLength(20)
                                        .HasColumnType("character varying(20)");

                                    b2.HasKey("SubIngredientRecipeId", "SubIngredientId", "Id");

                                    b2.ToTable("Ingredient");

                                    b2.WithOwner()
                                        .HasForeignKey("SubIngredientRecipeId", "SubIngredientId");
                                });

                            b1.Navigation("Ingredients");
                        });

                    b.OwnsMany("Api.Domain.Recipes.Tip", "Tips", b1 =>
                        {
                            b1.Property<int>("RecipeId")
                                .HasColumnType("integer");

                            b1.Property<int>("Id")
                                .ValueGeneratedOnAdd()
                                .HasColumnType("integer");

                            NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b1.Property<int>("Id"));

                            b1.Property<string>("Description")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("character varying(255)");

                            b1.HasKey("RecipeId", "Id");

                            b1.ToTable("Tip");

                            b1.WithOwner()
                                .HasForeignKey("RecipeId");
                        });

                    b.Navigation("Directions");

                    b.Navigation("RecipeDetails")
                        .IsRequired();

                    b.Navigation("RecipeNutrition")
                        .IsRequired();

                    b.Navigation("SubIngredients");

                    b.Navigation("Tips");
                });

            modelBuilder.Entity("Api.Domain.ManageableEntities.ManageableEntity", b =>
                {
                    b.Navigation("Image");
                });
#pragma warning restore 612, 618
        }
    }
}
