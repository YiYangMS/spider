using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using getMaintext;
using System.Data.SqlClient;

namespace getMainByCom
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("usage: getmain[config's path]");
                return;
            }
            string str = Maintext.getNodeList(args[0]);
            if (str != string.Empty)
            {
                Console.WriteLine(str);
                return;
            }
            //string path = "";
            string url = "";
            string title = "";
            try
            {
                const String connString = @"Data Source=(localdb)\Projects;Initial Catalog=testxml;Integrated Security=True";
                SqlConnection conn = new SqlConnection(connString);
                conn.Open();
                SqlCommand testCommand = conn.CreateCommand();
                string SQL = @"select title,guid,main from data";
                testCommand.CommandText = SQL;
                SqlDataReader reader = testCommand.ExecuteReader();
                List<string> urllist = new List<string>();
                List<string> titlelist = new List<string>();
                while (reader.Read())
                {
                    if (string.Format("{0}", reader[2]) == string.Empty)
                    {
                        urllist.Add(string.Format("{0}", reader[1]));
                        titlelist.Add(string.Format("{0}", reader[0]));
                    }
                }
                reader.Close();
                for (int i = 0; i < urllist.Count; i++)
                {
                    url = urllist[i];
                    title = titlelist[i];
                    string main = Maintext.getData(url);
                    if (main.Length > 2 && main[0] == 'E' && main[1] == ':')
                    {
                        Console.WriteLine(title);
                        Console.WriteLine(main.Substring(2));
                        continue;
                    }
                    if (main == "") Console.WriteLine(title);
                }
                conn.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
            Console.WriteLine("finish!");
        }
    }
}
