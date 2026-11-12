using System;
using System.ComponentModel;
using System.Globalization;

namespace Carpo.Core.Log
{
    [TypeConverter(typeof(MySmartExpandableObjectConverter))]
    [DefaultPropertyAttribute("Value")]
    public class Wrapper
    {
        public Wrapper(object[] val)
        {
            m_Value = val;
        }

        private object[] m_Value;

        [NotifyParentPropertyAttribute(true)]
        [TypeConverter(typeof(MySmartExpandableObjectConverter))]
        public object[] Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }
    }

    public class MySmartExpandableObjectConverter : ExpandableObjectConverter
    {
        private TypeConverter actualConverter = null;

        private void InitConverter(ITypeDescriptorContext context)
        {
            if (actualConverter == null)
            {
                TypeConverter parentConverter = TypeDescriptor.GetConverter(context.Instance);
                PropertyDescriptorCollection coll = parentConverter.GetProperties(context.Instance);
                PropertyDescriptor pd = coll[context.PropertyDescriptor.Name];

                if (pd.PropertyType == typeof(object))
                    actualConverter = TypeDescriptor.GetConverter(pd.GetValue(context.Instance));
                else
                    actualConverter = this;
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            InitConverter(context);

            return actualConverter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            InitConverter(context); // I guess it is not needed here

            return actualConverter.ConvertFrom(context, culture, value);
        }
    }
}