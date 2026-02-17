using BlApi;
using Helpers;
using BO;

namespace BlImplementation;

internal class AdminImplemenation : BlApi.IAdmin
{

    #region oberver functions
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5 


    public void ResetDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.ResetDB();
    }

    public void InitializeDB()
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        AdminManager.InitializeDB();
    }

    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    public void ForwardClock(TimeUnit timeUnit)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        Helpers.AdminManager.UpdateClock(timeUnit switch
        {
            TimeUnit.Second => AdminManager.Now.AddSeconds(1),
            TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
            TimeUnit.Hour => AdminManager.Now.AddHours(1),
            TimeUnit.Day => AdminManager.Now.AddDays(1),
            TimeUnit.Month => AdminManager.Now.AddMonths(1),
            TimeUnit.Year => AdminManager.Now.AddYears(1),
            _ => AdminManager.Now 
        });

    }
    public BO.Config GetConfig()
    {
        return Helpers.AdminManager.GetConfig();
    }

    public void SetConfig(BO.Config config)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        Helpers.AdminManager.SetConfig(config);
    }

    public string PasswordComplexityCheck(string password)
    {
        return Helpers.Tools.PasswordStrengthChecker(password);
    }

    public void StartSimulator(int interval)  //stage 7
    {
        AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
        AdminManager.Start(interval); //stage 7
    }
    public void StopSimulator()
        => AdminManager.Stop();

}
