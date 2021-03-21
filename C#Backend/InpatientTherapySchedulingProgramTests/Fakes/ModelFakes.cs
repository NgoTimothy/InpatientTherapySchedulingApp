﻿using Bogus;
using InpatientTherapySchedulingProgram.Models;
using System.Collections.Generic;

namespace InpatientTherapySchedulingProgramTests.Fakes
{
    public static class ModelFakes
    {
        public static Faker<User>UserFake { get; set; }
        public static Faker<Therapy>TherapyFake { get; set; }
        public static Faker<TherapistActivity>TherapistActivityFake { get; set; }
        public static Faker<Location> LocationFake { get; set; }
        public static Faker<TherapistEvent> TherapistEventFake { get; set; }
        public static Faker<Permission> PermissionFake { get; set; }

        static ModelFakes()
        {
            BuildUserFakes();
            BuildTherapyFakes();
            BuildTherapistActivityFakes();
            BuildLocationFakes();
            BuildTherapistEventFakes();
            BuildPermissionFakes();
        }

        private static void BuildTherapistActivityFakes()
        {
            TherapistActivityFake = new Faker<TherapistActivity>();
            TherapistActivityFake.RuleFor(m => m.ActivityName, r => r.IndexGlobal + r.Random.AlphaNumeric(10));
            TherapistActivityFake.RuleFor(m => m.IsProductive, r => r.Random.Bool());
        }

        private static void BuildTherapyFakes()
        {
            TherapyFake = new Faker<Therapy>();
            TherapyFake.RuleFor(m => m.Adl, r => r.IndexGlobal + r.Company.CompanyName());
            TherapyFake.RuleFor(m => m.TherapyType, r => r.Random.String2(10));
            TherapyFake.RuleFor(m => m.Abbreviation, r => r.IndexGlobal + r.Company.CompanySuffix());
        }

        private static void BuildUserFakes()
        {
            UserFake = new Faker<User>();
            UserFake.RuleFor(m => m.UserId, r => r.UniqueIndex);
            UserFake.RuleFor(m => m.FirstName, r => r.Name.FirstName());
            UserFake.RuleFor(m => m.MiddleName, r => r.Name.FirstName());
            UserFake.RuleFor(m => m.LastName, r => r.Name.LastName());
            UserFake.RuleFor(m => m.Address, r => r.Address.FullAddress());
            UserFake.RuleFor(m => m.PhoneNumber, r => r.Phone.PhoneNumber());
            UserFake.RuleFor(m => m.Username, r => r.IndexGlobal + r.Person.UserName);
            UserFake.RuleFor(m => m.Password, r => r.Internet.Password());
        }

        private static void BuildLocationFakes()
        {
            LocationFake = new Faker<Location>();
            LocationFake.RuleFor(m => m.LocationId, r => r.UniqueIndex);
            LocationFake.RuleFor(m => m.Name, r => r.UniqueIndex + r.Random.String2(10));
        }

        private static void BuildTherapistEventFakes()
        {
            TherapistEventFake = new Faker<TherapistEvent>();
            TherapistEventFake.RuleFor(m => m.EventId, r => r.UniqueIndex);
            TherapistEventFake.RuleFor(m => m.TherapistId, r => r.Random.Int());
            TherapistEventFake.RuleFor(m => m.StartTime, r => r.Date.Recent());
            TherapistEventFake.RuleFor(m => m.EndTime, r => r.Date.Soon());
            TherapistEventFake.RuleFor(m => m.ActivityName, r => r.IndexGlobal + r.Random.AlphaNumeric(10));
            TherapistEventFake.RuleFor(m => m.Notes, r => r.Commerce.ProductDescription());
        }
        
        private static void BuildPermissionFakes()
        {
            List<string> roles = new List<string>
            {
                "therapist",
                "nurse",
                "admin"
            };

            PermissionFake = new Faker<Permission>();
            PermissionFake.RuleFor(m => m.UserId, r => r.UniqueIndex);
            PermissionFake.RuleFor(m => m.Role, r => roles[r.Random.Int(0, 2)]);
        }
    }
    
}
