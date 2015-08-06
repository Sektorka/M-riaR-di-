using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Maria_Radio.Misc
{
    public class Variant: ErrorEvent
    {
        protected object val;

        public Variant(object val)
        {
            this.val = val;
        }

        public T Get<T>()
        {
            try
            {
                return (T) val;
            }
            catch (Exception e)
            {
                OnError(e);
                return default(T);
            }
        }

        public new Type GetType()
        {
            return val.GetType();
        }

        public override string ToString()
        {
            return val.ToString();
        }

        public static object create(string value, Type type)
        {
            try
            {
                if (type == typeof (Boolean))
                {
                    return Boolean.Parse(value);
                }

                if (type == typeof (Decimal))
                {
                    return Decimal.Parse(value);
                }

                if (type == typeof(Int32))
                {
                    return Int32.Parse(value);
                }

                if (type == typeof (Point))
                {
                    Match m = Regex.Match(value, @"{X=(\d+),Y=(\d+)}");

                    if (m.Success && m.Groups.Count > 0)
                    {
                        return new Point(int.Parse(m.Groups[1].Value), int.Parse(m.Groups[2].Value));
                    }
                    return new Point(0, 0);
                }
                
                return value;
            }
            catch (Exception)
            {
                return new Variant(value);
            }
        }
    }
}
