using System;

namespace Email2Zalo
{
    public class EmailObject
    {
        public EmailObject() { }
        public EmailObject(string pBody)
        {
            Body = pBody;
        }

        public string ToStringInfo()
        {
            string s;
            s = "Tel: " + this.Phone + "\n";
            if (!string.IsNullOrEmpty(this.Name))
                s += "Tên: " + this.Name + "\n";
            if (!string.IsNullOrEmpty(this.Email))
                s += "Email: " + this.Email + "\n";
            if (!string.IsNullOrEmpty(this.Site))
                s += "Site: " + this.Site + "\n";
            if (!string.IsNullOrEmpty(this.From))
                s += "Gửi từ: " + this.From + "\n";

            s += "Nội Dung: " + this.Content;

            return s;
        }

        public override string ToString()
        {
            string s;
            s = this.Phone;
            if (string.IsNullOrEmpty(s) && !string.IsNullOrEmpty(this.Email))
                s = this.Email;

            if (!string.IsNullOrEmpty(this.Name))
                s += " ," + this.Name;
            if (!string.IsNullOrEmpty(this.Content))
                s += ", " + this.Content;
            if (!string.IsNullOrEmpty(this.From))
                s += ", " + this.From;

            return s;            
        }
        //
        public string Site { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string From { get; set; }
        public string Email { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }

        private string _body;
         
        public string Body
        {
            get { return _body; }
            set
            {
                _body = value;
                int iStart = 0;
                int iEnd = 0;
                int iLen = 0;

                string keyword = "\r\nSite";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Site = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "\r\nĐược gửi từ";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.From = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "\r\nURL";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.From = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "<br/> URL";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.Length;
                    this.From = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }                            

                keyword = "\r\nEmail";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Email = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "\r\nSố Điện Thoại";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Phone = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }
                keyword = "\r\nSố điện thoại";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Phone = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }
                keyword = "<br/>Số điện thoại";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("<br/>", iStart + iLen);
                    this.Phone = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "\r\nNội Dung";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Content = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }

                keyword = "\r\nTên Quý Khách";
                iStart = _body.IndexOf(keyword);
                if (iStart > 0)
                {
                    iLen = _body.IndexOf(":", iStart) - iStart + 1;
                    iEnd = _body.IndexOf("\r\n", iStart + iLen);
                    this.Name = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                }
                
                if (string.IsNullOrEmpty(this.Name))
                {
                    keyword = "\r\nHọ và tên";
                    iStart = _body.IndexOf(keyword);
                    if (iStart > 0)
                    {
                        iLen = _body.IndexOf(":", iStart) - iStart + 1;
                        iEnd = _body.IndexOf("\r\n", iStart + iLen);
                        this.Name = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                    }
                    else
                    {
                        keyword = "Họ và tên";
                        iStart = _body.IndexOf(keyword);
                        if (iStart >= 0)
                        {
                            iLen = _body.IndexOf(":", iStart) - iStart + 1;
                            iEnd = _body.IndexOf("<br/>", iStart + iLen);
                            this.Name = _body.Substring(iStart + iLen, iEnd - iStart - iLen).Trim();
                        }
                    }
                }
            }
        }
    }
}
