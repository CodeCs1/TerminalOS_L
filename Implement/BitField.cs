using System;
using System.Reflection;
//https://stackoverflow.com/questions/14464/bit-fields-in-c-sharp
namespace TerminalOS_L {
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    sealed class BitField : Attribute
    {
        public int Length {get;private set;}
        
        public BitField(int Length)
        {
            if (Length < 1) {
                throw new ArgumentOutOfRangeException("Bit count must be greater than 0");
            }
            this.Length = Length;
        }
    }

    public class BitFieldOutput {
        public static int ToInt<T>(T t) where T : struct {
            int res = 0;
            int offset = 0;
            foreach(FieldInfo f in t.GetType().GetFields()) {
                object[] attrs = f.GetCustomAttributes(typeof(BitField),false);
                if (attrs.Length == 1) {
                    int mask = 0;
                    int Length = ((BitField)attrs[0]).Length;
                    for (int i=0;i<Length;i++) {
                        mask |= 1 << i;
                    }
                    res |= ((int)f.GetValue(t) & mask)<<offset;
                    offset += Length;
                }
            }
            return res;
        }
    }
}