using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using GalaSoft.MvvmLight.Command;
using ScannerParameters.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ScannerParameters.ViewModels
{
    public class ScannerParametersViewModel : INotifyPropertyChanged
    {
        private readonly UIApplication _uiApp;
        private readonly ScannerParametersModel _model;
        public event PropertyChangedEventHandler PropertyChanged;
        public string ParamName
        {
            get => _model.ParamName;
            set
            {
                if (_model.ParamName != value)
                {
                    _model.ParamName = value;
                    OnPropertyChanged(nameof(ParamName));
                }
            }
        }
        public string ParamValue
        {
            get => _model.ParamValue;
            set
            {
                if (_model.ParamValue != value)
                {
                    _model.ParamValue = value;
                    OnPropertyChanged(nameof(ParamValue));
                }
            }
        }

        public ICommand SelectElementCommand { get; }
        public ICommand IsolateElementCommand { get; }


        // Used to bind the functions of the WFP xaml Form.
        public ScannerParametersViewModel(UIApplication _uiApplication)
        {
            _uiApp = _uiApplication;
            _model = new ScannerParametersModel();
            SelectElementCommand = new RelayCommand(SelectElement);
            IsolateElementCommand = new RelayCommand(IsolateElement);

        }

        // Watches if has any changes in the parameters
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Select the elements
        public void SelectElement()
        {
            // Ensure the parameter name input cannot be left empty.
            if (string.IsNullOrWhiteSpace(ParamName))
            {
                TaskDialog.Show("Revit", "The Parameter Name cannot be empty, please provide a Parameter Name and try again.");
                return;
            }

            try
            {
                // declaration of variables.
                string paramName = ParamName;
                string paramValue = ParamValue;
                Document doc = _uiApp.ActiveUIDocument.Document;
                UIDocument uiDoc = _uiApp.ActiveUIDocument;


                // Find the elements by parameter and assign to a variable.
                IEnumerable<Element> elements = FindElementsByParameter(doc, paramName, paramValue);


                // Validate the existence of the specified parameter name within the elements.
                if (!elements.Any())
                {
                    TaskDialog.Show("Revit", $"There are no elements with the parameter name provided: {paramName}.");
                    return;
                }


                // Get a list of the Ids of all elements that have been found.
                List<ElementId> elementIds = elements.Select(el => el.Id).ToList();



                // Display a warning message indicating the number of elements found with the specified parameter value.
                MessageBox.Show($"{elementIds.Count} element(s) have been found with the parameter value that was provided.", "Revit");


                // Select all the elements that matches with the parameters that are provided.
                if (elementIds.Count > 0)
                {
                    using (Transaction transaction = new Transaction(doc, "Select Elements"))
                    {
                        transaction.Start();
                        uiDoc.Selection.SetElementIds(elementIds);
                        transaction.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Revit", "No Elements have been found with the parameters provided.");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Error while trying to select elements. Isue:{ex}");
            }

        }

        // Isolate the elements
        public void IsolateElement()
        {
            // Ensure the parameter name input cannot be left empty.
            if (string.IsNullOrWhiteSpace(ParamName))
            {
                TaskDialog.Show("Revit", "The Parameter Name cannot be empty, please provide a Parameter Name and try again.");
                return;
            }
            try
            {
                // declaration of variables.
                string paramName = ParamName;
                string paramValue = ParamValue;
                Document doc = _uiApp.ActiveUIDocument.Document;
                View activeView = doc.ActiveView;


                // Find the elements by parameter and assign to a variable.
                IEnumerable<Element> elements = FindElementsByParameter(doc, paramName, paramValue);


                // Validate the existence of the specified parameter name within the elements.
                if (!elements.Any())
                {
                    TaskDialog.Show("Revit", $"There are no elements with the parameter name provided: {paramName}.");
                    return;
                }


                // Get a list of the Ids of all elements that have been found.
                List<ElementId> elementIds = elements.Select(el => el.Id).ToList();


                // Display a warning message indicating the number of elements found with the specified parameter value.
                MessageBox.Show($"{elementIds.Count} element(s) have been found with the parameter value that was provided.", "Revit");


                // Isolate all the elements that matches with the parameters that are provided.
                if (elementIds.Count > 0)
                {
                    using (Transaction transaction = new Transaction(doc, "Isolate Elements"))
                    {
                        transaction.Start();
                        activeView.IsolateElementsTemporary(elementIds);
                        transaction.Commit();
                    }
                }
                else
                {
                    TaskDialog.Show("Revit", "No Elements have been found with the parameters provided.");
                }

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"Error while trying to select elements. Isue:{ex}");
            }


        }

        // Find the elements
        private IEnumerable<Element> FindElementsByParameter(Document doc, string paramName, string paramValue)
        {
            try
            {
                // Filter elements that are not elements type in the document
                FilteredElementCollector collector = new FilteredElementCollector(doc)
                    .WhereElementIsNotElementType();


                // Some manipulation of format string to the user can type lowercase and the parameter be found
                paramName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(paramName.ToLower());
                paramValue = paramValue?.ToLower();


                // Declaration of variable
                IEnumerable<Element> elements;


                // Get elements in the collector that matches with the parameter provided
                if (string.IsNullOrWhiteSpace(paramValue))
                {
                    elements = collector
                        .Where(el =>
                        {
                            Parameter parameter = el.LookupParameter(paramName);
                            return parameter != null && string.IsNullOrEmpty(parameter.AsString());
                        });
                }
                else
                {
                    elements = collector
                    .Where(el =>
                    {
                        Parameter parameter = el.LookupParameter(paramName);
                        return parameter != null &&
                            parameter.AsString() != null &&
                            parameter.AsString().ToLower() == paramValue;
                    })
                    .ToList();
                }


                return elements;

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Error", $"error while filtering elements: {ex}");
                return Enumerable.Empty<Element>();
            }
        }

    }
}
