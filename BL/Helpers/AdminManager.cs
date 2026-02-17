//using BO;
using DO;
using Helpers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TrieApi;

namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Configuration Variables and Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4-7
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4


    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    internal static event Action? ConfigUpdatedObservers; //stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //stage 5 - for clock update observers

    private static Task? _periodicTask = null; //stage 7
    private static readonly AsyncMutex s_periodicMutex = new(); //stage 7

    internal static TrieApi.Trie Addresses = new TrieApi.Trie();

    /// <summary>
    /// Method to update application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {

        if (s_periodicMutex.CheckAndSetInProgress())
            return;

        DateTime oldClock;
        lock (BlMutex)
        {
            oldClock = s_dal.Config.Clock; //stage 4
            s_dal.Config.Clock = newClock; //stage 4
        }


        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic students' updates:
        // - Go through all students to update properties that are affected by the clock update
        // - (students become not active after 5 years etc.)


        //TO_DO: //stage 7
        if (_periodicTask is null || _periodicTask.IsCompleted)

            _periodicTask = Task.Run(() =>
            {
                CourierManager.PeriodicSimulationChecks(oldClock, newClock);
                OrderManager.PeriodicSimulationChecks(oldClock, newClock);
            });

    

    

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5

        s_periodicMutex.UnsetInProgress();

    }

    /// <summary>
    /// Method for providing current configuration variables values for any BL class that may need it
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static BO.Config GetConfig() // stage 4
    => new BO.Config()
    {
        // --- COMPANY DETAILS ---
        CompanyAddress = s_dal.Config.CompanyAddress,
        Latitude = s_dal.Config.Latitude,
        Longitude = s_dal.Config.Longitude,
        MaxAirRange = s_dal.Config.MaxAirRange,

        // --- AVERAGE SPEEDS ---
        AvgCarSpeed = s_dal.Config.AvgCarSpeed,
        AvgMotocyclerSpeed = s_dal.Config.AvgMotorcycleSpeed,
        AvgBicycleSpeed = s_dal.Config.AvgBicycleSpeed,
        AvgWalkingSpeed = s_dal.Config.AvgWalkingSpeed,

        // ---  Administrative Details ---
        ManagerPassword = s_dal.Config.ManagerPass,
        ManagerId = s_dal.Config.ManagerId,

    };

    /// <summary>
    /// Method for setting current configuration variables values for any BL class that may need it
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void SetConfig(BO.Config configuration) //stage 4
    {
        bool configChanged = false; // stage 5

        Debug.WriteLine("this ----> " + configuration.MaxAirRange);


        // --- COMPANY DETAILS ---
        if (s_dal.Config.CompanyAddress != configuration.CompanyAddress)
        {
            s_dal.Config.CompanyAddress = configuration.CompanyAddress;
            configChanged = true;
        }

        if (s_dal.Config.Latitude != configuration.Latitude)
        {
            s_dal.Config.Latitude = configuration.Latitude;
            configChanged = true;
        }

        if (s_dal.Config.Longitude != configuration.Longitude)
        {
            s_dal.Config.Longitude = configuration.Longitude;
            configChanged = true;
        }

        if (s_dal.Config.MaxAirRange != configuration.MaxAirRange)
        {

            s_dal.Config.MaxAirRange = configuration.MaxAirRange;
            configChanged = true;
        }

        // --- AVERAGE SPEEDS ---
        if (s_dal.Config.AvgCarSpeed != configuration.AvgCarSpeed)
        {
            s_dal.Config.AvgCarSpeed = configuration.AvgCarSpeed;
            configChanged = true;
        }

        if (s_dal.Config.AvgMotorcycleSpeed != configuration.AvgMotocyclerSpeed)
        {
            s_dal.Config.AvgMotorcycleSpeed = configuration.AvgMotocyclerSpeed;
            configChanged = true;
        }

        if (s_dal.Config.AvgBicycleSpeed != configuration.AvgBicycleSpeed)
        {
            s_dal.Config.AvgBicycleSpeed = configuration.AvgBicycleSpeed;
            configChanged = true;
        }

        if (s_dal.Config.AvgWalkingSpeed != configuration.AvgWalkingSpeed)
        {
            s_dal.Config.AvgWalkingSpeed = configuration.AvgWalkingSpeed;
            configChanged = true;
        }


        // ---  Administrative Details ---
        if (s_dal.Config.ManagerPass != configuration.ManagerPassword)
        {
            s_dal.Config.ManagerPass = configuration.ManagerPassword;
            configChanged = true;
        }

        if (s_dal.Config.ManagerId != configuration.ManagerId)
        {
            s_dal.Config.ManagerId = configuration.ManagerId;
            configChanged = true;
        }


        // --- SYSTEM DATA (אם יש, לא חובה בשלב זה) ---
        // דוגמא אם תרצה להוסיף:
        /*
        if (s_dal.Config.ManagerId != configuration.ManagerId)
        {
            s_dal.Config.ManagerId = configuration.ManagerId;
            configChanged = true;
        }
        */

        //Stage 5: trigger observers
        if (configChanged)
            ConfigUpdatedObservers?.Invoke();
    }


    internal static void ResetDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            s_dal.ResetDB(); //stage 4
            AdminManager.UpdateClock(AdminManager.Now); //stage 5 - needed since we want the label on Pl to be updated
           ConfigUpdatedObservers?.Invoke(); 
            Console.WriteLine("Reset done.");
        }
    }

    internal static void InitializeDB() //stage 4-7
    {
        lock (BlMutex) //stage 7
        {
            DalTest.Initialization.Do(); //stage 4

            Addresses.Clear();
            s_dal.Address.DeleteAll();

            AdminManager.UpdateClock(AdminManager.Now);  //stage 5 - needed since we want the label on Pl to be updated           
            ConfigUpdatedObservers?.Invoke(); 

            Console.WriteLine("Init done.");

        }
    }

    internal static void ResetAddresses()
    {
        IEnumerable<KeyValuePair<string, int>> adr =
         from item in s_dal.Address.ReadAll()
         select new KeyValuePair<string, int>(item.FullAddress, item.Id);

        Addresses.Reset(adr);
    }


    #endregion Stage 4-7

    #region Stage 7 base

    /// <summary>    
    /// Mutex to use from BL methods to get mutual exclusion while the simulator is running
    /// </summary>
    internal static readonly object BlMutex = new(); // BlMutex = s_dal; // This field is actually the same as s_dal - it is defined for readability of locks
    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static volatile Thread? s_thread;
    /// <summary>
    /// The Interval for clock updating
    /// in minutes by second (default value is 1, will be set on Start())    
    /// </summary>
    private static int s_interval = 1;
    /// <summary>
    /// The flag that signs whether simulator is running
    /// 
    private static volatile bool s_stop = false;

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
           throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Start(int interval)
    {
        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }

    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7                                                 
    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread.Interrupt(); //awake a sleeping thread
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }
    }

    private static Task? _simulateTask = null;

    private static void clockRunner() 
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));


            if (_simulateTask is null || _simulateTask.IsCompleted)//stage 7
                _simulateTask = Task.Run(() => CourierManager.CourierSimulatorAsync());


            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }


    }


    #endregion Stage 7 base
}

