namespace Email2Zalo
{
    class Configuration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }

        public string UserName { get; set; }
        public string Password { get; set; }

        public int IntervalInMinutes { get; set; }
        
        public string CustomSender { get; set; }
        public string CustomSubject { get; set; }

        public string ZaloContact { get; set; }
        public string ZaloUserName { get; set; }
        public string ZaloPassword { get; set; }

        public FilterMailBoxEnum FilterType { get; set; }

        public string Mailbox
        {
            get
            {
                string q = "Inbox";
                
                if (FilterType == FilterMailBoxEnum.AllMessageExceptSpamAndTrash)
                    q = null;

                return q;
            }
        }

        private const int MINUTE_IN_MILLISECOND = 60000;
        public int IntervalInMillisecond
        {
            get
            {
                if (this.IntervalInMinutes > 0)
                    return this.IntervalInMinutes * MINUTE_IN_MILLISECOND;
                else
                    return MINUTE_IN_MILLISECOND;
            }
        }        

        public void Load()
        {
            this.Host = (string)Properties.Settings.Default["ServerHost"];
            this.Port = (int)Properties.Settings.Default["ServerPort"];
            this.Ssl = (bool)Properties.Settings.Default["ServerSsl"];

            this.UserName = (string)Properties.Settings.Default["UserName"];
            this.Password = (string)Properties.Settings.Default["Password"];
            this.ZaloContact = (string)Properties.Settings.Default["ZaloContact"];

            this.CustomSender = (string)Properties.Settings.Default["CustomSender"];
            this.CustomSubject = (string)Properties.Settings.Default["CustomSubject"];
            this.FilterType = (FilterMailBoxEnum)Properties.Settings.Default["FilterType"];
            this.IntervalInMinutes = (int)Properties.Settings.Default["IntervalInMinutes"];
        }

        public void Save()
        {
            Properties.Settings.Default["ServerHost"] = this.Host;
            Properties.Settings.Default["ServerPort"] = this.Port;
            Properties.Settings.Default["ServerSsl"] = this.Ssl;

            Properties.Settings.Default["UserName"] = this.UserName;
            Properties.Settings.Default["Password"] = this.Password;

            Properties.Settings.Default["ZaloContact"] = this.ZaloContact;

            Properties.Settings.Default["CustomSender"] = this.CustomSender;
            Properties.Settings.Default["CustomSubject"] = this.CustomSubject;
            Properties.Settings.Default["FilterType"] = (int)this.FilterType;
            Properties.Settings.Default["IntervalInMinutes"] = this.IntervalInMinutes;

            Properties.Settings.Default.Save();
        }       

        private const string URL_INBOX = @"https://mail.google.com";
        public string InboxUrl
        {
            get
            {
                return URL_INBOX;
            }
        }
    }
}
