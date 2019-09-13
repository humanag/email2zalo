using System;
using System.Text;
using System.Threading;
using System.Xml;
using S22.Imap;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;

namespace Email2Zalo
{
    public enum Status
    {
        Init,
        ClientSecretsError,
        CredentialValid,
        TokenInvalid,
        Running
    }

    class UserCredential
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool Ssl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    class EmailBusiness : IDisposable
    {
        public delegate bool ProcessEmail(string pBody);
        public static void EmailDispose()
        {
            if (_instance != null)
                _instance.Dispose();
        }

        public event ProcessEmail ProcessEmailText;

        private EmailBusiness()
        {
            Mailbox = "Inbox";
            Status = Status.Init;            
        }

        private static EmailBusiness _instance = null;
        public static EmailBusiness Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new EmailBusiness();

                return _instance;
            }
        }

        public Status Status { get; set; }
        public string Mailbox { get; set; }
        public string Subjects { get; set; }
        public string Senders { get; set; }
        private SearchCondition _searchCondition = null;

        private UserCredential _credential = null;
        
        public UserCredential Credential
        {
            get
            {
                if (_credential == null)
                    _credential = new UserCredential();

                return _credential;
            }
        }
        
        public void InitCredential()
        {         
           
            _searchCondition = new SearchCondition();

            SearchCondition condition = null;
            if (!string.IsNullOrEmpty(Senders))
            {
                List<string> mSenders = Senders.Split(';').ToList();
                condition = SearchCondition.From(mSenders[0]);
                for (int i=1; i < mSenders.Count; i++)
                {
                    string sender = mSenders[i];
                    if (string.IsNullOrEmpty(sender))
                        continue;

                    condition = condition.Or(SearchCondition.From(sender));
                }
            }

            SearchCondition conditionSubject = null;
            if (!string.IsNullOrEmpty(Subjects))
            {
                List<string> mSubjects = Subjects.Split(';').ToList();
                conditionSubject = SearchCondition.Subject(mSubjects[0]);
                for (int i = 1; i < mSubjects.Count; i++)
                {
                    string sub = mSubjects[i];
                    if (string.IsNullOrEmpty(sub))
                        continue;

                    conditionSubject = conditionSubject.Or(SearchCondition.Subject(sub));
                }
            }
            
            _searchCondition = SearchCondition.Unseen();
            
            if (condition != null)
                _searchCondition = _searchCondition.And(condition);

            if (conditionSubject != null)
                _searchCondition = _searchCondition.And(conditionSubject);
                        
            try
            {
                if (_email != null) _email.Logout();
            }
            catch
            {
            }

            try
            {                

                if (!string.IsNullOrEmpty(Credential.UserName) && !string.IsNullOrEmpty(Credential.Host))
                    _email = new ImapClient(Credential.Host, Credential.Port, Credential.UserName, Credential.Password, AuthMethod.Login, Credential.Ssl);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                throw ex;
            }
                
        }

        private ImapClient _email = null;
        private ImapClient EMailClient
        {
            get
            {
                if (_email == null)
                {
                    try
                    {
                        _email = new ImapClient(Credential.Host, Credential.Port, Credential.UserName, Credential.Password, AuthMethod.Login, Credential.Ssl);
                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message);
                    }
                }

                return _email;
            }
        }

        public void Revoke()
        {
            _credential = null;

            if (_email != null)
            {
                _email.Logout();
                _email.Dispose();
                _email = null;
            }
        }

        public long? MailCheckMessages()
        {
            // Request should fail now - invalid grant.
            try
            {
                return TryCheckMessages();
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                return -1;
            }
        }        

        private long? TryCheckMessages()
        {
            long nr = 0;
            IEnumerable<uint> uids = null;
            try
            {
                // Returns a collection of identifiers of all mails matching the specified search criteria.
                uids = EMailClient.Search(_searchCondition, Mailbox);
            }
            catch(Exception ex)
            {
                Console.Write(ex.Message);
                //
                _email = new ImapClient(Credential.Host, Credential.Port, Credential.UserName, Credential.Password, AuthMethod.Login, Credential.Ssl);
                uids = EMailClient.Search(_searchCondition, Mailbox);
            }

            try
            {
                // Download mail messages from the default mailbox.
                // IEnumerable<MailMessage> messages = client.GetMessages(uids, false);
                foreach (var id in uids)
                {
                    MailMessage message = EMailClient.GetMessage(id, FetchOptions.TextOnly, false);
                    var s = message.Body;
                    
                    //Xu ly email, neu thanh cong danh dau da xem.
                    if (ProcessEmailBody(s))
                    {
                        EMailClient.SetMessageFlags(id, null, MessageFlag.Seen);
                    }
                    nr++;

                    if (nr > 10)
                        break;

                    Thread.Sleep(300);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            return nr;
        }

        private bool ProcessEmailBody(string s)
        {
            if (ProcessEmailText != null)
                return ProcessEmailText(s);

            return true;
        }

        internal void Authorize()
        {
            InitCredential();
        }

        public void Dispose()
        {
            if (_email != null)
            {
                _email.Dispose();
            }
        }

        internal bool Testlogin(string pHost, int pPort, string pUserName, string pPassword, bool pSsl)
        {
            ImapClient client = new ImapClient(pHost, pPort, pSsl);
            try
            {
                client.Login(pUserName, pPassword, AuthMethod.Login);
                client.Logout();
            }
            catch(Exception ex)
            {
                throw ex;
                //return false;
            }

            return true;
        }
    }
}