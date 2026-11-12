using System.Data;

namespace Carpo.Data.Models
{
    public class OutputParametersModel
    {
        public class OutParModel
        {
            public string Name { get; set; }

            public string TypeSet
            {
                set
                {
                    TypeStr = value;

                    switch (value)
                    {
                        case "string":
                            Type = SqlDbType.VarChar;
                            break;

                        case "int":
                            Type = SqlDbType.Int;
                            break;

                        case "boolean":
                        case "bool":
                        case "bit":
                            Type = SqlDbType.Bit;
                            break;

                        case "datetime":
                            Type = SqlDbType.Date;
                            break;

                        default:
                            Type = SqlDbType.VarChar;
                            break;
                    }
                }
            }

            public string TypeStr { get; set; }
            public SqlDbType Type { get; set; }
            public object Value { get; set; } = null;
        }
    }
}