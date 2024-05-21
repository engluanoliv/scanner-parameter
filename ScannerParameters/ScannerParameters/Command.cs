using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows;

namespace ScannerParameters
{
    // Add this to connect the action to the button 
    [Transaction(TransactionMode.Manual)]
    internal class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // Declaration of usable variables
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;
                ViewType activeViewType = uidoc.ActiveView.ViewType;

                // Views required to use the add in
                List<ViewType> allowedViews = new List<ViewType>
                {
                    ViewType.CeilingPlan,
                    ViewType.FloorPlan,
                    ViewType.ThreeD,
                };

                if (!allowedViews.Contains(activeViewType))
                {
                    MessageBox.Show("This Scanner can not be used in this type of view. The only view types allowed are: Reflected Ceiling Plan, 3D View and Floor Plan.",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    return Result.Cancelled;
                }


                ScannerParameterForm scannerForm = new ScannerParameterForm(uiapp);
                scannerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return Result.Failed;
            }

            return Result.Succeeded;
        }
    }
}
