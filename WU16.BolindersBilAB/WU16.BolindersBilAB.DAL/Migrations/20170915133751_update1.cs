using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WU16.BolindersBilAB.DAL.Migrations
{
    public partial class update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarImages_Cars_CarLicenseNumber",
                table: "CarImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarBrands_CarBrandBrandName",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarBrandBrandName",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_CarImages_CarLicenseNumber",
                table: "CarImages");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CarBrandBrandName",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarLicenseNumber",
                table: "CarImages");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Locations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarBrandId",
                table: "Cars",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarId",
                table: "CarImages",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarBrandId",
                table: "Cars",
                column: "CarBrandId");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_CarId",
                table: "CarImages",
                column: "CarId");

            migrationBuilder.AddForeignKey(
                name: "FK_CarImages_Cars_CarId",
                table: "CarImages",
                column: "CarId",
                principalTable: "Cars",
                principalColumn: "LicenseNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarBrands_CarBrandId",
                table: "Cars",
                column: "CarBrandId",
                principalTable: "CarBrands",
                principalColumn: "BrandName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CarImages_Cars_CarId",
                table: "CarImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Cars_CarBrands_CarBrandId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_CarBrandId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_CarImages_CarId",
                table: "CarImages");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Locations");

            migrationBuilder.DropColumn(
                name: "CarBrandId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "CarId",
                table: "CarImages");

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Locations",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarBrandBrandName",
                table: "Cars",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarLicenseNumber",
                table: "CarImages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_CarBrandBrandName",
                table: "Cars",
                column: "CarBrandBrandName");

            migrationBuilder.CreateIndex(
                name: "IX_CarImages_CarLicenseNumber",
                table: "CarImages",
                column: "CarLicenseNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_CarImages_Cars_CarLicenseNumber",
                table: "CarImages",
                column: "CarLicenseNumber",
                principalTable: "Cars",
                principalColumn: "LicenseNumber",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Cars_CarBrands_CarBrandBrandName",
                table: "Cars",
                column: "CarBrandBrandName",
                principalTable: "CarBrands",
                principalColumn: "BrandName",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
