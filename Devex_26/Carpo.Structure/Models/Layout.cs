using Carpo.Structure.Models.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace Carpo.Structure.Models
{
    //public class Notifs
    //{
    //    public Control Control {get; set;}
    //    public JNotifications Notifications { get; set; }
    //}

    public class JControl
    {
        public Control Control { get; set; }
        public Type ControlType { get; set; }
        public string ControlName { get; set; }
        public bool IsNameDefinied { get; set; } = true;
        public bool DoValidate { get; set; } = false;
        public bool SaveValueSession { get; set; } = false;
        public bool SaveValueApp { get; set; } = false;
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ControlEvents
    {
        [NotifyParentPropertyAttribute(true)]
        public Control Control { get; set; }

        [NotifyParentPropertyAttribute(true)]
        public List<JEvent> Events { get; set; }
    }
}