

// ReSharper disable StringLiteralTypo

namespace Aaru.Tests.Issues._542;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

/* https://github.com/aaru-dps/Aaru/issues/542
 *
 * SilasLaspada commented on Feb 10, 2021
 *
 * When extracting an image of a SafeDisc protected CD, most files aren't properly extracted.
 */

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class SimsEP : FsExtractHashIssueTest
{
    protected override string DataFolder => Path.Combine(Consts.TEST_FILES_ROOT, "Issues", "Fixed", "issue542", "exp");
    protected override string TestFile => "THE_SIMS_EP.aaruf";
    protected override Dictionary<string, string> ParsedOptions => new();
    protected override bool Debug => false;
    protected override bool Xattrs => false;
    protected override string Encoding => null;
    protected override bool ExpectPartitions => true;
    protected override string Namespace => null;
}