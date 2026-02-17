using System.Configuration;
using System.Data;
using System.Security.AccessControl;
using System.Windows;

namespace PL;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{


    protected override void OnExit(ExitEventArgs e)
    {
        BlApi.Factory.Get().Admin.StopSimulator();
        Helpers.Tools.RestartSimulator();
        base.OnExit(e);
    }



}

