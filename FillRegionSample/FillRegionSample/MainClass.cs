using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace FillRegionSample
{
    [Transaction(TransactionMode.Manual)]
    public class MainClass : IExternalCommand
    {
        Result IExternalCommand.Execute(ExternalCommandData commandData,
            ref string message, ElementSet elements)
        {
            // current document
            Document doc = commandData.Application.ActiveUIDocument.Document;
            // create fill pattern
            FillPattern fillPattern = new FillPattern("My Fill Pattern", FillPatternTarget.Model,
                FillPatternHostOrientation.ToView, 0.785398, 1, 1);

            // transaction start
            Transaction t = new Transaction(doc);
            t.Start("Create Fill Region");
            // add created fill pattern to doc
            FillPatternElement fillPatternElement = FillPatternElement.Create(doc, fillPattern);
            // get fill region type
            FilledRegionType fillRegionType = (FilledRegionType)new FilteredElementCollector(doc).
                OfClass(typeof(FilledRegionType)).FirstOrDefault();
            // duplicate fill region type
            FilledRegionType newFillRegionType = (FilledRegionType)fillRegionType.
                Duplicate("New FillRegionType");
            // set pattern & color
            newFillRegionType.ForegroundPatternId = fillPatternElement.Id;
            newFillRegionType.ForegroundPatternColor = new Color(0, 0, 0);

            // create four corner points
            XYZ start = new XYZ(0, 0, 0);
            XYZ right = new XYZ(10, 0, 0);
            XYZ up = new XYZ(10, 10, 0);
            XYZ left = new XYZ(0, 10, 0);

            // create lines
            Line line1 = Line.CreateBound(start, right);
            Line line2 = Line.CreateBound(right, up);
            Line line3 = Line.CreateBound(up, left);
            Line line4 = Line.CreateBound(left, start);

            // curves list to curves loop
            List<Curve> curves = new List<Curve> { line1, line2, line3, line4 };
            CurveLoop curveLoop = CurveLoop.Create(curves);

            // create fill region
            FilledRegion filledRegion = FilledRegion.Create(doc, newFillRegionType.Id,
                doc.ActiveView.Id, new List<CurveLoop> { curveLoop });

            // end transaction
            t.Commit();

            return Result.Succeeded;
        }
    }
}
