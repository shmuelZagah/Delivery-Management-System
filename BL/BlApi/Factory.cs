
using Helpers;

namespace BlApi;
public static class Factory
{
    private static IBl? _instance = null;
    private static object _instanceLock = new object();
    public static IBl Get()
    {
        if (_instance == null)
            lock(_instanceLock)
            {
                if (_instance == null)
                { 
                    _instance = new BlImplementation.Bl();
                }
            }

        return _instance;
    }
}
