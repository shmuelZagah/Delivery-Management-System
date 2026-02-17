using BlApi;
using PL.CourierLogin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Interaction logic for LoginWindow.xaml
/// </summary>
public partial class LoginWindow : Window, INotifyPropertyChanged
{
    BlApi.IBl s_bl = BlApi.Factory.Get();

    public event PropertyChangedEventHandler? PropertyChanged;

    private string _username = string.Empty;


    public string UserName
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(IsThereIsInput));
            if (!long.TryParse(UserName, out long result) && UserName != string.Empty)
                Excep.Text = "User name must be numeric";
            else Excep.Text = "";
        }
    }

    public bool IsThereIsInput => UserName.Length > 0;

    public LoginWindow()
    {
        InitializeComponent();

        this.DataContext = this;
    }


    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        Mouse.OverrideCursor = Cursors.Wait;

        Excep.Text = "";

        try
        {
            if (!long.TryParse(UserName, out long result) && UserName != string.Empty)
                throw new Exception("User name must be numeric");


            Helpers.Tools.UserT = await s_bl.Courier.Login(UserName, password.Password);

            Helpers.Tools.UserId = int.Parse(UserName);

            if (Helpers.Tools.UserT == BO.UserType.Courier)
            {
                var CourierLogin = new CourierMainWindow();
                CourierLogin.Show();
            }
            else if (Helpers.Tools.UserT == BO.UserType.Manager)
            {
                var MainWindow = new MainWindow();
                MainWindow.Show();
            }
            this.Close();

        }
        catch (Exception ex)
        {
            Excep.Text = ex.Message;
        }
        finally
        {
            Mouse.OverrideCursor = null;
        }
    }


    private void userPass_TextChanged(object sender, RoutedEventArgs e)
    {
        if (password.Password.Length > 0) passEnter.Text = "";
        else passEnter.Text = "Enter Password";
    }


    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


}
