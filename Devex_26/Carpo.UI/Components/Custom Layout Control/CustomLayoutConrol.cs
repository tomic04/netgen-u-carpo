using DevExpress.XtraLayout;

namespace Carpo.UI.Components
{
    internal class CustomLayoutControl : LayoutControl
    {
        protected override void CreateILayoutControlImplementor()
        {
            base.CreateILayoutControlImplementor();
        }

        protected override LayoutControlImplementor CreateILayoutControlImplementorCore()
        {
            return new CustomLayoutControlImplementor(this);
        }

        public event CustomDrawEventHandler CustomDrawBackground;

        public event CustomDrawEventHandler CustomDrawTextArea;

        public event CustomDrawEventHandler CustomDrawControlsArea;

        public event CustomDrawEventHandler CustomDrawSelection;

        #region OnCustomDrawBackground

        /// <summary>
        /// Triggers the CustomDrawBackground event.
        /// </summary>
        public virtual void OnCustomDrawBackground(CustomDrawEventArgs ea)
        {
            if (CustomDrawBackground != null)
                CustomDrawBackground(this, ea);
        }

        #endregion OnCustomDrawBackground

        #region OnCustomDrawTextArea

        /// <summary>
        /// Triggers the CustomDrawTextArea event.
        /// </summary>
        public virtual void OnCustomDrawTextArea(CustomDrawEventArgs ea)
        {
            if (CustomDrawTextArea != null)
                CustomDrawTextArea(this, ea);
        }

        #endregion OnCustomDrawTextArea

        #region OnCustomDrawControlsArea

        /// <summary>
        /// Triggers the CustomDrawControlsArea event.
        /// </summary>
        public virtual void OnCustomDrawControlsArea(CustomDrawEventArgs ea)
        {
            if (CustomDrawControlsArea != null)
                CustomDrawControlsArea(this, ea);
        }

        #endregion OnCustomDrawControlsArea

        #region OnCustomDrawSelection

        /// <summary>
        /// Triggers the CustomDrawSelection event.
        /// </summary>
        public virtual void OnCustomDrawSelection(CustomDrawEventArgs ea)
        {
            if (CustomDrawSelection != null)
                CustomDrawSelection(this, ea);
        }

        #endregion OnCustomDrawSelection
    }
}