using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Runner.VersionTableInfo;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get { return "VersionInfoComparisons"; }
        }
    }
}
