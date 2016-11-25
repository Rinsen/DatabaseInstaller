namespace Rinsen.DatabaseInstaller.SqlTypes
{
    public class AutoIncrement
    {
        public int StartValue { get; private set; }

        public int Increment { get; private set; }
        
        public AutoIncrement(int startValue = 1, int increment = 1)
        {
            StartValue = startValue;
            Increment = increment;
        }
    }
}