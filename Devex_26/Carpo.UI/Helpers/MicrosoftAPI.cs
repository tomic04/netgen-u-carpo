using Carpo.Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace Carpo.UI.Helpers
{
    internal static class MicrosoftAPI
    {
        internal static void AddContactToOutlook(OutlookModel data)
        {
            try
            {
                Outlook.Application outlookApp = new Outlook.Application();
                Outlook.MAPIFolder contactsFolder = outlookApp.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);


                Outlook.Items contactItems = contactsFolder.Items;

                string filter = $"[Email1Address] = '{data.PrivateEmail}'";

                Outlook.ContactItem existingContact = contactItems.Find(filter) as Outlook.ContactItem;

                if (existingContact != null)
                {

                    existingContact.FirstName = data.Name;
                    existingContact.LastName = data.Surname;
                    existingContact.BusinessAddressStreet = data.CompanyAddress;
                    existingContact.BusinessAddressCity = data.CompanyCity;
                    existingContact.BusinessAddressPostalCode = data.CompanyPostalCode;
                    existingContact.CompanyName = data.Company;
                    existingContact.Title = data.Title;
                    existingContact.JobTitle = data.JobTitle;
                    existingContact.Email1Address = data.PrivateEmail;
                    existingContact.Email2Address = data.CompanyEmail;
                    existingContact.MobileTelephoneNumber = data.PrivateMobile;
                    existingContact.HomeTelephoneNumber = data.PrivateHomeNumber;
                    existingContact.BusinessTelephoneNumber = data.CompanyMobile;
                    existingContact.Business2TelephoneNumber = data.CompanyHomeNumber;
                    existingContact.HomeAddressStreet = data.PrivateAddress;
                    existingContact.HomeAddressCity = data.PrivateCity;
                    existingContact.HomeAddressPostalCode = data.PrivatePostalCode;

                    existingContact.Save();
                }
                else
                {
                    Outlook.ContactItem newContact = (Outlook.ContactItem)outlookApp.CreateItem(Outlook.OlItemType.olContactItem);

                    newContact.FirstName = data.Name;
                    newContact.LastName = data.Surname;
                    newContact.BusinessAddressStreet = data.CompanyAddress;
                    newContact.BusinessAddressCity = data.CompanyCity;
                    newContact.BusinessAddressPostalCode = data.CompanyPostalCode;
                    newContact.CompanyName = data.Company;
                    newContact.Title = data.Title;
                    newContact.JobTitle = data.JobTitle;
                    newContact.Email1Address = data.PrivateEmail;
                    newContact.Email2Address = data.CompanyEmail;
                    newContact.MobileTelephoneNumber = data.PrivateMobile;
                    newContact.HomeTelephoneNumber = data.PrivateHomeNumber;
                    newContact.BusinessTelephoneNumber = data.CompanyMobile;
                    newContact.Business2TelephoneNumber = data.CompanyHomeNumber;
                    newContact.HomeAddressStreet = data.PrivateAddress;
                    newContact.HomeAddressCity = data.PrivateCity;
                    newContact.HomeAddressPostalCode = data.PrivatePostalCode;

                    newContact.Save();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
