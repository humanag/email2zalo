using Bms.Windows.Tools;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace Email2Zalo
{
    public partial class SettingsForm : Form
    {
        private Configuration config = new Configuration();

        public int CountSend { get; private set; } = 0;
        
        public SettingsForm()
        {
            config.Load();
            
            InitializeComponent();

            //NativeMethods.PreventSleep();
            
            SetControlsValue();

            this.Icon = MyResources.Instance.ApplicationIcon;
            notifyIcon.Icon = MyResources.Instance.NotificationInit;

            UpdateRequestQ();
            UpdateInterval();
            progressBar1.Visible = false;
            // CheckMail();
            // timerCheck.Start();
        }

        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            GetControlsValue();
            config.Save();

            UpdateRequestQ();
            UpdateInterval();
            UpdateOnStart();

            //this.Hide();
            //CheckMail();
        }

        private void UpdateRequestQ()
        {
            EmailBusiness.Instance.Credential.Host = config.Host;
            EmailBusiness.Instance.Credential.Port = config.Port;
            EmailBusiness.Instance.Credential.Ssl = config.Ssl;

            EmailBusiness.Instance.Credential.UserName = config.UserName;
            EmailBusiness.Instance.Credential.Password = config.Password;

            EmailBusiness.Instance.Mailbox = config.Mailbox;
            EmailBusiness.Instance.Senders = config.CustomSender;
            EmailBusiness.Instance.Subjects = config.CustomSubject;
            EmailBusiness.Instance.ProcessEmailText += this.ProcessEmail;
            EmailBusiness.Instance.InitCredential();
        }

        private bool ProcessEmail(string pBody)
        {
            EmailObject emailObject = new EmailObject(pBody);

            if (string.IsNullOrEmpty(emailObject.Phone) && string.IsNullOrEmpty(emailObject.Content))
                return false;
            bool result = true;
            string text = emailObject.ToString();
            result = SendToZalo(text); 
            WriteToLog(text, result);
            
            return result;
        }

        private bool SendToZalo(string text)
        {
            var result = ZaloBusiness.Instance.SendMessage(text);
            return result;
        }

        private void WriteToLog(string pText, bool pState = true)
        {
            string mText = "\r\n"; //Dong moi
            
            //Neu gui mail thanh cong, tang so dem, va ghi log
            if (pState) { CountSend++; mText += Convert.ToString(CountSend) + " - "; }

            mText  += DateTime.Now.ToString() + ": " + pText;
            rtbLog.AppendText(mText);
        }

        private void UpdateInterval()
        {
            this.timerCheck.Interval = config.IntervalInMillisecond;
        }

        private void UpdateOnStart()
        {
            if (this.cbOnStart.Checked)
                StartApplicationOnWindowsStartup.Enable();
            else
                StartApplicationOnWindowsStartup.Disable();
        }

        private void GetControlsValue()
        {
            GetServerInfo();
            GetFilter();
            GetInterval();
            GetZaloInfo();
        }

        private void SetControlsValue()
        {
            SetZaloInfo();
            SetServerInfo();
            SetFilter();
            SetInterval();
            SetOnStart();
        }

        private void SetZaloInfo()
        {
            tbZaloContact.Text = config.ZaloContact;
        }

        private void SetOnStart()
        {
            cbOnStart.Checked = StartApplicationOnWindowsStartup.IsEnabled();
        }
        
        private void GetZaloInfo()
        {
            config.ZaloContact = tbZaloContact.Text.Trim();
        }

        private void GetServerInfo()
        {
            config.Host = tbHost.Text;
            config.Port = (int)numPort.Value;
            config.Ssl = ckSsl.Checked;

            config.UserName = tbUserName.Text;
            config.Password = tbPassword.Text;
        }

        private void SetServerInfo()
        {
            tbHost.Text = config.Host;
            numPort.Value = config.Port;
            ckSsl.Checked = config.Ssl;

            tbUserName.Text = config.UserName;
            tbPassword.Text = config.Password;
        }

        private void GetFilter()
        {
            config.FilterType = FilterMailBoxEnum.InboxOnly;
            if (rbAll.Checked)
                config.FilterType = FilterMailBoxEnum.AllMessageExceptSpamAndTrash;

            config.CustomSender = this.tbSender.Text;
            config.CustomSubject = this.tbSubject.Text;
        }

        private void SetFilter()
        {
            this.tbSender.Text = config.CustomSender;
            this.tbSubject.Text = config.CustomSubject;
            switch (config.FilterType)
            {
                case FilterMailBoxEnum.AllMessageExceptSpamAndTrash:
                    this.rbAll.Checked = true;
                    break;
                default: // FilterEnum.InboxOnly
                    this.rbInbox.Checked = true;
                    break;
            }
        }

        private void GetInterval()
        {
            this.config.IntervalInMinutes = (int) this.numIntervalMinutes.Value;
        }

        private void SetInterval()
        {
            this.numIntervalMinutes.Value = this.config.IntervalInMinutes;
            this.timerCheck.Interval = this.config.IntervalInMillisecond;
        }

        private void viewOptionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetControlsValue();
            this.Show();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            SetControlsValue();
            this.Show();
        }

        private void timerCheck_Tick(object sender, EventArgs e)
        {
            CheckMail();
        }


        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CleanExit();
        }

        private void CleanExit()
        {
            this.timerCheck.Stop();
            this.notifyIcon.Visible = false;
            EmailBusiness.EmailDispose();
            ZaloBusiness.ZaloDispose();
            Application.Exit();
        }

        private void checkMailNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CheckMail();
        }

        private void inboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewInbox();
        }


        private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm a = new AboutForm();
            a.ShowDialog();
        }

        private void revokeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EmailBusiness.Instance.Revoke();
            CleanExit();
        }

        private void ViewInbox()
        {
            Process.Start(config.InboxUrl);
        }

        private void CheckMail()
        {
            timerCheck.Stop();
            try
            {
                long? messagesCount = EmailBusiness.Instance.MailCheckMessages();
                this.notifyIcon.Icon = (messagesCount.GetValueOrDefault() > 0) ? MyResources.Instance.NotificationMessages : MyResources.Instance.NotificationNoMessage;
            }
            catch(Exception ex)
            {
                WriteToLog(ex.Message, false);
                this.notifyIcon.Icon = MyResources.Instance.NotificationError;
            }
            finally
            {
                StartTimer();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ZaloBusiness.Instance.Contact = config.ZaloContact;
            ZaloBusiness.Instance.OpenUrl();
            Thread.Sleep(1000);
            StartTimer();
            btTestLoginEmail.Enabled = false;
        }

        private void StartTimer()
        {
            timerCheck.Start();
            progressBar1.Visible = true;
        }

        private void btTestLoginEmail_Click(object sender, EventArgs e)
        {
            try
            {
                if (EmailBusiness.Instance.Testlogin(tbHost.Text, (int)numPort.Value, tbUserName.Text, tbPassword.Text, ckSsl.Checked))
                    WriteToLog("Test email done", false);
                else
                    WriteToLog("Test email false", false);
            }
            catch(Exception ex)
            {
                WriteToLog("Test email " + ex.Message, false);
            }
        }
    }
}
