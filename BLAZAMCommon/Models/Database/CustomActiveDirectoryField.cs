﻿using BLAZAM.Common.Data.ActiveDirectory;
using BLAZAM.Common.Models.Database.Permissions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace BLAZAM.Common.Models.Database
{
    public class CustomActiveDirectoryField : AppDbSetBase, IActiveDirectoryField
    {

        [Required]
        public string FieldName { get; set; }
        [Required]
        public string DisplayName { get; set; }

        public ActiveDirectoryFieldType FieldType { get; set; } = ActiveDirectoryFieldType.Text;

        [Required]
        public List<ActiveDirectoryFieldObjectType> ObjectTypes { get; set; }


        public override string? ToString()
        {
            return FieldName;
        }
        public override int GetHashCode()
        {
            if (FieldName == null) return Id.GetHashCode();
            return FieldName.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            if (obj is ActiveDirectoryField)
            {
                var other = obj as ActiveDirectoryField;

                if (other.FieldName == this.FieldName)
                {
                    return true;
                }

            }
            return false;
        }
        public bool IsActionAppropriateForObject(ActiveDirectoryObjectType objectType)
        {

            switch (objectType)
            {
                case ActiveDirectoryObjectType.User:
                    switch (FieldName)
                    {
                        case "city":
                        case "cn":
                        case "company":
                        case "depatment":
                        case "description":
                        case "displayName":
                        case "distinguishedName":
                        case "employeedId":
                        case "givenname":
                        case "homeDirectory":
                        case "homeDrive":
                        case "homePhone":
                        case "mail":
                        case "memberOf":
                        case "middleName":
                        case "objectSID":
                        case "pager":
                        case "physicalDeliveryOffice":
                        case "postalCode":
                        case "profilePath":
                        case "samaccountname":
                        case "scriptPath":
                        case "site":
                        case "sn":
                        case "st":
                        case "street":
                        case "streetAddress":
                        case "telephoneNumber":
                        case "title":
                        case "userPrincipalName":
                            return true;
                    }
                    break;
                case ActiveDirectoryObjectType.Computer:
                    switch (FieldName)
                    {
                        case "cn":
                        case "description":
                        case "displayName":
                        case "distinguishedName":
                        case "memberOf":
                        case "objectSID":
                        case "samaccountname":
                        case "site":
                            return true;
                    }
                    break;

                case ActiveDirectoryObjectType.Group:
                    switch (FieldName)
                    {
                        case "cn":
                        case "description":
                        case "displayName":
                        case "distinguishedName":
                        case "mail":
                        case "memberOf":
                        case "objectSID":
                        case "samaccountname":
                        case "site":
                            return true;
                    }
                    break;

                case ActiveDirectoryObjectType.OU:
                    switch (FieldName)
                    {
                        case "cn":
                        case "description":
                        case "displayName":
                        case "distinguishedName":
                        case "objectSID":
                        case "site":
                            return true;


                    }
                    break;


            }
            return false;


        }


    }

}
