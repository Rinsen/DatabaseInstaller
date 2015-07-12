using System.Collections.Generic;

namespace Rinsen.DatabaseInstaller.Sql
{
    public class NamedPrimaryKey : Dictionary<string, List<string>>
    {

        

        public void Add(string constraintName, string columnName)
        {
            if (ContainsKey(constraintName))
            {
                this[constraintName].Add(columnName);
            }
            else
            {
                Add(constraintName, new List<string> { columnName });
            }
        }


    }

    //public class NamedPrimaryKey
    //{
    //    private readonly Dictionary<string, List<string>> _namedPrimaryKeys;

    //    public int Count { get { return _namedPrimaryKeys.Count } }

    //    public NamedPrimaryKey()
    //    {
    //        _namedPrimaryKeys = new Dictionary<string, List<string>>();
    //    }

    //    public void Add(string constraintName, string columnName)
    //    {
    //        if (_namedPrimaryKeys.ContainsKey(constraintName))
    //        {
    //            _namedPrimaryKeys[constraintName].Add(columnName);
    //        }
    //        else
    //        {
    //            _namedPrimaryKeys.Add(constraintName, new List<string> { columnName });
    //        }
    //    }


    //}
}