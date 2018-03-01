using System;
using System.Collections.Generic;
using System.Linq;
using BTS__User__Mangement__API.EmailComposer;
using System.Web;

namespace BTS__User__Mangement__API.Models
{
    public class EmailCommunication
    {
        public static string SenEmail(string username, string password, string logoUrl, string emailSubject, string contentHeading, string mainText, bool hasProfile, string profilePicUrl, BTS__User__Mangement__API.EmailComposer.ArrayOfProfileInfo profileInfo, BTS__User__Mangement__API.EmailComposer.ArrayOfActionList actionLists, string detailHeading, BTS__User__Mangement__API.EmailComposer.ArrayOfString detailList, string recepientEmail)
        {
            emailComposerSoapClient MailSender = new emailComposerSoapClient();
            string status = MailSender.queueEmail(username, password, logoUrl, emailSubject, contentHeading, mainText, hasProfile, profilePicUrl, profileInfo, actionLists, detailHeading, detailList, recepientEmail);
            return status;
        }
    }
}