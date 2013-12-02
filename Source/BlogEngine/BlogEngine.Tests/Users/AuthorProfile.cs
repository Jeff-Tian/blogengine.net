using System;
using System.IO;
using NUnit.Framework;
using BlogEngine.Tests.PageTemplates.Admin;

namespace BlogEngine.Tests.Users
{
    [TestFixture]
    public class AuthorProfile : BeTest
    {
        [SetUp]
        public void Init()
        {
            Login("admin");
        }

        [Test]
        public void CanAddUpdateAndDeleteUserProfile()
        {
            // create user
            var u = ie.Page<BlogEngine.Tests.PageTemplates.Admin.Users>();

            ie.GoTo(u.Url);
            u.AddNew.Click();

            TypeQuickly(u.UserName, "testuser");
            TypeQuickly(u.Email, "me@us.com");
            TypeQuickly(u.Password, "test");
            TypeQuickly(u.Password2, "test");

            u.Save.Click();
            Wait(1);

            // create profile
            FillProfile(
                "testuser", 
                "Smith",
                "XX", 
                "test user", 
                "me@us.com", 
                "http://path/to/photo", 
                "111-111-1111", 
                "222-222-2222", 
                "333-333-3333", 
                "Chicago", 
                "IL", 
                "USA", 
                "something about me"
            );          
            ValidateProfile(
                "testuser",
                "Smith",
                "XX",
                "test user",
                "me@us.com",
                "http://path/to/photo",
                "111-111-1111",
                "222-222-2222",
                "333-333-3333",
                "Chicago",
                "IL",
                "USA",
                "something about me"
            );

            // update profile
            FillProfile(
                "testuser",
                "Johnson",
                "ZZ",
                "test updated",
                "he@us.com",
                "http://path/to/another/photo",
                "444-444-4444",
                "555-555-5555",
                "666-666-6666",
                "Edmonton",
                "Alberta",
                "Canada",
                "something else about me"
            );
            ValidateProfile(
                "testuser",
                "Johnson",
                "ZZ",
                "test updated",
                "he@us.com",
                "http://path/to/another/photo",
                "444-444-4444",
                "555-555-5555",
                "666-666-6666",
                "Edmonton",
                "Alberta",
                "Canada",
                "something else about me"
            );

            // delete user and profile
            ie.GoTo(u.Url);
            u.Delete.Click();
        }

        void FillProfile(string firstName, string lastName, string middleName, string displayName, string email, string photo, string mobile, string phone, string fax, string city, string state, string country, string aboutme)
        {
            var p = ie.Page<Profile>();
            ie.GoTo(string.Format(p.Url, firstName));

            TypeQuickly(p.FirstName, firstName);
            TypeQuickly(p.LastName, lastName);
            TypeQuickly(p.MiddleName, middleName);
            TypeQuickly(p.DisplayName, displayName);
            TypeQuickly(p.Email, email);
            TypeQuickly(p.Photo, photo);
            TypeQuickly(p.Mobile, mobile);
            TypeQuickly(p.MainPhone, phone);
            TypeQuickly(p.Fax, fax);
            TypeQuickly(p.City, city);
            TypeQuickly(p.State, state);
            TypeQuickly(p.Country, country);
            TypeQuickly(p.Biography, aboutme);

            p.BtnSave.Click();
            Wait(1);
            ie.GoTo(Constants.Root);
            Wait(1);
            ie.GoTo(string.Format(p.Url, firstName));
        }

        void ValidateProfile(string firstName, string lastName, string middleName, string displayName, string email, string photo, string mobile, string phone, string fax, string city, string state, string country, string aboutme)
        {
            Assert.IsTrue(ie.Html.Contains(firstName));
            Assert.IsTrue(ie.Html.Contains(lastName));
            Assert.IsTrue(ie.Html.Contains(middleName));
            Assert.IsTrue(ie.Html.Contains(displayName));
            Assert.IsTrue(ie.Html.Contains(email));
            Assert.IsTrue(ie.Html.Contains(photo));
            Assert.IsTrue(ie.Html.Contains(mobile));
            Assert.IsTrue(ie.Html.Contains(phone));
            Assert.IsTrue(ie.Html.Contains(fax));
            Assert.IsTrue(ie.Html.Contains(city));
            Assert.IsTrue(ie.Html.Contains(state));
            Assert.IsTrue(ie.Html.Contains(country));
            Assert.IsTrue(ie.Html.Contains(aboutme));
        }

        //[TearDown]
        //public void Dispose()
        //{
        //    var p = ie.Page<Profile>();

        //    ie.GoTo(string.Format(p.Url, "Admin"));

        //    TypeQuickly(p.LastName, "");
        //    TypeQuickly(p.MiddleName, "");
        //    TypeQuickly(p.Email, "");
        //    TypeQuickly(p.Photo, "");

        //    TypeQuickly(p.Mobile, "");
        //    TypeQuickly(p.MainPhone, "");
        //    TypeQuickly(p.Fax, "");
        //    TypeQuickly(p.City, "");
        //    TypeQuickly(p.State, "");
        //    TypeQuickly(p.Country, "");
        //    TypeQuickly(p.Biography, "");

        //    p.BtnSave.Click();
        //}
    }
}
