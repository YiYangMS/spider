using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReadXml
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            string path = @"d:\sinanews2.xml";
            //string path = args[0];
            doc.Load(path);
            const String connString = @"Data Source=(localdb)\Projects;Initial Catalog=testxml;Integrated Security=True";
            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlCommand testCommand = conn.CreateCommand();
            string SQL = "";
            try
            {
                XmlNode xnd = doc.SelectSingleNode("//channel");
                XmlNodeList xnl = xnd.SelectNodes("item");
                foreach (XmlNode i in xnl)
                {
                    XmlNode j = i.SelectSingleNode("title");
                    string Title = j.InnerText;
                    j = i.SelectSingleNode("description");
                    string Description = j.InnerText;
                    j = i.SelectSingleNode("pubDate");
                    string PubDate = j.InnerText;
                    j = i.SelectSingleNode("guid");
                    string Guid = j.InnerText;
                    SQL = string.Format(@"select title from data where title =  N'{0}'", Title);
                    testCommand.CommandText = SQL;
                    object obj = testCommand.ExecuteScalar();
                    if (obj == null)
                    {
                        SQL = string.Format(@"insert into data(title, guid, description, pubDate) values(N'{0}','{1}',N'{2}','{3}')", Title, Guid, Description, PubDate);
                        testCommand.CommandText = SQL;
                        testCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {
                string str = e.Message.ToString();
                Console.WriteLine(str);
            }
            conn.Close();
            Console.WriteLine("finish!");
            Console.ReadLine();
        }
    }
}


//http://rss.sina.com.cn/news/marquee/ddt.xml 
//http://news.163.com/special/00011K6L/rss_newstop.xml

//Mon, 21 Jul 2014 02:41:21 GMT