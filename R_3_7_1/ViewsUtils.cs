using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_3_7_1
{
    public class ViewsUtils
    {
        public static List<View> GetViews(ExternalCommandData commandData)
        {
            var doc = commandData.Application.ActiveUIDocument.Document;
            List<View> views = new FilteredElementCollector(doc)
                .OfClass(typeof(View))                
                .Cast<View>()
                .ToList();

            return views;
        }
       

        
    }
}
