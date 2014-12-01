using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections;


namespace CommentTroll
{
    public partial class Form1 : Form
    {

        String SessionID; //Note that %3D needs to be removed from it!
        String SteamLogin;
        String Comment;

        static CookieContainer CookC;
        Random r = new Random(Convert.ToInt32(DateTime.Now.Ticks % int.MaxValue));


        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SessionID = textBox2.Text;
            SteamLogin = textBox3.Text;
            Comment = textBox4.Text;

            CookC = new CookieContainer(); //flush it
            CookC.Add(new Cookie("sessionid", SessionID, "/", "steamcommunity.com"));
            CookC.Add(new Cookie("steamLogin", SteamLogin, "/", "steamcommunity.com"));
            CookC.Add(new Cookie("steamCC_127_0_0_1", "US", "/", "steamcommunity.com"));
            //REQUIRED: Take IP, and convert dots to _ then put it in front of "steamCC_" and set the value to a country code
            //EG: 127.0.0.1 -> steamCC_127_0_0_1 = US

            ArrayList u1 = GetUsers(textBox1.Text);

            int count = 0;

            foreach(String User in u1)
            {
                String Spun = parseSpintax(r, Comment);

                if (PostComment(Spun, User, SessionID))
                {
                    count += 1;
                    label2.Text = count.ToString();
                    this.Refresh();
                    Application.DoEvents();
                }
            }

            MessageBox.Show("Done!");
        }


        /// <summary>
        /// Returns all users as an arraylist from group URL
        /// </summary>
        /// <param name="GroupPageURL"></param>
        /// <returns>ArrayList</returns>
        public ArrayList GetUsers(String GroupPageURL)
        {
            WebClient wc = new WebClient();

            String gPage = wc.DownloadString(GroupPageURL + "/memberslistxml/?xml=1");
            int pages = Convert.ToInt32(GetSubstringByString("<totalPages>", "</totalPages>", gPage));


            String MemberList = "";
            for (int p = 1; p <= pages; p++) //Loop through all pages and smash them together. Very shitty
            {
                MemberList += wc.DownloadString(GroupPageURL + "/memberslistxml/?xml=1&p=" + p);
            }


            Regex UserRegex = new Regex(@"7656[0-9]{13}", RegexOptions.None);
            ArrayList retBuffer = new ArrayList();
            int i = 0;
 
            foreach (Match ItemMatch in UserRegex.Matches(MemberList)) //Now loop through with a regex! :D
            {
                retBuffer.Add(Convert.ToString(ItemMatch));
                i++;
            }

            return retBuffer;

        }


        public string GetSubstringByString(string a, string b, string c)
        {
            return c.Substring((c.IndexOf(a) + a.Length), (c.IndexOf(b) - c.IndexOf(a) - a.Length));
        }

        /// <summary>
        /// Post a comment to a user's profile.
        /// </summary>
        /// <param name="comment">The comment message.</param>
        /// <param name="user">The user to post to.</param>
        /// <returns>True if comment posting succeeded, false otherwise.</returns>
        public static bool PostComment(string comment, string sID, string Session)
        {
            NameValueCollection data = new NameValueCollection();
            data.Add("comment", comment);
            data.Add("count", "1"); //# of comments to return, 1 = fastest
            data.Add("sessionid", Session);
            return DoCommentAction("post", sID, data, sID);
        }

        private static bool DoCommentAction(string action, string user, NameValueCollection data, string SessionID)
        {
            try
            {
                String URL = String.Format("http://steamcommunity.com/comment/Profile/{0}/{1}/-1", action, user);
                string resp = Fetch(URL, "POST", data, CookC);
                //ActionResponse response = JsonConvert.DeserializeObject<ActionResponse>(resp);
                return resp.ToLower().Contains("true");//response.success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string Fetch(string url, string method, NameValueCollection data = null, CookieContainer cookies = null, bool ajax = true)
        {
            HttpWebResponse response = Request(url, method, data, cookies, ajax);
            using (Stream responseStream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static HttpWebResponse Request(string url, string method, NameValueCollection data = null, CookieContainer cookies = null, bool ajax = true, string referer = "")
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = method;
            request.Accept = "text/javascript, text/html, application/xml, text/xml, */*";
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            //request.Host is set automatically
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:33.0) Gecko/20100101 Firefox/33.0";
            request.Referer = string.IsNullOrEmpty(referer) ? "http://steamcommunity.com/id/bobthestroodle" : referer;
            request.Timeout = 50000; //Timeout after 50 seconds

            if (ajax)
            {
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");
                request.Headers.Add("X-Prototype-Version", "1.7");
            }

            // Cookies
            request.CookieContainer = cookies ?? new CookieContainer();

            // Request data
            if (data != null)
            {
                string dataString = String.Join("&", Array.ConvertAll(data.AllKeys, key =>
                    String.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(data[key]))
                ));

                byte[] dataBytes = Encoding.UTF8.GetBytes(dataString);
                request.ContentLength = dataBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(dataBytes, 0, dataBytes.Length);
                }
            }

            // Get the response
            return request.GetResponse() as HttpWebResponse;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public String parseSpintax(Random rand, String s)
        {
            if (s.Contains("{"))
            {
                int closingBracePosition = s.IndexOf('}');
                int openingBracePosition = closingBracePosition;
                while (!s[openingBracePosition].Equals('{')) openingBracePosition--;
                String spintaxBlock = s.Substring(openingBracePosition, closingBracePosition - openingBracePosition + 1);
                String[] items = spintaxBlock.Substring(1, spintaxBlock.Length - 2).Split('|');
                s = s.Replace(spintaxBlock, items[rand.Next(items.Length)]);
                return parseSpintax(rand, s);
            }
            else { return s; }
        }

    }
}
