namespace Aaru.Tests.Issues;

using System.Collections.Generic;
using System.IO;

/* https://github.com/aaru-dps/Aaru/issues/495
 *
 * SilasLaspada commented on Jan 10, 2021
 *
 * Trying to extract files from a CDTV image crashes with "Error reading file: Object reference not set to an instance of an object."
 */

public class _495 : FsExtractIssueTest
{
    public override string DataFolder => Path.Combine(Consts.TEST_FILES_ROOT, "Issues", "Fixed", "issue495");
    public override string TestFile => "NetworkCD.aaruf";
    public override Dictionary<string, string> ParsedOptions => new();
    public override bool Debug => false;
    public override bool Xattrs => false;
    public override string Encoding => null;
    public override bool ExpectPartitions => true;
    public override string Namespace => null;
}