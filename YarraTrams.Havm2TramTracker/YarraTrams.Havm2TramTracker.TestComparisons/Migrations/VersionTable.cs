using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.VersionTableInfo;
using Microsoft.Extensions.Options;

namespace YarraTrams.Havm2TramTracker.TestComparisons
{
    public class VersionTable : DefaultVersionTableMetaData
    {
        public VersionTable(IConventionSet conventionSet, IOptions<RunnerOptions> runnerOptions)
            : base(conventionSet, runnerOptions)
        {
            //SchemaName = "distribution";
        }

        public override string TableName
        {
            get { return "VersionInfoComparisons"; }
        }
    }
}
