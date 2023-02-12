using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAPI_3_7_1
{
    public class MainViewViewModel
    {
        private ExternalCommandData _commandData;
        private Document _doc;

        public List<FamilySymbol> TitleBlockType { get; } = new List<FamilySymbol>(null);
        public FamilySymbol SelectedTitleBlockType { get; set; }
        public int TitleQuantity { get; set; }
        public List<View> View { get; } = new List<View>(null);
        public View SelectedView { get; set; }
        public string Designer { get; set; }
        public DelegateCommand SaveCommand { get;}


        public MainViewViewModel(ExternalCommandData commandData)
        {
            _commandData = commandData;
            _doc = _commandData.Application.ActiveUIDocument.Document;

            TitleBlockType = SheetUtils.GetTitleBlocks(commandData);
            TitleQuantity = 1;
            View = ViewsUtils.GetViews(commandData);
            Designer = "Проектировщик";
            SaveCommand = new DelegateCommand(OnSaveCommand);
        }

        public void OnSaveCommand()
        {
            UIApplication uiapp = _commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Document doc = uidoc.Document;

            using(var ts = new Transaction (doc, "Создание листов"))
            {     
                try
                {
                    ts.Start();

                    for (int i=0; i< TitleQuantity; i++)
                    { 
                    ViewSheet viewSheet = ViewSheet.Create(doc, SelectedTitleBlockType.Id);

                    viewSheet.get_Parameter(BuiltInParameter.SHEET_DESIGNED_BY).Set(Designer);

                    if (viewSheet == null)
                    {
                        throw new Exception("Error");
                    }

                    ElementId dublicatedPlanId = SelectedView.Duplicate(ViewDuplicateOption.Duplicate);

                    UV location = new UV(viewSheet.Outline.Max.U - viewSheet.Outline.Min.U / 2, 
                                       (viewSheet.Outline.Max.V - viewSheet.Outline.Min.V) / 2);

                    Viewport viewport = Viewport.Create(doc, viewSheet.Id, dublicatedPlanId, new XYZ(location.U, location.V, 0));
                    }
                    ts.Commit();
                }
                catch
                { }
            }
            RaiseCloseRequest();
        }

        public event EventHandler HideRequest;
        private void RaiseHideRequest()
        {
            HideRequest?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ShowRequest;
        private void RaiseShowRequest()
        {
            ShowRequest?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler CloseRequest;
        private void RaiseCloseRequest()
        {
            CloseRequest?.Invoke(this, EventArgs.Empty);
        }
    }
}

