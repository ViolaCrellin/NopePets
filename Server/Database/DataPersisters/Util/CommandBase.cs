using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Server.Database.DataProviders.Util;
using Server.MasterData.Model;

namespace Server.Database.DataPersisters.Util
{
    public interface ICommandBase
    {

    }

    //Todo - this is nasty!
    public class CommandBase<T> : ICommandBase where T : new()
    {

        private readonly IList<ColumnInfo<T>> _columns;
        public int NumberOfColumns => _columns.Count;

        public CommandBase(IList<ColumnInfo<T>> columns)
        {
            _columns = columns;
        }

        #region Insert
        public SqlCommand Insert()
        {
            var baseCommandString = InsertRowBase();
            StringBuilder sb = new StringBuilder(baseCommandString);

            //We want to skip the ID column which is always auto incremented
            var i = 1;
            foreach (var column in _columns)
            {
                if (column.IsId)
                {
                    sb.Replace($"column{i},", string.Empty);
                    sb.Replace($"value{i},", string.Empty);
                }
              
                sb.Replace($"column{i}", $"{column.Name}");
                sb.Replace($"value{i}", $"@{column.Name}");            
                i++;
            }
            return new SqlCommand(sb.ToString());
        }

        public SqlCommand InsertJoinData()
        {
            var baseCommandString = InsertRowBase();
            StringBuilder sb = new StringBuilder(baseCommandString);

            //We want to insert ids on join tables
            //By this point the validators have checked that the foreign key exists in the table
            var i = 1;
            foreach (var column in _columns)
            {
                sb.Replace($"column{i}", $"{column.Name}");
                sb.Replace($"value{i}", $"@{column.Name}");
                i++;
            }
            return new SqlCommand(sb.ToString());
        }

        public SqlCommand InsertDataWithForeignKey(string foreignKeyName)
        {
            var baseCommandString = InsertRowBase();
            StringBuilder sb = new StringBuilder(baseCommandString);

            var i = 1;
            foreach (var column in _columns)
            {
                if (column.IsId && !string.Equals(foreignKeyName, column.Name))
                {
                    sb.Replace($"column{i},", string.Empty);
                    sb.Replace($"value{i},", string.Empty);
                }

                sb.Replace($"column{i}", $"{column.Name}");
                sb.Replace($"value{i}", $"@{column.Name}");
                i++;
            }
            return new SqlCommand(sb.ToString());
        }

        private string InsertRowBase()
        {
            var tableName = typeof(T).Name + 's';
            var commandString = $"INSERT INTO {tableName} ";
            var columnString = "(";
            var valueString = "VALUES (";
            for (var i = 1; i <= _columns.Count; i++)
            {
                if (i == _columns.Count)
                {
                    columnString += $"column{i}) ";
                    valueString += $"value{i})";
                    break;
                }
                columnString += $"column{i}, ";
                valueString += $"value{i}, ";
            }

            return commandString + columnString + valueString + ScopeIdentityString;
        }

        private const string ScopeIdentityString = "; SELECT SCOPE_IDENTITY()";


        #endregion

        public SqlCommand UpdateRow()
        {
            var baseCommandString = UpdateRowBase();
            StringBuilder sb = new StringBuilder(baseCommandString);

            var i = 1;
            foreach (var column in _columns)
            {
                if (column.IsId)
                {
                    sb.Replace($"column{i} = value{i}", string.Empty);
                }

                sb.Replace($"column{i}", $"{column.Name}");
                sb.Replace($"value{i}", $"@{column.Name}");
                i++;
            }
            return new SqlCommand(sb.ToString());
        }

        private string UpdateRowBase()
        {
            var tableName = typeof(T).Name + 's';
            var commandString = $"UPDATE {tableName} ";
            var columnValueString = "SET (";
            for (var i = 1; i <= _columns.Count; i++)
            {
                if (i == _columns.Count)
                {
                    columnValueString += $"column{i} = value{i}) WHERE";
                    break;
                }
                columnValueString += $"column{i} = value{i}, ";
            }

            var idColumns = _columns.Where(column => column.IsId);

            foreach (var idColumn in idColumns)
            {
                columnValueString += $"{idColumn.Name} = @{idColumn.Name}Value;";
            }

            return commandString + columnValueString;
        }

        public SqlCommand UpsertRow(T data)
        {
            throw new NotImplementedException();
        }

        public SqlCommand DeleteRow(T data)
        {
            throw new NotImplementedException();
        }

    }
}