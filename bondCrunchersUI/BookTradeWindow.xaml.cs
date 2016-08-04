using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Web.Script.Serialization;
namespace bondCrunchersUI
{
    /// <summary>
    /// Interaction logic for BookTradeWindow.xaml
    /// </summary>
    public partial class BookTradeWindow : Window
    {
        public BookTradeWindow()
        {
            InitializeComponent();           
        }

        JavaScriptSerializer jsonSerializer = new JavaScriptSerializer();
        string transactionLogURI = "http://192.168.66.1:8080/EBondTraderWeb/rest/product/trans";
        WebClient web = new WebClient();

        private void SetupTradeBooking(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            foreach (Bond temp in bondCrunchersUI.MainWindow.bondList)
            {
                cmbISIN.Items.Add(temp.isin);
                
            }
            cmbISIN.SelectedItem = bondCrunchersUI.MainWindow.selectedBond;
            txtSettlemetAmount.Text = "123";
            txtAccruedAmount.Text = "12.33";
        }

        private void BookTrade(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Transaction newTransaction = new Transaction();
            getData(newTransaction);
            try
            {
                string json = jsonSerializer.Serialize(newTransaction);
                web.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                web.UploadString(transactionLogURI, "POST", json);
            }
            catch (Exception ie)
            {
                MessageBox.Show("Error Occured. " + ie);
            }
        }

        private void CancelTrade(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void getData(Transaction newTransaction)
        {
            newTransaction.isin = (string)cmbISIN.SelectedItem;
            //newTransaction.timeStamp = (long)(TimeZoneInfo.ConvertTimeToUtc(DateTime.Now) - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
            //newTransaction.settlementDate = (long)(TimeZoneInfo.ConvertTimeToUtc((DateTime)dtpSettlement.SelectedDate) - new DateTime(1970,1,1,0,0,0,0, System.DateTimeKind.Utc)).TotalSeconds;
            //MessageBox.Show(newTransaction.timeStamp+"");
            newTransaction.settlementDate = (long)((DateTime)dtpSettlement.SelectedDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.timeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.timeStamp = (long)((DateTime)dtpTrade.SelectedDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.settlementAmount = decimal.Parse(txtSettlemetAmount.Text);
            newTransaction.tradeYield = decimal.Parse(txtTradeYield.Text);
            newTransaction.accruedAmount = decimal.Parse(txtAccruedAmount.Text);
            newTransaction.cleanPrice = decimal.Parse(txtCleanPrice.Text);
            newTransaction.dirtyPrice = decimal.Parse(txtDirtyPrice.Text);
       }

        private void EnableFields(object sender, TextChangedEventArgs e)
        {
            try
            {
                float.Parse(txtTradeYield.Text);
                txtCleanPrice.IsEnabled = true;
                txtDirtyPrice.IsEnabled = true;
                txtCleanPrice.Text = "";
                txtDirtyPrice.Text = "";
            }
            catch (FormatException fe)
            {
                txtCleanPrice.IsEnabled = false;
                txtDirtyPrice.IsEnabled = false;
                txtDirtyPrice.Text = "Please enter a numeric in trade";
                txtCleanPrice.Text = "Please enter a numeric in trade";
            }
        }
    }
}
