using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoToIt.Constants
{
    /// <summary>
    /// Contains various Signatures / Guids / Command IDs
    /// </summary>
    public static class Signatures
    {
        internal const string PackageGuid = "580458ce-fede-4155-890e-b0751181f5f3";
        internal const string PackageId = "guidGoToPkg";

        internal static readonly Guid CmdSetGuid = new Guid(CmdSetGuidString);
        internal const string CmdSetGuidString = "a8deaa92-f23a-481d-b60a-09e56a12b9f9";
        internal const string CmdSetId = "guidGoToCmdSet";

        internal const string ApplicationInsights_InstrumentationKey = "61f08e05-3897-44e6-8fb1-3a52f3de299d";
    }
}
