using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Security.Policy;
using System.Windows.Media.Imaging;

namespace ScannerParameters
{
    public class Application : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {

            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {

            // Assebly Path
            Assembly assemblyPath = Assembly.GetExecutingAssembly();


            // Create the custom ribbon tab
            string tabName = "Parameters";
            application.CreateRibbonTab(tabName);


            // Create the button
            PushButtonData button = new PushButtonData("ParameterScannetButton", "Scanner", assemblyPath.Location, "ScannerParameters.Command");


            // Add icon to the button
            Uri uriImage = new Uri("/ScannerParameters;component/Resources/Icon.png", UriKind.RelativeOrAbsolute);
            BitmapImage largeImage = new BitmapImage(uriImage);
            button.LargeImage = largeImage;


            // Add a tooltip to a button
            button.ToolTip = "Scanner Paremeters";


            // Create a panel
            RibbonPanel ribbonPanel = application.CreateRibbonPanel(tabName, "Parameter Tools");


            // Add the button to the panel
            RibbonItem pushButton = ribbonPanel.AddItem(button);

            return Result.Succeeded;
        }
    }
}
