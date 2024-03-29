﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace PodoDemo.Models
{
    public partial class PodoDemoNContext : DbContext
    {
        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<Contact> Contact { get; set; }
        public virtual DbSet<Menu> Menu { get; set; }
        public virtual DbSet<OptionMaster> OptionMaster { get; set; }
        public virtual DbSet<OptionMasterDetail> OptionMasterDetail { get; set; }
        public virtual DbSet<SubMenu> SubMenu { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAuth> UserAuth { get; set; }
        public virtual DbSet<Todo> Todo { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Price> Price { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Appointment> Appointment { get; set; }
        public virtual DbSet<Holiday> Holiday { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
        //    optionsBuilder.UseSqlServer(@"DB CONN STRING");
        //}

        public PodoDemoNContext(DbContextOptions<PodoDemoNContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                //entity.HasIndex(e => e.Biznum)
                //    .HasName("UK_Account")
                //    .IsUnique();

                entity.Property(e => e.Accountid).HasColumnName("accountid");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasMaxLength(500);

                entity.Property(e => e.Addresscity)
                    .HasColumnName("addresscity")
                    .HasMaxLength(500);

                entity.Property(e => e.Addressdetail)
                    .HasColumnName("addressdetail")
                    .HasMaxLength(500);

                entity.Property(e => e.Addresstype)
                    .HasColumnName("addresstype")
                    .HasMaxLength(50);

                entity.Property(e => e.Biznum)
                    .IsRequired()
                    .HasColumnName("biznum")
                    .HasMaxLength(50);

                entity.Property(e => e.Ceo)
                    .HasColumnName("ceo")
                    .HasMaxLength(50);

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Fax)
                    .HasColumnName("fax")
                    .HasMaxLength(20);

                entity.Property(e => e.Founddate)
                    .HasColumnName("founddate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Homepage)
                    .HasColumnName("homepage")
                    .HasMaxLength(100);

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasColumnName("ownerid")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Account)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_Account_User");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(20);

                entity.Property(e => e.Postcode)
                    .HasColumnName("postcode")
                    .HasMaxLength(8);
            });

            modelBuilder.Entity<Contact>(entity =>
            {
                entity.Property(e => e.Contactid).HasColumnName("contactid");

                entity.Property(e => e.Accountid).HasColumnName("accountid");

                entity.Property(e => e.Bossid).HasColumnName("bossid");

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(50);

                entity.Property(e => e.Detail).HasColumnName("detail");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(50);

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasColumnName("ownerid")
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Contact)
                    .HasForeignKey(d => d.Accountid)
                    .HasConstraintName("FK_Contact_Account");
            });

            modelBuilder.Entity<Menu>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Isused).HasColumnName("isused");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Order)
                    .HasColumnName("order")
                    .IsRequired();
            });

            modelBuilder.Entity<OptionMaster>(entity =>
            {
                entity.HasKey(e => e.Masterid)
                    .HasName("PK_OptionMaster");

                entity.Property(e => e.Masterid)
                    .HasColumnName("masterid");
                    //.ValueGeneratedNever();

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Defaultvalue)
                    .HasColumnName("defaultvalue")
                    .HasMaxLength(10);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.Issystem).HasColumnName("issystem");

                entity.Property(e => e.Isused).HasColumnName("isused");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasColumnName("ownerid")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<OptionMasterDetail>(entity =>
            {
                entity.HasKey(e => e.Optionid)
                    .HasName("PK_OptionMasterDetail");

                entity.Property(e => e.Optionid)
                    .HasColumnName("optionid")
                    .HasMaxLength(10);

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.Isused).HasColumnName("isused");

                entity.Property(e => e.Masterid).HasColumnName("masterid");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Order).HasColumnName("order");

                entity.HasOne(d => d.Master)
                    .WithMany(p => p.OptionMasterDetail)
                    .HasForeignKey(d => d.Masterid)
                    .HasConstraintName("FK_OptionMasterDetail_OptionMaster");
            });

            modelBuilder.Entity<SubMenu>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(10);

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Isdeleted).HasColumnName("isdeleted");

                entity.Property(e => e.Ismanager).HasColumnName("ismanager");

                entity.Property(e => e.Isused).HasColumnName("isused");

                entity.Property(e => e.Mainmenuid).HasColumnName("mainmenuid");

                entity.Property(e => e.Menuurl)
                    .IsRequired()
                    .HasColumnName("menuurl");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100);

                entity.Property(e => e.Order).HasColumnName("order");

                entity.HasOne(d => d.Mainmenu)
                    .WithMany(p => p.SubMenu)
                    .HasForeignKey(d => d.Mainmenuid)
                    .HasConstraintName("FK_SubMenu_Menu1");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasMaxLength(50);

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Department)
                    .HasColumnName("department")
                    .HasMaxLength(10);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100);

                entity.Property(e => e.Engname)
                    .HasColumnName("engname")
                    .HasMaxLength(50);

                entity.Property(e => e.Excelauth).HasColumnName("excelauth");

                entity.Property(e => e.Ismaster).HasColumnName("ismaster");

                entity.Property(e => e.Keybox)
                    .HasColumnName("keybox")
                    .HasMaxLength(50);

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasColumnName("level")
                    .HasMaxLength(10);

                entity.Property(e => e.Mobile)
                    .HasColumnName("mobile")
                    .HasMaxLength(20);

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasMaxLength(20);

                entity.Property(e => e.Position)
                    .HasColumnName("position")
                    .HasMaxLength(10);

                entity.Property(e => e.Pw)
                    .IsRequired()
                    .HasColumnName("pw")
                    .HasMaxLength(50);

                entity.Property(e => e.Token)
                    .HasColumnName("token")
                    .HasMaxLength(500);

                entity.HasOne(d => d.DepartmentNavigation)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.Department)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_User_OptionMasterDetail");

                entity.HasOne(d => d.Organization)
                    .WithMany(p => p.User)
                    .HasForeignKey(d => d.Organizationid)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_User_Organization");
            });

            modelBuilder.Entity<UserAuth>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .HasName("PK_UserAuth");

                entity.Property(e => e.Read)
                    .IsRequired()
                    .HasColumnName("read");

                entity.Property(e => e.Modify)
                    .IsRequired()
                    .HasColumnName("modify");

                entity.Property(e => e.Write)
                    .IsRequired()
                    .HasColumnName("write");

                entity.Property(e => e.Delete)
                    .IsRequired()
                    .HasColumnName("delete");

                entity.Property(e => e.Modifydate)
                    .HasColumnName("modifydate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasColumnName("modifyuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .HasColumnType("datetime");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasColumnName("createuser")
                    .HasMaxLength(50);

                entity.Property(e => e.Submenuid).HasColumnName("submenuid");
                entity.Property(e => e.Userid).HasColumnName("userid");


                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserAuth)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("FK_UserAuth_User");
            });

            modelBuilder.Entity<Todo>(entity =>
            {
                entity.Property(e => e.Todoid).HasColumnName("todoid");
                //entity.HasKey(e => e.Todoid)
                //.HasName("PK_Todo");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.Regardingobjectid)
                    .HasColumnName("regardingobjectid");

                entity.Property(e => e.Regardingobjecttypeid)
                    .HasColumnName("regardingobjecttypeid");

                entity.Property(e => e.Regardingobjectname)
                    .HasColumnName("regardingobjectname");

                entity.Property(e => e.Startdate)
                    .IsRequired()
                    .HasColumnName("startdate");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate");

                entity.Property(e => e.Createdate)
                    .IsRequired()
                    .HasColumnName("createdate");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("createuser");

                entity.Property(e => e.Modifydate)
                    .IsRequired()
                    .HasColumnName("modifydate");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modifyuser");

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ownerid");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Todo)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_Todo_User");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                //entity.Property(e => e.Productid)
                //    .IsRequired()                   
                //    .HasColumnName("productid");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Maker)
                    .HasMaxLength(10)
                    .HasColumnName("maker");

                entity.Property(e => e.Origin)
                    .HasMaxLength(10)
                    .HasColumnName("origin");

                entity.Property(e => e.Productcode)
                    .HasMaxLength(50)
                    .HasColumnName("productcode");

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasColumnName("ownerid")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Product)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_Product_User");

                entity.Property(e => e.Createdate)
                    .IsRequired()
                    .HasColumnName("createdate");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("createuser");

                entity.Property(e => e.Modifydate)
                    .IsRequired()
                    .HasColumnName("modifydate");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modifyuser");

            });

            modelBuilder.Entity<Price>(entity =>
            {
                //entity.Property(e => e.Productid)
                //    .IsRequired()                   
                //    .HasColumnName("productid");

                entity.Property(e => e.Prices)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("prices");

                entity.Property(e => e.Cost)
                    .HasMaxLength(10)
                    .HasColumnName("cost");

                entity.Property(e => e.Currency)
                    .HasMaxLength(10)
                    .HasColumnName("currency");

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasColumnName("ownerid")
                    .HasMaxLength(50);

                entity.Property(e => e.Createdate)
                    .IsRequired()
                    .HasColumnName("createdate");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("createuser");

                entity.Property(e => e.Modifydate)
                    .IsRequired()
                    .HasColumnName("modifydate");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modifyuser");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Price)
                    .HasForeignKey(d => d.Productid)
                    .HasConstraintName("FK_Price_Product");
            });

            modelBuilder.Entity<Organization>(entity =>
            {
                entity.HasKey(e => e.Organizationid)
                    .HasName("PK_Organization");

                entity.Property(e => e.Organizationid)
                    .HasColumnName("organizationid");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(e => e.Createdate)
                    .HasColumnName("createdate")
                    .IsRequired();

                entity.Property(e => e.Memo)
                    .HasColumnName("memo");                
            });

            modelBuilder.Entity<Appointment>(entity =>
            {
                entity.Property(e => e.Appointmentid).HasColumnName("appointmentid");
                //entity.HasKey(e => e.Todoid)
                //.HasName("PK_Todo");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.Regardingobjectid)
                    .HasColumnName("regardingobjectid");

                entity.Property(e => e.Regardingobjecttypeid)
                    .HasColumnName("regardingobjecttypeid");

                entity.Property(e => e.Regardingobjectname)
                    .HasColumnName("regardingobjectname");

                entity.Property(e => e.Startdate)
                    .IsRequired()
                    .HasColumnName("startdate");

                entity.Property(e => e.Enddate)
                    .HasColumnName("enddate");

                entity.Property(e => e.Createdate)
                    .IsRequired()
                    .HasColumnName("createdate");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("createuser");

                entity.Property(e => e.Modifydate)
                    .IsRequired()
                    .HasColumnName("modifydate");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modifyuser");

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ownerid");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Appointment)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_Appointment_User");

                entity.Property(e => e.State)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("state");
            });

            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.Property(e => e.Holidayid).HasColumnName("holidayid");
                //entity.HasKey(e => e.Todoid)
                //.HasName("PK_Todo");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200)
                    .HasColumnName("name");

                entity.Property(e => e.Description)
                    .HasColumnName("description");

                entity.Property(e => e.Startdate)
                    .IsRequired()
                    .HasColumnName("startdate");

                entity.Property(e => e.Enddate)
                    .IsRequired()
                    .HasColumnName("enddate");

                entity.Property(e => e.Createdate)
                    .IsRequired()
                    .HasColumnName("createdate");

                entity.Property(e => e.Createuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("createuser");

                entity.Property(e => e.Modifydate)
                    .IsRequired()
                    .HasColumnName("modifydate");

                entity.Property(e => e.Modifyuser)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("modifyuser");

                entity.Property(e => e.Ownerid)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("ownerid");

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Holiday)
                    .HasForeignKey(d => d.Ownerid)
                    .HasConstraintName("FK_Holiday_User");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("type");
            });
        }
    }
}