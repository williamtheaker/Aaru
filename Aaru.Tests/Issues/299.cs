namespace Aaru.Tests.Issues;

using System.Collections.Generic;
using System.IO;
using Aaru.CommonTypes.Interfaces;
using Aaru.DiscImages;

/* FakeShemp commented on Mar 1, 2020
 *
 * Neither of the Dreamcast homebrew images found on
 * http://dcevolution.sourceforge.net/index.php?id=house_of_terror can be converted to DICF.
 */

public class _299 : OpticalImageConvertIssueTest
{
    public override Dictionary<string, string> ParsedOptions => new();
    public override string DataFolder => Path.Combine(Consts.TEST_FILES_ROOT, "Issues", "Pending", "issue299");
    public override string InputPath => "2d_house_of_terror.cdi";
    public override string SuggestedOutputFilename => "AaruTestIssue299.aif";
    public override IWritableImage OutputFormat => new AaruFormat();
    public override string Md5 => null;
    public override bool UseLong => true;
}