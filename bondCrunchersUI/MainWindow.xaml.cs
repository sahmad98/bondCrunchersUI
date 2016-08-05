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
        public static string IP = "http://192.168.66.1:8080";
        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        string restURI = IP+"/EBondTraderWeb/rest/bond";
        string transactionURI = IP+"/EBondTraderWeb/rest/bond/transhis";
        string searchURI = IP+"/EBondTraderWeb/rest/bond/allBondsBy";
        string customerSearchURI = IP + "/EBondTraderWeb/rest/bond/allBondsByCustomSearch";

        public static List<Bond> bondList = null;
        public static string selectedBond = "";
        WebClient webClient = new WebClient();

        private void Setup(object sender, RoutedEventArgs e)
        {
            
        }

        private void LoadBonds(object sender, RoutedEventArgs e)
        {
            try
            {
                string json = webClient.DownloadString(restURI);
                bondList = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
                foreach (Bond temp in bondList)
                {
                    temp.ConvertDates();
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
            if(!bondData.Items.IsEmpty)
                selectedBond = ((Bond)bondData.SelectedItem).isin;
        }

        private void RefreshTransaction(object sender, RoutedEventArgs e)
        {
            string json = webClient.DownloadString(transactionURI);
            List<TransactionLog> transactionList = (List<TransactionLog>)jsonSerializer.Deserialize(json, typeof(List<TransactionLog>));
            foreach (TransactionLog temp in transactionList)
            {
                temp.ConvertDates();
                transactionHistory.Items.Add(temp);
            }
        }

        private void Search(object sender, RoutedEventArgs e)
        {
            List<Bond> searchResult = bondList.FindAll(x => x.isin.StartsWith(txtSearch.Text));
            bondData.Items.Clear();
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void SearchChanged(object sender, TextChangedEventArgs e)
        {
            /*
            List<Bond> searchResult = bondList.FindAll(x => x.isin.StartsWith(txtSearch.Text));
            */
            //webClient.Headers.Add(HttpRequestHeader.ContentType, "text/plain");
            string json = "";
            if (cmbCoupon.Text == "Any" || cmbCoupon.Text == "")
                json = webClient.DownloadString(customerSearchURI + "?isin=" + txtSearch.Text);
            else
                json = webClient.DownloadString(customerSearchURI + "?isin=" + txtSearch.Text + "&coupon_Period="+cmbCoupon.Text);

            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            bondList = searchResult;
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void CouponPeriodChanged(object sender, EventArgs e)
        {
            string search = null;
            if (cmbCoupon.Text == "Any")
                search = null;
            else
                search = cmbCoupon.Text;
            string json = webClient.DownloadString(searchURI + "CouponPeriod?coupon_Period=" + search);
            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            bondList = searchResult;
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void AddToGrid(Bond temp)
        {
            temp.ConvertDates();
            bondData.Items.Add(temp);
        }

        private void SearchIssuer(object sender, TextChangedEventArgs e)
        {
            string json = webClient.DownloadString(searchURI + "IssuerName?issuerName=" + txtIssuerSearch.Text);
            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void TestConnection(object sender, RoutedEventArgs e)
        {

        }
    }
}
