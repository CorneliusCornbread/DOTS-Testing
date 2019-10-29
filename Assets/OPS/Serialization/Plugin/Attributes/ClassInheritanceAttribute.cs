using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Attributes
{
    /// <summary>
    /// Attach this Attribute to a base class and link to a class inheriting the base class.
    /// As Parameter apply a type of this inherited class and an increasing index beginning by 0.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ClassInheritanceAttribute : Attribute
    {
        private Type inheritanceType;

        public Type InheritanceType
        {
            get
            {
                return this.inheritanceType;
            }
        }

        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        public ClassInheritanceAttribute(Type _InheritanceType, int _Index)
        {
            this.inheritanceType = _InheritanceType;
            this.index = _Index;
        }

        //1.Go to earlyies base with SerializeClassAttrbute
        //2.Serialize
        //3. chekc if _ojbect type != current type
        //4.Iteratie trough all inheritane atrributes.
        //5.Check wihich one is in inheritance chain
        //6. got to this one. repeat
    }
}
