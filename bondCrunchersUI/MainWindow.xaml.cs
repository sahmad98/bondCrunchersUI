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

        private void BookTrade(object sender, RoutedEventArgs e)
        {
            WebClient web = new WebClient();
            string uri = "http://192.168.66.53:8080/EBondTraderWeb/rest/EBond";
            web.BaseAddress = uri;
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(TestClass));
            TestClass t1 = (TestClass)serializer.ReadObject(web.OpenRead(uri));
            bondList.Items.Add(t1);
        }
    }
}
