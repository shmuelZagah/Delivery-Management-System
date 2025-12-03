namespace BIApi;

public static class Factory
{
    public static IBi Get() => new BlImplementation.Bl();
}
