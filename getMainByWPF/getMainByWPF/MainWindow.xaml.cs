using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using getMaintext;
using System.Data.SqlClient;

namespace getMainByWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void choosefile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.InitialDirectory = "d:\\";
            fileDialog.Title = "选择文件";
            fileDialog.Filter = "xml files (*.xml)|*.xml";
            fileDialog.FilterIndex = 1;
            fileDialog.RestoreDirectory = true;
            if (fileDialog.ShowDialog() == true)
            {
                operateList.Items.Clear();
                String fileName = fileDialog.FileName;
                string str = Maintext.getNodeList(fileName);
                if (str != string.Empty)
                {
                    operateList.Items.Add(str);
                    return;
                }
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
                        //if (string.Format("{0}", reader[2]) == string.Empty)
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
                            operateList.Items.Add(title);
                            operateList.Items.Add(main.Substring(2));
                            continue;
                        }
                        if (main == "") operateList.Items.Add(title);
                    }
                    conn.Close();
                }
                catch (Exception ex)
                {
                    operateList.Items.Add(ex.Message.ToString());
                }
                operateList.Items.Add("finish!");
            }
            else
            {

            }
        }
    }
}


// DisplayMemberBinding="{Binding Path=main}"