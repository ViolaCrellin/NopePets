using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Server.MasterData.Model;

namespace Server.Database.DataProviders.Util
{
    public class ColumnInfo<T>
    {
        public string Name { get; }
        public bool IsId { get; }
        public string DataType { get; }

        public ColumnInfo(string name, string dataType, bool isId)
        {
            Name = name;
            DataType = dataType;
            IsId = isId;
        }

        protected bool Equals(ColumnInfo<T> other)
        {
            return string.Equals(Name, other.Name) && IsId == other.IsId && string.Equals(DataType, other.DataType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ColumnInfo<T>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsId.GetHashCode();
                hashCode = (hashCode * 397) ^ (DataType != null ? DataType.GetHashCode() : 0);
                return hashCode;
            }
        }

    }
}