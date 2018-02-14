using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Database.DataProviders.Util
{
    public interface IQuery<T>
    {
        string SelectAll();
        string SelectById(int id);
    }

    public class Query<T> : IQuery<T>
    {
        private readonly string _tableName;
        private readonly string _idName;


        public Query()
        {
            _tableName = typeof(T).Name + 's';
            _idName = typeof(T).Name + "Id";
        }

        public string SelectAll() =>  $"SELECT * FROM dbo.[{_tableName}]";

        public string SelectById(int id) => $"SELECT * FROM dbo.[{_tableName}] WHERE [{_idName}] = {id}";

        public string SelectByNamedId(int id, string idName) => $"SELECT * FROM dbo.[{_tableName}] WHERE [{idName}] = {id}";

        public string SelectByIds(IEnumerable<int> ids)
        {
            var sb = new StringBuilder($"SELECT * FROM dbo.[{_tableName}] WHERE [{_idName}] IN (");
            int idCount = 1;
            ids = ids.ToList();
            foreach (var id in ids)
            {
                if (idCount == ids.Count())
                {
                    sb.Append($"{id})");
                    continue;
                }
                sb.Append($"{id}, ");
                idCount++;
            }

            return sb.ToString();
        }

    }
}