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
using System.Net;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;

namespace bondCrunchersUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        string restURI = "http://192.168.66.1:8080/EBondTraderWeb/rest/product";
        public static List<Bond> bondList = null;
        public static string selectedBond = "";
        private void Setup(object sender, RoutedEventArgs e)
        {
            
        }

        private void LoadBonds(object sender, RoutedEventArgs e)
        {
            try
            {
                WebClient webClient = new WebClient();
                string json = webClient.DownloadString(restURI);
                bondList = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
                foreach (Bond temp in bondList)
                {
                    bondData.Items.Add(temp);
                }
            }
            catch(Exception ie)
            {
                MessageBox.Show(ie + "");
            }
        }

        private void BookTrade(object sender, RoutedEventArgs e)
        {
            BookTradeWindow tradeWindow = new BookTradeWindow();
            tradeWindow.ShowDialog();
            
        }

        private void ChangeSelection(object sender, SelectionChangedEventArgs e)
        {
            selectedBond = ((Bond)bondData.SelectedItem).isin;
        }
    }
}
