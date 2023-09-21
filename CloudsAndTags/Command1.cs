#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace CloudsAndTags
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiApp = commandData.Application;
                Document doc = uiApp.ActiveUIDocument.Document;

                FilteredElementCollector viewCollector = filterMethod(doc);
                foreach (View view in viewCollector)
                {
                    if (view is ViewSheet || view is ViewPlan || view is ViewSection || view is View3D)
                    {
                        // Get all RevisionCloud elements in the current view
                        FilteredElementCollector cloudCollector = new FilteredElementCollector(doc, view.Id)
                            .OfClass(typeof(RevisionCloud));

                        foreach (RevisionCloud cloud in cloudCollector)
                        {
                            // Check if the cloud has associated tags
                            ICollection<ElementId> tagIds = cloud.GetDependentElements(null);

                            if (tagIds.Count > 0)
                            {
                                TaskDialog.Show("Cloud with Tags", $"Cloud {cloud.Name} in view {view.Name} has {tagIds.Count} associated tags.");
                            }
                            else
                            {
                                TaskDialog.Show("Cloud without Tags", $"Cloud {cloud.Name} in view {view.Name} has no associated tags.");
                            }
                        }
                    }
                }

                return Result.Succeeded;
            }
            catch (ArgumentException ex)
            {
                TaskDialog.Show("Error", $"An error occurred: {ex.Message}");
                return Result.Failed;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"An unexpected error occurred: {ex.Message}");
                return Result.Failed;
            }
        }

        private static FilteredElementCollector filterMethod(Document doc)
        {
            // Get all views in the document
            return new FilteredElementCollector(doc)
                .OfClass(typeof(View));
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
