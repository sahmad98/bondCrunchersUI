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
        static string IP = bondCrunchersUI.MainWindow.IP;
        string transactionLogURI = IP+"/EBondTraderWeb/rest/bond/trans";
        //string bondDataURI = IP+ "/EBondTraderWeb/rest/product?isin=";
        WebClient web = new WebClient();
        Bond selectedBond = null;

        private void SetupTradeBooking(object sender, RoutedEventArgs e)
        {
            foreach (Bond temp in bondCrunchersUI.MainWindow.bondList)
            {
                cmbISIN.Items.Add(temp.isin);              
            }
            cmbISIN.SelectedItem = bondCrunchersUI.MainWindow.selectedBond;
            //string response = web.DownloadString(bondDataURI+cmbISIN.SelectedItem);
            //selectedBond = (Bond)jsonSerializer.Deserialize(response, typeof(Bond));
            selectedBond = bondCrunchersUI.MainWindow.bondList.Find(x => x.isin == (string)cmbISIN.SelectedItem);
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

        //Populate the Trade Information in Transaction object
        private void getData(Transaction newTransaction)
        {
            newTransaction.isin = (string)cmbISIN.SelectedItem;
            newTransaction.settlementDate = (long)(((DateTime)dtpTrade.SelectedDate).AddDays(2) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.timeStamp = (long)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.tradeDate = (long)((DateTime)dtpTrade.SelectedDate - new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
            newTransaction.settlementAmount = decimal.Parse(settlementPrice.ToString());
            newTransaction.tradeYield = decimal.Parse(txtTradeYield.Text);
            newTransaction.accruedAmount = decimal.Parse(accruedInterest.ToString());
            newTransaction.cleanPrice = decimal.Parse(cleanPrice.ToString());
            newTransaction.dirtyPrice = decimal.Parse(dirtyPrice.ToString());
            newTransaction.quantity = int.Parse(txtQuantity.Text);
       }

        private void EnableFields(object sender, TextChangedEventArgs e)
        {
            try
            {

                txtCleanPrice.IsEnabled = true;
                txtDirtyPrice.IsEnabled = true;
                if (selectedBond != null)
                {
                    CalculateCleanPrice();
                    CalculateDirtyPrice();
                }
                else
                {
                    txtCleanPrice.Text = "NULL";
                }
            }
            catch (FormatException)
            {
                txtCleanPrice.IsEnabled = false;
                txtDirtyPrice.IsEnabled = false;
                txtDirtyPrice.Text = "Please enter a numeric in trade";
                txtCleanPrice.Text = "Please enter a numeric in trade";
                //MessageBox.Show(fe+"");
            }

        }
        decimal faceValue = 100;
        double cleanPrice = 0;
        double dirtyPrice = 0;
        decimal accruedInterest = 0;
        double settlementPrice = 0;
        private void CalculateCleanPrice()
        {
            //Corrected
            //faceValue = (selectedBond.currentYield * selectedBond.lastPrice) / selectedBond.couponRate;
            int frequency = 1;
            if (selectedBond.couponPeriod == "Semi-Annual")
                frequency = 2;
            else if (selectedBond.couponPeriod == "Quaterly")
                frequency = 4;

            decimal yield = decimal.Parse(txtTradeYield.Text) / (100*frequency);
            int numberOfYears = selectedBond.Maturity.Year - ((DateTime)(dtpTrade.SelectedDate)).Year;
            double numerator = 1 - Math.Pow(1 + double.Parse(yield.ToString()), -numberOfYears*frequency);
            double denominator = double.Parse(yield.ToString());
            double extraFactor = double.Parse(faceValue.ToString()) * Math.Pow(1 + double.Parse(yield.ToString()), -numberOfYears*frequency);
            double presentValue = double.Parse((faceValue*selectedBond.couponRate/(100*frequency)).ToString())*(numerator/denominator);
            cleanPrice = presentValue + extraFactor;
            txtCleanPrice.Text = String.Format("{0:C}" ,cleanPrice);
         }

        private void ChangeBond(object sender, EventArgs e)
        {
            try
            {
                selectedBond = bondCrunchersUI.MainWindow.bondList.Find(x => x.isin == cmbISIN.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("Error Occured: Bond list empty.");
            }
       }

        private void CalculateDirtyPrice()
        {
            if (selectedBond != null)
            {
                DateTime lastCoupon = new DateTime(selectedBond.Start.Ticks);
                while (DateTime.Compare(lastCoupon, (DateTime)dtpTrade.SelectedDate) <= 0)
                {
                    if (selectedBond.couponPeriod == "Annual")
                        lastCoupon = lastCoupon.AddYears(1);
                    else if (selectedBond.couponPeriod == "Semi-Annual")
                        lastCoupon = lastCoupon.AddMonths(6);
                    else if (selectedBond.couponPeriod == "Quaterly")
                        lastCoupon = lastCoupon.AddMonths(3);
                }
                if (selectedBond.couponPeriod == "Annual")
                {
                    lastCoupon = lastCoupon.AddYears(-1);
                }
                else if (selectedBond.couponPeriod == "Semi-Annual")
                {
                    lastCoupon = lastCoupon.AddMonths(-6);
                }
                else if (selectedBond.couponPeriod == "Quaterly")
                {
                    lastCoupon = lastCoupon.AddMonths(-3);
                }
                //MessageBox.Show(lastCoupon.ToShortDateString());
                int numberOfDaysAccrued = (((DateTime)dtpTrade.SelectedDate).Subtract(lastCoupon)).Days;
                //MessageBox.Show(numberOfDaysAccrued+"");
                accruedInterest = (decimal.Parse((numberOfDaysAccrued / 360.0).ToString()) * selectedBond.couponRate * faceValue)/100;
                //MessageBox.Show(accruedInterest + "");
                dirtyPrice = cleanPrice + double.Parse(accruedInterest.ToString());
                txtDirtyPrice.Text = String.Format("{0:C}", dirtyPrice);
                txtAccruedAmount.Text = String.Format("{0:C}", accruedInterest);
          }
        }

        private void QuantityChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                settlementPrice = double.Parse(dirtyPrice.ToString()) * int.Parse(txtQuantity.Text);
                txtSettlemetAmount.Text = String.Format("{0:C}", settlementPrice);
            }
            catch (FormatException)
            {
                txtSettlemetAmount.Text = "Enter Integer Quantity";
            }
        }
    }
}
