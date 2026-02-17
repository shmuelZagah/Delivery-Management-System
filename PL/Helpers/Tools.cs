using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PL.Helpers;


internal static class Tools
{
    private static int id = 0;
    private static int interval = 0;        
    private static UserType? userType = null;
    private static bool isRunning = false;

    internal static int UserId
    {
        get { return id; }
        set { id = value; }
    }

    internal static UserType? UserT
    {
        get { return userType; }
        set { userType = value; }
    }

    internal static bool IsRunning
    {
        get { return isRunning; }
        set { isRunning = value; }
    }

    internal static int Interval
    {
        get { return interval; }
        set { interval = value; }
    }

    public static void RestartSimulator()
    {
        interval = 0;
        isRunning = false;
    }
}
