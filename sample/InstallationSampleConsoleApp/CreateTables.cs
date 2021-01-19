using System;
using System.Collections.Generic;
using Rinsen.DatabaseInstaller;

namespace InstallationSampleConsoleApp
{
    public class CreateTables : DatabaseVersion
    {
        public CreateTables() 
            : base(2)
        {
        }

        public override void AddDbChanges(List<IDbChange> dbChangeList)
        {
            var table = dbChangeList.AddNewTable<NullableData>().SetPrimaryKeyNonClustered();

            table.AddAutoIncrementColumn(m => m.Id);
            table.AddColumn(m => m.NotNullableBool);
            table.AddColumn(m => m.NullableBool);
            table.AddColumn(m => m.NullableByte);
            table.AddColumn(m => m.NullableByteArray);
            table.AddColumn(m => m.NullableDateTime);
            table.AddColumn(m => m.NullableDateTimeOffset);
            table.AddColumn(m => m.NullableDecimal);
            table.AddColumn(m => m.NullableDouble);
            table.AddColumn(m => m.NullableGuid);
            table.AddColumn(m => m.NullableInt);
            table.AddColumn(m => m.NullableLong);
            table.AddColumn(m => m.NullableShort);
        }
    }

    public class NullableData
    {
        public int Id { get; set; }

        public Guid? NullableGuid { get; set; }

        public DateTime? NullableDateTime { get; set; }

        public DateTimeOffset? NullableDateTimeOffset { get; set; }

        public double? NullableDouble { get; set; }

        public decimal? NullableDecimal { get; set; }

        public byte? NullableByte { get; set; }

        public byte?[] NullableByteArray { get; set; }

        public long? NullableLong { get; set; }

        public short? NullableShort { get; set; }

        public int? NullableInt { get; set; }

        public bool? NullableBool { get; set; }

        public bool NotNullableBool { get; set; }
    }
}
