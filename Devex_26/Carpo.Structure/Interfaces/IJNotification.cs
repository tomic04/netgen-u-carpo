using Carpo.Structure.Models.Core;
using static Carpo.Structure.Enums.Controls;

namespace Carpo.Structure.Interfaces
{
    public interface INotifyPopup
    {
        void OpenPopup();
    }

    public interface INotifyPopulate
    {
        void Populate(JEventAction action, bool force, bool isAsync, FocusType focusType);
    }
}