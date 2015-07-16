namespace Rinsen.DatabaseInstaller
{
    public interface IDbChange
    {
        string GetUpScript();

        string GetDownScript();

    }
}
