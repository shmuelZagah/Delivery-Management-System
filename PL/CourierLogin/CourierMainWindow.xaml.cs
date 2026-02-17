using BO;
using PL.Courier;
using PL.Helpers;
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
using System.Windows.Shapes;
using System.Xml;

namespace PL.CourierLogin
{
    /// <summary>
    /// Interaction logic for CourierMainWindow.xaml
    /// </summary>
    public partial class CourierMainWindow : Window
    {
        BlApi.IBl s_bl = BlApi.Factory.Get();


        //---------------
        #region Variables
        //---------------
        //ViewModel properties
        public BO.Courier CurrentCourier
        {
            get { return (BO.Courier)GetValue(CurrentCourierProperty); }
            set { SetValue(CurrentCourierProperty, value); }
        }

        public static readonly DependencyProperty CurrentCourierProperty =
            DependencyProperty.Register("CurrentCourier", typeof(BO.Courier), typeof(CourierMainWindow), new PropertyMetadata(null));


        public BO.OpenOrderInList SelectedOrder
        {
            get { return (BO.OpenOrderInList)GetValue(SelectedOrderProperty); }
            set { SetValue(SelectedOrderProperty, value); }
        }

        public static readonly DependencyProperty SelectedOrderProperty =
            DependencyProperty.Register("SelectedOrder", typeof(BO.OpenOrderInList), typeof(CourierMainWindow), new PropertyMetadata(null));

        #endregion
        //---------------
        public CourierMainWindow()
        {

            //made the courier a default data
            this.DataContext = this;

            InitializeComponent();

        }

        //---------------
        #region Buttons
        //---------------

        private void UpdateDetails_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            try
            {
                s_bl.Courier.UpdateCourier(Helpers.Tools.UserId, CurrentCourier);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void FinishDelivery_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            IsLoadInProgress = true;

            try
            {
                if (CurrentCourier?.InProgress == null) return;
                s_bl.Order.CompleteOrderHandling(Helpers.Tools.UserId, CurrentCourier.Id, CurrentCourier.InProgress.DeliveryId);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsLoadInProgress = false;
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void CustomerUnavailable_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            IsLoadInProgress = true;

            try
            {
                if (CurrentCourier?.InProgress == null) return;
                s_bl.Order.DeliveryEnded(Helpers.Tools.UserId, CurrentCourier.InProgress.OrderId, BO.DeliveryEndType.ClientNotFound);
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message);
                IsLoadInProgress = false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        private void DeliveryRefused_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            IsLoadInProgress = true;

            try
            {
                if (CurrentCourier?.InProgress == null) return;
                s_bl.Order.DeliveryEnded(Helpers.Tools.UserId, CurrentCourier.InProgress.OrderId, BO.DeliveryEndType.ClientRefusedAccept);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsLoadInProgress = false;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void SelectOrder_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            if (SelectedOrder == null)
            {
                MessageBox.Show("Please select an order to handle.");
                Mouse.OverrideCursor = null;
                return;
            }
            BO.OpenOrderInList selectedOrder = (BO.OpenOrderInList)SelectedOrder;
            try
            {
                s_bl.Order.ChooseOrderForHandling(Helpers.Tools.UserId, CurrentCourier.Id, selectedOrder.OrderId, selectedOrder.ActualDistance);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void doubleClick_OrderList(object sender, MouseButtonEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                SelectOrder_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                s_bl.Order.RemoveObserver(OrderListObserver);
                s_bl.Courier.RemoveObserver(OrderListObserver);

                new LoginWindow().Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally { Mouse.OverrideCursor = null; }
        }


        //---------------
        #endregion
        //---------------



        public IEnumerable<BO.OpenOrderInList> OpenOrderList
        {
            get { return (IEnumerable<BO.OpenOrderInList>)GetValue(OpenOrderListProperty); }
            set { SetValue(OpenOrderListProperty, value); }
        }

        public static readonly DependencyProperty OpenOrderListProperty =
            DependencyProperty.Register("OpenOrderList", typeof(IEnumerable<BO.OpenOrderInList>), typeof(CourierMainWindow), new PropertyMetadata(null));



        public IEnumerable<BO.ClosedDeliveryInList> CloseOrderList
        {
            get { return (IEnumerable<BO.ClosedDeliveryInList>)GetValue(CloseOrderListProperty); }
            set { SetValue(CloseOrderListProperty, value); }
        }

        public static readonly DependencyProperty CloseOrderListProperty =
            DependencyProperty.Register("CloseOrderList", typeof(IEnumerable<BO.ClosedDeliveryInList>), typeof(CourierMainWindow), new PropertyMetadata(null));



        //-------------------------------
        #region Helpers
        //-------------------------------

        private async Task queryOrderLists()
        {
            try
            {
                var openTask = GetOpenOrders();
                var closedTask = Task.Run(() => GetDeliveryInList().ToList()); 
                var courierTask = s_bl.Courier.GetCourierDetails(Helpers.Tools.UserId, Helpers.Tools.UserId);

                OpenOrderList = await openTask;
                CloseOrderList = await closedTask;
                CurrentCourier = await courierTask;

                if (IsLoadInProgress)
                    IsLoadInProgress = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        private readonly ObserverMutex _mutex = new(); //stage 7
        private void OrderListObserver() //stage 5
        {
            #region Stage 7 (for multithreading)
            if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
                return;
            _ = Dispatcher.BeginInvoke(async () =>
            {
                await queryOrderLists();

                // Check if a restart was requested while we were working
                if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                    OrderListObserver();
            });
            #endregion Stage 7 (for multithreading)
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try { 
            OrderListObserver();
            s_bl.Order.AddObserver(OrderListObserver);
            s_bl.Courier.AddObserver(OrderListObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Close();
                return;
            }
        }



        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Order.RemoveObserver(OrderListObserver);
            s_bl.Courier.RemoveObserver(OrderListObserver);
        }


        private async Task<IEnumerable<BO.OpenOrderInList>> GetOpenOrders()
        {
            return (await s_bl.Order.GetOpenOrders(
                Helpers.Tools.UserId,
                Helpers.Tools.UserId,
                null,
                null
            )).ToList();
        }


        private IEnumerable<BO.ClosedDeliveryInList> GetDeliveryInList()
        {
            return s_bl.Order.GetClosedOrders(Helpers.Tools.UserId, Helpers.Tools.UserId, null, null).ToList();
        }

        public bool IsLoadInProgress
        {
            get { return (bool)GetValue(IsFirstLoadInProgressProperty); }
            set { SetValue(IsFirstLoadInProgressProperty, value); }
        }

        public static readonly DependencyProperty IsFirstLoadInProgressProperty =
            DependencyProperty.Register(nameof(IsLoadInProgress), typeof(bool), typeof(CourierMainWindow), new PropertyMetadata(true));


        #endregion
        //-------------------------------

    }
}


