using System.Collections.Generic;
using System.Windows.Forms;

namespace Carpo.Structure.Interfaces
{
    public interface IJButtonControl : IJControl, IJDataRow, IJControlCaption
    { }

    public interface IJButtonEditControl : IJControl, IJEditor
    { }

    public interface IJCheckEditControl : IJControl, IJEditor, IJControlCaption
    { }

    public interface IJDateEditControl : IJControl, IJEditor
    { }

    public interface IJDaysEditControl : IJControl, IJEditor
    { }

    public interface IJHyperLinkEditControl : IJControl, IJEditor
    { }

    public interface IJLabelEditControl : IJControl, IJEditor
    { }

    public interface IJLookUpControl : IJControl, IJEditor, IJDataSource, INotifyPopulate, INotifyPopup, IJDataRow
    { }

    public interface IJAutoSuggestControl : IJControl, IJEditor, IJDataSource, INotifyPopulate, INotifyPopup, IJDataRow
    { }

    public interface IJMemoEditControl : IJControl, IJEditor
    { }
    public interface IRichEditControl : IJControl, IJEditor
    { }

    public interface IJRadioEditControl : IJControl, IJEditor, IJControlCaption
    { }

    public interface IJSearchControl : IJControl, IJEditor, IJDataRow
    { }

    public interface IJTextEditControl : IJControl, IJEditor
    { }

    public interface IJTreeLookUpControl : IJControl, IJEditor, INotifyPopulate, INotifyPopup
    { }

    public interface IJComboMultiEditControl : IJControl, IJEditor, IJDataSource, INotifyPopulate, INotifyPopup
    { }

    /// <summary>DX TokenEdit — multi-value string (separator) opt-in control.</summary>
    public interface IJTokenEditControl : IJControl, IJEditor, IJDataSource, INotifyPopulate
    { }

    public interface IJTimeEditControl : IJControl, IJEditor
    { }

    public interface IJPictureEditControl : IJControl, IJEditor
    { }

    public interface IJPdfViewerControl : IJControl, IJEditor
    { }
    public interface IJDocumentViewControl : IJControl, IJEditor { }

    public interface IJChartControl : IJControl
    { }

    public interface IJCustomUC : IJControl
    {
        Control Control { get; set; }
        string ControlName { get; set; }
        Dictionary<string, object> Properties { get; set; }
    }
}