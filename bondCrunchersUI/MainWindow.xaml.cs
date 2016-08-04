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
using System.Windows.Threading;

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

        public static string IP = "";
        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        string restURI = ""; // IP + "/EBondTraderWeb/rest/bond";
        string transactionURI = ""; // IP+"/EBondTraderWeb/rest/bond/transhis";
        string searchURI = ""; // IP+"/EBondTraderWeb/rest/bond/allBondsBy";
        string customerSearchURI = ""; // IP + "/EBondTraderWeb/rest/bond/allBondsByCustomSearch";
        
        public static List<Bond> bondList = null;
        public static string selectedBond = "";
        WebClient webClient = new WebClient();
        
        private void LoadBonds(object sender, RoutedEventArgs e)
        {
            try
            {
                bondList = GetJsonObjects(restURI);
                bondData.Items.Clear();
                foreach (Bond bond in bondList)
                {
                    bond.ConvertDates();
                    bondData.Items.Add(bond);
                }
            }
            catch (Exception)
            {

            }
        }

        private List<Bond> GetJsonObjects(string URI)
        {
            List<Bond> resultList = null;
            try
            {
                webClient.Proxy = null;
                string json = webClient.DownloadString(restURI);
                resultList = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            }
            catch (Exception ie)
            {
                MessageBox.Show(ie + "");
            }
            return resultList;
        }

        private void BookTrade(object sender, RoutedEventArgs e)
        {
            BookTradeWindow tradeWindow = new BookTradeWindow();
            bool? success = tradeWindow.ShowDialog();
            if (success == true)
                statusLabel.Content = "Status: Last trade added to queue";
            else
                statusLabel.Content = "Status: Last trade cancelled";            
        }

        private void ChangeSelection(object sender, SelectionChangedEventArgs e)
        {
            if(!bondData.Items.IsEmpty)
                selectedBond = ((Bond)bondData.SelectedItem).isin;
        }

        private void RefreshTransaction(object sender, RoutedEventArgs e)
        {
            transactionHistory.Items.Clear();
            try
            {
                string json = webClient.DownloadString(transactionURI);
                List<TransactionLog> transactionList = (List<TransactionLog>)jsonSerializer.Deserialize(json, typeof(List<TransactionLog>));
                //List<TransactionLog> transactionList = GetJsonObjects(transactionURI);
                foreach (TransactionLog transaction in transactionList)
                {
                    transaction.ConvertDates();
                    transactionHistory.Items.Add(transaction);
                }
            }
            catch (Exception)
            {

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
            string json = "";
            string search = GetSearchURI();
            json = webClient.DownloadString(customerSearchURI + search);
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
            string search = GetSearchURI();
            string json = webClient.DownloadString(customerSearchURI +  search);
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
            string search = GetSearchURI();
            string json = webClient.DownloadString(customerSearchURI + search);
            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void TestConnection(object sender, RoutedEventArgs e)
        {
            try
            {
                if (webClient.DownloadString(restURI + "/test") != "Connection Done")
                    throw new Exception();
                statusLabel.Content = "Status: Connection successful";
                //MessageBox.Show("Connection Successful.");
            }
            catch(Exception)
            {
                statusLabel.Content = "Status: Connection failed.";
                //MessageBox.Show("Connection failed.");
            }
        }

        private void IPChanged(object sender, TextChangedEventArgs e)
        {
            IP = "http://"+txtIP.Text;
            restURI = IP + "/EBondTraderWeb/rest/bond";
            transactionURI = IP + "/EBondTraderWeb/rest/bond/transhis";
            searchURI = IP + "/EBondTraderWeb/rest/bond/allBondsBy";
            customerSearchURI = IP + "/EBondTraderWeb/rest/bond/allBondsByCustomSearch";
            statusLabel.Content = "Status: Connection not tested";
        }

        private void ClearBonds(object sender, RoutedEventArgs e)
        {
            if (bondList != null)
            {
                bondList.Clear();
                bondData.Items.Clear();
            }
        }

        private string GetSearchURI()
        {
            string search = "?";
            if(txtSearch.Text != "")
                search += ("isin="+txtSearch.Text);
            if (cmbCoupon.SelectedIndex != -1)
            {
                search += ("&couponPeriod="+cmbCoupon.Text);
            }
            if (txtIssuerSearch.Text != "")
            {
                search += ("&issuerName="+txtIssuerSearch.Text);
            }
            if (cmbRating.SelectedIndex != -1)
            {
                if (cmbRatingSymbol.SelectedIndex != -1)
                {
                    if (cmbRating.Text == "Moody's")
                        search += ("&moodys=" + cmbRatingSymbol.Text);
                    else if (cmbRating.Text == "SnP")
                        search += ("&snp=" + cmbRatingSymbol.Text);
                    else if (cmbRating.Text == "Fitch")
                        search += ("&fitch="+cmbRatingSymbol.Text);
                }
            }
            if (cmbYield.SelectedIndex != -1)
            {
                if(cmbYield.Text == "0-1%")
                    search += ("&currentYield=0");
                if(cmbYield.Text == "1-2%")
                    search += ("&currentYield=1");
                if(cmbYield.Text == "2-3%")
                    search += ("&currentYield=2");
                if (cmbYield.Text == "3-4%")
                    search += ("&currentYield=3");
                if (cmbYield.Text == "4-5%")
                    search += ("&currentYield=4");
                if (cmbYield.Text == "5-6%")
                    search += ("&currentYield=5");
                if (cmbYield.Text == "6-7%")
                    search += ("&currentYield=7");
            }
            search = search.Replace("+", "%2B");
            return search;
        }

        private void PopulateRatings(object sender, EventArgs e)
        {
            if (cmbRating.SelectedIndex != -1)
            {
                if (cmbRating.Text == "Moody's")
                    PopulateMoody();
                else if (cmbRating.Text == "SnP")
                    PopulateSnP();
                else if (cmbRating.Text == "Fitch")
                    PopulateFitch();
            }
        }

        private void PopulateMoody()
        {
            cmbRatingSymbol.Items.Clear();
            cmbRatingSymbol.Items.Add("A1");
            cmbRatingSymbol.Items.Add("A2");
            cmbRatingSymbol.Items.Add("A3");
            cmbRatingSymbol.Items.Add("Aa1");
            cmbRatingSymbol.Items.Add("Aa2");
            cmbRatingSymbol.Items.Add("Aa3");
            cmbRatingSymbol.Items.Add("Aaa");
            cmbRatingSymbol.Items.Add("B1");
            cmbRatingSymbol.Items.Add("B2");
            cmbRatingSymbol.Items.Add("B3");
            cmbRatingSymbol.Items.Add("Ba1");
            cmbRatingSymbol.Items.Add("Ba2");
            cmbRatingSymbol.Items.Add("Ba3");
            cmbRatingSymbol.Items.Add("Baa1");
            cmbRatingSymbol.Items.Add("Baa2");
            cmbRatingSymbol.Items.Add("Baa3");
            cmbRatingSymbol.Items.Add("Ca");
            cmbRatingSymbol.Items.Add("Caa1");
            cmbRatingSymbol.Items.Add("Caa2");
            cmbRatingSymbol.Items.Add("Caa3");
            cmbRatingSymbol.Items.Add("WR");
        }

        private void PopulateSnP()
        {
            cmbRatingSymbol.Items.Clear();
            cmbRatingSymbol.Items.Add("AAA");
            cmbRatingSymbol.Items.Add("AA+");
            cmbRatingSymbol.Items.Add("AA");
            cmbRatingSymbol.Items.Add("AA-");
            cmbRatingSymbol.Items.Add("A");
            cmbRatingSymbol.Items.Add("A-");
            cmbRatingSymbol.Items.Add("BBB+");
            cmbRatingSymbol.Items.Add("BBB");
            cmbRatingSymbol.Items.Add("BBB-");
            cmbRatingSymbol.Items.Add("BB+");
            cmbRatingSymbol.Items.Add("BB");
            cmbRatingSymbol.Items.Add("BB-");
            cmbRatingSymbol.Items.Add("B+");
            cmbRatingSymbol.Items.Add("B");
            cmbRatingSymbol.Items.Add("B-");
            cmbRatingSymbol.Items.Add("CCC+");
            cmbRatingSymbol.Items.Add("CCC");
            cmbRatingSymbol.Items.Add("CCC-");
            cmbRatingSymbol.Items.Add("D");
        }

        private void PopulateFitch()
        {
            cmbRatingSymbol.Items.Clear();
            cmbRatingSymbol.Items.Add("AA+");
            cmbRatingSymbol.Items.Add("AA");
            cmbRatingSymbol.Items.Add("AA-");
            cmbRatingSymbol.Items.Add("A+");
            cmbRatingSymbol.Items.Add("A");
            cmbRatingSymbol.Items.Add("A-");
            cmbRatingSymbol.Items.Add("BBB+");
            cmbRatingSymbol.Items.Add("BBB");
            cmbRatingSymbol.Items.Add("BBB-");
            cmbRatingSymbol.Items.Add("BB+");
            cmbRatingSymbol.Items.Add("BB");
            cmbRatingSymbol.Items.Add("B+");
            cmbRatingSymbol.Items.Add("B");
            cmbRatingSymbol.Items.Add("B-");
            cmbRatingSymbol.Items.Add("CCC+");
            cmbRatingSymbol.Items.Add("CC");
            cmbRatingSymbol.Items.Add("WD");

        }

        private void SearchRatingChanged(object sender, EventArgs e)
        {
            string search = GetSearchURI();
            //MessageBox.Show(search);
            string json = webClient.DownloadString(customerSearchURI + search);
            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }

        private void DoubleClickTrade(object sender, MouseButtonEventArgs e)
        {
            BookTrade(sender, new RoutedEventArgs());
        }

        private void YieldChanged(object sender, EventArgs e)
        {
            string search = GetSearchURI();
            //MessageBox.Show(search);
            string json = webClient.DownloadString(customerSearchURI + search);
            List<Bond> searchResult = (List<Bond>)jsonSerializer.Deserialize(json, typeof(List<Bond>));
            bondData.Items.Clear();
            foreach (Bond temp in searchResult)
            {
                AddToGrid(temp);
            }
        }


        private void CancelOrder(object sender, RoutedEventArgs e)
        {
            if (transactionHistory.SelectedIndex != -1)
            {
                Transaction selectedTransaction = (Transaction)transactionHistory.SelectedItem;
                webClient.Headers.Clear();
                webClient.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string json = jsonSerializer.Serialize(selectedTransaction);
                //MessageBox.Show(json);
                webClient.UploadString(restURI + "/cancelOrder", "PUT", json);
            }
            RefreshTransaction(sender, e);
        }
        private void Setup(object sender, RoutedEventArgs e)
        {

        }

        private void SearchTransactionISIN(object sender, TextChangedEventArgs e)
        {
            string search = GetTransactionSearchURI();
            try
            {
                string json = webClient.DownloadString(restURI + "/transactionSearch"+search);
                List<TransactionLog> transactionList = (List<TransactionLog>)jsonSerializer.Deserialize(json, typeof(List<TransactionLog>));
                //List<TransactionLog> transactionList = GetJsonObjects(transactionURI);
                transactionHistory.Items.Clear();
                foreach (TransactionLog transaction in transactionList)
                {
                    transaction.ConvertDates();
                    transactionHistory.Items.Add(transaction);
                }
            }
            catch (Exception)
            {

            }
        }

        private string GetTransactionSearchURI()
        {
            string search = "?";
            if (txtTransISIN.Text != "")
            {
                search += ("isin="+txtTransISIN.Text);
            }
            if (txtCustomerID.Text != "")
            {
                search += ("&customerId="+txtCustomerID.Text);
            }
            return search;
        }

        private void SearchTransactionCustomerID(object sender, TextChangedEventArgs e)
        {
            SearchTransactionISIN(sender, e);
        }

        private void ExportBond(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter("data.csv");
                if (bondList != null)
                {
                    sw.WriteLine("id, isin, issuerName, maturityDate, startDate, bondDuration, couponRate, couponPeriod, high, low, last, change, yield, pieceSize, fitch, moody's, snp");
                    foreach (var bond in bondList)
                    {
                        sw.WriteLine(bond.id + ", " + bond.isin + ", " + bond.issuerName + ", " + bond.Maturity.ToShortDateString() + ", "
                            + bond.Start.ToShortDateString() + ", " + bond.bondDuration + ", " + bond.couponRate + ", " + bond.couponPeriod +
                            ", " + bond.high + ", " + bond.low + ", " + bond.lastPrice + ", " + bond.changePrice + ", " +
                            bond.currentYield + ", " + bond.pieceSize + ", " + bond.fitch + ", " + bond.moodys + ", " + bond.snP);
                    }
                }
                statusLabel.Content = "Status: Data exported to data.csv";
            }
            catch (Exception)
            {
                statusLabel.Content = "Status: Writing to CSV file failed.";
            }
            finally {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        private void ExportTransaction(object sender, RoutedEventArgs e)
        {
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter("transaction_history.csv");
                if (!transactionHistory.Items.IsEmpty)
                {
                    sw.WriteLine("orderID, timeStamp, customerID, isin, tradeDate, settlementDate, cleanPrice, dirtyPrice, tradeYield, quantity, accruedAmount, settlementAmount, status");
                    foreach (TransactionLog transaction in transactionHistory.Items)
                    {
                        sw.WriteLine(transaction.orderId + ", " + transaction.timeStampLog.ToShortDateString() + ", " + transaction.customerId + ", " + transaction.isin + ", "
                            + transaction.tradeDateShort + ", " + transaction.settlementDateShort + ", " + transaction.cleanPrice + ", " + transaction.dirtyPrice +
                            ", " + transaction.tradeYield + ", " + transaction.quantity + ", " + transaction.accruedAmount + ", " + transaction.settlementAmount + ", " +
                            transaction.status);
                    }
                }
                statusLabel.Content = "Status: Data exported to transaction_history.csv";
            }
            catch (Exception)
            {
                statusLabel.Content = "Status: Writing to CSV file failed.";
            }
            finally
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
    }
}
