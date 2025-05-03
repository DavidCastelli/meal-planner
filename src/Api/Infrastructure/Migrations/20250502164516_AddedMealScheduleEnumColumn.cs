using Api.Domain.Meals;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedMealScheduleEnumColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False")
                .Annotation("Npgsql:Enum:schedule", "none,monday,tuesday,wednesday,thursday,friday,saturday,sunday")
                .Annotation("Npgsql:Enum:tag_type", "carnivore,vegetarian,vegan,breakfast,lunch,dinner,supper")
                .OldAnnotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False")
                .OldAnnotation("Npgsql:Enum:tag_type", "carnivore,vegetarian,vegan,breakfast,lunch,dinner,supper");

            migrationBuilder.AddColumn<Schedule>(
                name: "Schedule",
                table: "Meal",
                type: "schedule",
                nullable: false,
                defaultValue: Schedule.None);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "Meal");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False")
                .Annotation("Npgsql:Enum:tag_type", "carnivore,vegetarian,vegan,breakfast,lunch,dinner,supper")
                .OldAnnotation("Npgsql:CollationDefinition:case_insensitive", "en-u-ks-primary,en-u-ks-primary,icu,False")
                .OldAnnotation("Npgsql:Enum:schedule", "none,monday,tuesday,wednesday,thursday,friday,saturday,sunday")
                .OldAnnotation("Npgsql:Enum:tag_type", "carnivore,vegetarian,vegan,breakfast,lunch,dinner,supper");
        }
    }
}
