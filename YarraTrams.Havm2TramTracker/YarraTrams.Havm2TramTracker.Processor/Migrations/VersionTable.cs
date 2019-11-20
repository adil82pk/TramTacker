using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;
using FluentMigrator.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace YarraTrams.Havm2TramTracker.Processor
{
    [VersionTableMetaData]
    public class VersionTable : DefaultVersionTableMetaData
    {
        public override string TableName
        {
            get { return "VersionInfoHavm2TT"; }
        }
    }
}
