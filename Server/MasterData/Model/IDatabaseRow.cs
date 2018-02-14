using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.MasterData.Model
{
    public interface IDatabaseRow
    {
        int PrimaryId { get; }
    }

    public interface IDatabaseJoinRow : IDatabaseRow
    {
        int SecondaryId { get; }
    }
}
