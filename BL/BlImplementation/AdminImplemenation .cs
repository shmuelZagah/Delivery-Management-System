using BIApi;
using Helpers;
using BO;

namespace BlImplementation;

internal class AdminImplemenation : IAdmin
{
    public void ResetDB()
    {
        AdminManager.ResetDB();
    }

    public void InitializeDB()
    {
        AdminManager.InitializeDB();
    }

    public DateTime GetClock()
    {
        return AdminManager.Now;
    }

    public void ForwardClock(TimeUnit timeUnit)
    {
        Helpers.AdminManager.UpdateClock(timeUnit switch
        {
            TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
            TimeUnit.Hour => AdminManager.Now.AddHours(1),
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
        Helpers.AdminManager.SetConfig(config);
    }

}
