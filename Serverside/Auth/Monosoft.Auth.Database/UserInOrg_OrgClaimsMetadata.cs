﻿// <copyright file="UserInOrg_OrgClaimsMetadata.cs" company="Monosoft Holding ApS">
// Copyright (c) Monosoft Holding ApS. All rights reserved.
// </copyright>

namespace Monosoft.Database.Auth.Datalayer
{
    using Microsoft.EntityFrameworkCore;

    public class UserInOrg_OrgClaimsMetadata
    {
        public int Id { get; set; }

        public virtual UserInOrganisation UserInOrganisation { get; set; }

        public string Scope { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public Common.DTO.MetaData Convert2DTO()
        {
            return new Common.DTO.MetaData()
            {
                Key = this.Key,
                Scope = this.Scope,
                Value = this.Value
            };
        }

        internal static void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInOrg_OrgClaimsMetadata>()
                .HasKey(p => p.Id);
            modelBuilder.Entity<UserInOrg_OrgClaimsMetadata>()
                .Property(f => f.Id)
                .ValueGeneratedOnAdd();
        }
    }
}