using BO;
using PL.Courier;
using PL.Helpers;
using PL.Order;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    #region Button Handlers for Opening Other Windows
    private void btnShowMainTable_Click(object sender, RoutedEventArgs e)
    {
        //eg: new MainTableWindow().Show();
    }
    private void btnInitializeData_Click(object sender, RoutedEventArgs e)
    {
        // שלב 1 — הודעת אישור
        if (MessageBox.Show("Are you sure you want to initialize the database?",
                            "Initialize DB",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question) != MessageBoxResult.Yes)
            return;

        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            // שלב 2 — קריאה ל-BL
            s_bl.Admin.InitializeDB();

            // שלב 3 — רענון מסכים פתוחים אם יש צורך
            MessageBox.Show("Database initialized successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

            Configuration = s_bl.Admin.GetConfig();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Initialization failed:\n{ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }
    private void btnResetDatabase_Click(object sender, RoutedEventArgs e)
    {
        // שלב 1 — הודעת אישור
        if (MessageBox.Show("Are you sure you want to reset the database? All data will be removed.",
                            "Reset DB",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) != MessageBoxResult.Yes)
            return;

        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            // שלב 2 — קריאה ל-BL
            s_bl.Admin.ResetDB();

            Configuration = s_bl.Admin.GetConfig();


            // שלב 3 — סגירת חלונות פתוחים אם צריך
            CloseAllChildWindows();

            MessageBox.Show("Database reset successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Reset failed:\n{ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void CloseAllChildWindows()
    {
        foreach (Window w in Application.Current.Windows)
        {
            if (w != this)
                w.Close();
        }
    }

    private void btnHandleCourierTable_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        CourierListWindow? courierListWindow = App.Current.Windows.OfType<CourierListWindow>().FirstOrDefault();
        try
        {
            if (courierListWindow == null)
            {
                courierListWindow = new CourierListWindow();
                courierListWindow.Show();
            }
            else
                courierListWindow.Activate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnHandleOrderTable_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            OrderListWindow? orderListWindow = App.Current.Windows.OfType<OrderListWindow>().FirstOrDefault();
            if (orderListWindow == null)
            {
                orderListWindow = new PL.Order.OrderListWindow();
                orderListWindow.Show();
            }
            else orderListWindow.Activate();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            CloseAllChildWindows();
            new LoginWindow().Show();
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"ERROR: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally { Mouse.OverrideCursor = null; }

    }

    #endregion

    #region Dependency Properties and g/s functions

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    public static readonly DependencyProperty ConfigurationProperty =
DependencyProperty.Register("Configuration", typeof(BO.Config), typeof(MainWindow));

    public BO.Config Configuration
    {
        get => (BO.Config)GetValue(ConfigurationProperty);
        set => SetValue(ConfigurationProperty, value);
    }

    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }


    //--------------------
    #region Toggle
    //--------------------

    private bool _isRunning;

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            if (_isRunning == value) return;
            _isRunning = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ToggleText));
            OnPropertyChanged(nameof(IsLocked));
        }
    }

    public string ToggleText => IsRunning ? "Pause" : "Start";
    public bool IsLocked => IsRunning ? false : true;



    public static readonly DependencyProperty IntervalProperty =
      DependencyProperty.Register(
          nameof(Interval),
          typeof(int),
          typeof(MainWindow),
          new PropertyMetadata(0, OnIntervalChanged)
      );

    public int Interval
    {
        get => (int)GetValue(IntervalProperty);
        set
        {
            SetValue(IntervalProperty, value);
        }
    }

    private static void OnIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Helpers.Tools.Interval = (int)e.NewValue;
    }



    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
       PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    private void btnSimultion_Click(object sender, RoutedEventArgs e)
    {
        Helpers.Tools.IsRunning = IsRunning = !IsRunning;

        if (!IsRunning)
        {
            s_bl.Admin.StopSimulator();
        }
        else
        {
            s_bl.Admin.StartSimulator(Interval);
        }
    }

    #endregion
    //------------------

    #endregion

    #region Configuration Apply Button Handlers
    private void btnApplyCompanyAddress_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Company Address updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Company Address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyLatitude_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Latitude updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Latitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyLongitude_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Longitude updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Longitude: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyMaxAirRange_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Max Air Range updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Max Air Range: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }
    private void btnApplyAvgCarSpeed_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Average Car Speed updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Average Car Speed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyAvgMotocyclerSpeed_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Average Motocycler Speed updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Average Motocycler Speed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyAvgBicycleSpeed_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Average Bicycle Speed updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Average Bicycle Speed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }

    private void btnApplyAvgWalkingSpeed_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        try
        {
            s_bl.Admin.SetConfig(Configuration);
            MessageBox.Show("Average Walking Speed updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to update Average Walking Speed: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }
    #endregion

    #region advance clock button handlers
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        Mouse.OverrideCursor = null;
    }

    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        Mouse.OverrideCursor = null;
    }

    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        Mouse.OverrideCursor = null;
    }

    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        Mouse.OverrideCursor = null;
    }

    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;
        s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
        Mouse.OverrideCursor = null;
    }

    #endregion

    #region Clock and Config Observers


    private readonly ObserverMutex _mutex = new(); //stage 7

    private void ClockObserver() //stage 5
    {
        if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;
        _ = Dispatcher.BeginInvoke(async () =>
        {
            CurrentTime = s_bl.Admin.GetClock();

            // Check if a restart was requested while we were working
            if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                ClockObserver();
        });
    }

    private void ConfigObserver() //stage 5
    {
        if (_mutex.CheckAndSetLoadInProgressOrRestartRequired())
            return;
        _ = Dispatcher.BeginInvoke(async () =>
        {
            Configuration = s_bl.Admin.GetConfig()!;

            // Check if a restart was requested while we were working
            if (await _mutex.UnsetLoadInProgressAndCheckRestartRequested())
                ConfigObserver();
        });
    }


    #endregion

    #region Window Events
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        CurrentTime = s_bl.Admin.GetClock();
        Configuration = s_bl.Admin.GetConfig();

        s_bl.Admin.AddClockObserver(ClockObserver);
        s_bl.Admin.AddConfigObserver(ConfigObserver);

        IsRunning = Helpers.Tools.IsRunning;
        Interval = Helpers.Tools.Interval;
        SetValue(IntervalProperty, Helpers.Tools.Interval);
    }

    private void Window_Closed(object sender, EventArgs e)
    {
        s_bl.Admin.RemoveClockObserver(ClockObserver);
        s_bl.Admin.RemoveConfigObserver(ConfigObserver);
    }

    #endregion


    public MainWindow()
    {
        InitializeComponent();


    }

}