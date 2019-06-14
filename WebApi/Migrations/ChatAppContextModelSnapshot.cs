﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApi.Models;

namespace WebApi.Migrations
{
    [DbContext(typeof(ChatAppContext))]
    partial class ChatAppContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WebApi.Models.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Author");

                    b.Property<string>("Message");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Chat");
                });

            modelBuilder.Entity("WebApi.Models.Chats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Creator");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("allChat");
                });

            modelBuilder.Entity("WebApi.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.Property<string>("Password");

                    b.Property<string>("Role");

                    b.Property<string>("Status");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "admin",
                            Password = "admin",
                            Role = "admin",
                            Status = "Online"
                        });
                });

            modelBuilder.Entity("WebApi.Models.UserChat", b =>
                {
                    b.Property<int>("UserId");

                    b.Property<int>("ChatsId");

                    b.HasKey("UserId", "ChatsId");

                    b.HasIndex("ChatsId");

                    b.ToTable("UserChat");
                });

            modelBuilder.Entity("WebApi.Models.UsersInChats", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ChatName");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("UsersInChats");
                });

            modelBuilder.Entity("WebApi.Models.UserChat", b =>
                {
                    b.HasOne("WebApi.Models.Chats", "Chats")
                        .WithMany("UserChat")
                        .HasForeignKey("ChatsId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WebApi.Models.User", "User")
                        .WithMany("UserChat")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
