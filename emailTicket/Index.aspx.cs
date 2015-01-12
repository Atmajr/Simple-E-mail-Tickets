﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

//QP emailTicket 1.0
//author: Justin R. Acker
//company: PracticePro Software Systems
//purpose: allow end users to easily create e-mail delivered trouble tickets for issues with other software
//usage: see readme.txt

//TO DO:

//VERSION 2
//- "help me find my CIN" animated gif
//- link to DB to hold record of tickets
//- read-only link to inHouse - based on CIN, auto-deny unsupported clients, and auto-fill their username, e-mail, and phone

//VERSION 3
//- have emailTicket create inHouse call logs for client who have opened a ticket through the system

//VERSION 4
//- create a "first-time run" and/or "configuration" page to help manage the text files

namespace emailTicket
{

    public partial class Index : System.Web.UI.Page
    {
        private string[] emailArray = new string[] { };
        private string[] managerEmail = new string[] { };
        private string[] generalEmail = new string[] { };
        private int emailCount;
        private int numberOfEmails = 0;
        private int ccToManager = 0;
        private int ccToGeneral = 0;
        private int autoRespond = 1;
        private long ticketCounter = 0;
        private string autoResponse = "";
        private string ticketTemplate = "";
        private string productType = "";
        private string registeredName = "";
        private string productVersion = "";
        private string osVersion = "";
        private string method = "";

        protected void Page_Load(object sender, EventArgs e)
        {
            startValidation(); //turn on real-time validation of applicable fields

            //define some lists for array extensibility
            List<string> emailList = emailArray.ToList<string>();
            List<string> managerList = managerEmail.ToList<string>();
            List<string> generalList = generalEmail.ToList<string>();
            string addressType;
            string address;

            Dictionary<string, string> buildVersions = new Dictionary<string, string>()
            {
                {"3.1", "Windows NT 3.1"},
                {"3.5", "Windows NT 3.5"},
                {"4.0", "Windows 95/NT 4.0"},
                {"4.1", "Windows 98"},
                {"4.9", "Windows ME"},
                {"5.0", "Windows 2000"},
                {"5.1", "Windows XP"},
                {"5.2", "Windows Server 2003"},
                {"6.0", "Windows Vista"},
                {"6.1", "Windows 7"},
                {"6.2", "Windows 8/Server 2008"},
                {"6.3", "Windows 8.1"}
            };

            //check to see if the link came pre-populated with CIN and/or username
            //and, if it did, auto-populate the relevant textboxes.

            if (Request.QueryString.Count > 0)
            {
                method = "External Software";

                try
                {
                    string cin = Request.QueryString["CIN"];
                    if (!(string.IsNullOrEmpty(cin)))
                    {
                        textboxCIN.Text = cin;
                    }
                }
                catch
                {
                    return;
                }

                try
                {
                    registeredName = Request.QueryString["RegisteredName"];

                }
                catch
                {
                    return;
                }


                try
                {
                    productType = Request.QueryString["ProductType"];
                }
                catch
                {
                    return;
                }

                try
                {
                    productVersion = Request.QueryString["ProductVersion"];
                }
                catch
                {
                    return;
                }

                try
                {
                    osVersion = Request.QueryString["OSVersion"];
                    string truncatedVersion = osVersion.Substring(0, 3);
                    if (buildVersions.ContainsKey(truncatedVersion))
                    {
                        osVersion = buildVersions[truncatedVersion];
                    }
                    else
                    {
                        osVersion = "Windows Build " + osVersion;
                    }

                }
                catch
                {
                    return;
                }
            }

            else
            {
                method = "Web Site";
            }

            //open emailList.txt and save each line as a value in an array
            string[] lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"emailList.txt"));

            //iterate through the array an organize the e-mail addresses into their respective lists
            foreach (string line in lines)
            {
                addressType = line.Substring(0, 7);
                address = line.Substring(8);
                switch (addressType)
                {
                    case "manager":
                        managerList.Add(address);
                        break;

                    case "general":
                        generalList.Add(address);
                        break;

                    case "support":
                        numberOfEmails++; //increment the number of support e-mails for later use
                        emailList.Add(address);
                        break;       
                }
            }


            //open autoResponse.txt and save each line as a value in an array
            string[] autoResponseLines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"autoResponse.txt"));

            //combine the lines in a single string with proper line breaks
            foreach (string line in autoResponseLines)
            {
                autoResponse = autoResponse + line + "\r\n";
            }

            //open ticketTemplate.txt and save each line as a value in an array
            string[] ticketTemplateLines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"ticketTemplate.txt"));

            //combine the lines in a single string with proper line breaks
            foreach (string line in ticketTemplateLines)
            {
                ticketTemplate = ticketTemplate + line + "\r\n";
            }

            //convert the lists to arrays for easier programmability
            emailArray = emailList.ToArray<string>();
            managerEmail = managerList.ToArray<string>();
            generalEmail = generalList.ToArray<string>();

            string jumble = "";
            lines = File.ReadAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config.txt"));

            foreach (string line in lines)
            {
                jumble += line;
            }

            string[] split = jumble.Split(new Char[] { '|' });
            emailCount = Convert.ToInt32(split[1]); //get the variable that determines whose turn it is
            ccToManager = Convert.ToInt32(split[3]); //should we cc: the manager e-mail addresses?
            ccToGeneral = Convert.ToInt32(split[5]); //should we cc: the general e-mail addresses?
            autoRespond = Convert.ToInt32(split[7]); //should we generate and sent an auto-response receipt?
            ticketCounter = Convert.ToInt64(split[9]); //retireve the amount of tickets sent so far.
            
        }

        protected void buttonSubmit_Click(object sender, EventArgs e)
        {

            if (!submitTimeValidation()) //verify the form is valid and logical before we do anything else
                return;
            else
            {
                //collect data from the form, and define other necessary variables
                string name = textboxName.Text;
                string cin = textboxCIN.Text;
                string email = textboxEmail.Text;
                string phone = textboxPhone.Text;
                string issue = textboxIssue.Text;
                DateTime currentTime = DateTime.Now;
                string format = "MMM ddd d HH:mm yyyy";
                string ticketTime = currentTime.ToString(format);
                string contactPref = "No Preference";
                bool emailExists = false;

                ticketCounter++;

                if (contactPrefRadioButton.SelectedIndex == 1)
                {
                    contactPref = "Phone";
                }
                else if (contactPrefRadioButton.SelectedIndex == 0)
                {
                    contactPref = "E-mail";
                }

                if (cin == "99999")
                {
                    cin = "UNREGISTERED";
                }

                //testing only; remove before production
                //returnPanel.Controls.Add(new LiteralControl("Name: " + name + "<br />CIN: " + cin + "<br />Email: " + email + "<br />Phone: " + phone + "<br / >Preferred contact method:" + contactPref + "<br />" + issue));

                //old ticket build method
                //string body = "Client Name: " + name + "\r\n" + "CIN: " + cin + "\r\n" + "E-mail: " + email + "\r\n" + "Phone: " + phone + "\r\n" + "Preferred contact method: " + contactPref + "\r\n\r\n" + issue;
                
                //build ticket message body
                StringBuilder ticketBuilder = new StringBuilder(ticketTemplate);
                ticketBuilder.Replace("<datetime>", ticketTime);
                ticketBuilder.Replace("<name>", name);
                ticketBuilder.Replace("<cin>", cin);
                ticketBuilder.Replace("<phone>", phone);
                ticketBuilder.Replace("<email>", email);
                ticketBuilder.Replace("<contactPref>", contactPref);
                ticketBuilder.Replace("<issue>", issue);
                ticketBuilder.Replace("<registeredName>", registeredName);
                ticketBuilder.Replace("<productType>", productType);
                ticketBuilder.Replace("<productVersion>", productVersion);
                ticketBuilder.Replace("<osVersion>", osVersion);
                ticketBuilder.Replace("<method>", method);
                string ticket = ticketBuilder.ToString();

                //increment the counter to determine who gets this ticket
                if (emailCount < (numberOfEmails - 1))
                {
                    emailCount++;
                }

                else
                {
                    emailCount = 0;
                }

                //update the config file to increment that variable
                string output = "emailCount|" + emailCount + "|ccToManager|" + ccToManager + "|ccToGeneral|" + ccToGeneral + "|autorespond|" + autoRespond + "|ticketCounter|" + ticketCounter + "|";
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"config.txt");
                File.WriteAllText(path, output);

                //create and send the ticket to the support staff
                System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                message.To.Add(emailArray[emailCount]);
                if (ccToManager == 1) //should we cc the manager?
                {
                    foreach (string manager in managerEmail)
                    {
                        message.CC.Add(manager);
                    }
                }
                if (ccToGeneral == 1) //should we bcc the general e-mail
                {
                    foreach (string general in generalEmail)
                    {
                        message.Bcc.Add(general);
                    }
                }
                message.Subject = "Tech Support TroubleTicket - CIN: " + cin;
                message.From = new System.Net.Mail.MailAddress("general@yourtechsupportcompany.com");
                if (!(String.IsNullOrEmpty(email)))
                {
                    message.ReplyToList.Remove(message.From);
                    message.ReplyToList.Add(email);
                    message.Headers.Add("Reply-To", email);
                }
                message.Body = ticket;
                System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                smtp.Send(message);

                if (autoRespond == 1)
                {

                    //build auto-response message body
                    StringBuilder autoResponseBuilder = new StringBuilder(autoResponse);
                    autoResponseBuilder.Replace("<datetime>", ticketTime);
                    autoResponseBuilder.Replace("<contactPref>", contactPref);
                    autoResponseBuilder.Replace("<issue>", issue);
                    autoResponse = autoResponseBuilder.ToString();

                    //if the user specified an e-mail address, create and send a receipt
                    if (!(String.IsNullOrEmpty(email)))
                    {
                        System.Net.Mail.MailMessage autoResponseMessage = new System.Net.Mail.MailMessage();
                        autoResponseMessage.To.Add(email);
                        autoResponseMessage.Subject = "QuickPractice Trouble Ticket Receipt";
                        autoResponseMessage.From = new System.Net.Mail.MailAddress("noreply@yourtechsupportcompany.com");
                        autoResponseMessage.Body = autoResponse;
                        smtp.Send(autoResponseMessage);

                        emailExists = true;
                    }
                }

                //empty the textboxes, just in case the redirect is hijacked
                ClearTextBoxes(form1);

                //redirect to the completion page, and let it know whether the user specified an e-mail address
                Response.Redirect("Complete.aspx?emailExists=" + emailExists.ToString());

                //returnPanel.Controls.Add(new LiteralControl("<div class=label>Thank you for using the QuickPractice Online Support System!<br />If you specified an e-mail address, you will receive a receipt of this ticket.<br />A support representative will contact you as soon as a solution to your issue has been identified."));

            }
            
        }

        void startValidation()
        {
            //triggers real-time field validation on textChanged
            textboxName.TextChanged -= textChanged;
            textboxName.TextChanged += textChanged;
            textboxCIN.TextChanged -= textChanged;
            textboxCIN.TextChanged += textChanged;
            textboxEmail.TextChanged -= textChanged;
            textboxEmail.TextChanged += textChanged;
            textboxPhone.TextChanged -= textChanged;
            textboxPhone.TextChanged += textChanged;
            textboxIssue.TextChanged -= textChanged;
            textboxIssue.TextChanged += textChanged;
        }
        
        void textChanged(object sender, EventArgs e)
        {
            //causes realtime validation to fire
            Validate();
        }

        public bool submitTimeValidation()
        {
            //check various ticket-breaking logic before allowing the ticket to be submitted

            if (string.IsNullOrEmpty(textboxCIN.Text))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>CIN is required!"));
                return false;
            }

            else if (!(System.Text.RegularExpressions.Regex.IsMatch(textboxCIN.Text, @"\d{5}?")))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>CIN appears invalid! CIN should be five numbers. Are you missing a zero?"));
                return false;
            }

            else if (string.IsNullOrEmpty(textboxIssue.Text))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>You must describe your issue!"));
                return false;
            }

            else if ((string.IsNullOrEmpty(textboxEmail.Text)) && (string.IsNullOrEmpty(textboxPhone.Text)))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>Either phone number or e-mail is required."));
                return false;
            }

            else if ((string.IsNullOrEmpty(textboxEmail.Text)) && (contactPrefRadioButton.SelectedIndex == 0))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>You indicated you'd prefer to be contacted by e-mail, but you did not specify an e-mail address."));
                return false;
            }

            else if ((string.IsNullOrEmpty(textboxPhone.Text)) && (contactPrefRadioButton.SelectedIndex == 1))
            {
                errorPanel.Controls.Add(new LiteralControl("<font color='red'>You indicated you'd prefer to be contacted by phone, but you did not specify a phone number."));
                return false;
            }

            else
            {
                return true;
            }

        }

        public void ClearTextBoxes(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                {
                    ((TextBox)(c)).Text = string.Empty;
                }
            }
        }

    }

}