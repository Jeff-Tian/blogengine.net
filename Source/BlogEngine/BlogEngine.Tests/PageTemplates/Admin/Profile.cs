using WatiN.Core;

namespace BlogEngine.Tests.PageTemplates.Admin
{
    public class Profile : Page
    {
        public string Url
        {
            get { return Constants.Root + "/admin/Users/Profile.aspx?id={0}"; }
        }

        public TextField FirstName { get { return Document.TextField(Find.ById("txtFirstName")); } }
        public TextField LastName { get { return Document.TextField(Find.ById("txtLastName")); } }
        public TextField MiddleName { get { return Document.TextField(Find.ById("txtMiddleName")); } }
        public TextField DisplayName { get { return Document.TextField(Find.ById("txtDispalayName")); } }
        public TextField Email { get { return Document.TextField(Find.ById("txtEmail")); } }
        public TextField Birthday { get { return Document.TextField(Find.ById("txtBirthday")); } }
        public TextField Photo { get { return Document.TextField(Find.ById("txtPhotoURL")); } }

        public TextField Mobile { get { return Document.TextField(Find.ById("txtMobile")); } }
        public TextField City { get { return Document.TextField(Find.ById("txtCity")); } }
        public TextField MainPhone { get { return Document.TextField(Find.ById("txtMainPhone")); } }
        public TextField State { get { return Document.TextField(Find.ById("txtState")); } }
        public TextField Fax { get { return Document.TextField(Find.ById("txtFax")); } }
        public TextField Country { get { return Document.TextField(Find.ById("txtCountry")); } }
        public TextField Biography { get { return Document.TextField(Find.ById("biography")); } }

        public Button BtnSave { get { return Document.Button(Find.ById("btnSave")); } }
    }
}
