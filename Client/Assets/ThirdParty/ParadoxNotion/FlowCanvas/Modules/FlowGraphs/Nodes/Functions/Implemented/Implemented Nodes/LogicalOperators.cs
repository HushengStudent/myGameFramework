using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;

namespace FlowCanvas.Nodes
{

    [Category("Logic Operators")]
    [Description("Returns if the object is not null")]
    [Name("Is Valid")]
    public class IsNotNull : PureFunctionNode<bool, object>
    {
        public override bool Invoke(object OBJECT) {
            return OBJECT != null && !OBJECT.Equals(null);
        }
    }

    [Category("Logic Operators")]
    [Description("Returns whether the input object is of type T as well as the object itself for convenience.")]
    public class IsOfType : PureFunctionNode<bool, object, System.Type>
    {
        public object OBJECT { get; private set; }
        public override bool Invoke(object OBJECT, System.Type type) {
            this.OBJECT = OBJECT;
            return OBJECT != null && type.RTIsAssignableFrom(OBJECT.GetType());
        }
    }

    ////////////////////////////////////////
    ////////////ANY COMPARABLE//////////////
    ////////////////////////////////////////

    [Category("Logic Operators/Any")]
    [Name(">")]
    [Description("Any Greater Than")]
    public class AnyGreaterThan : PureFunctionNode<bool, IComparable, IComparable>
    {
        public override bool Invoke(IComparable a, IComparable b) {
            return a.CompareTo(b) == 1;
        }
    }

    [Category("Logic Operators/Any")]
    [Name("≥")]
    [Description("Any Greater Or Equal Than")]
    public class AnyGreaterEqualThan : PureFunctionNode<bool, IComparable, IComparable>
    {
        public override bool Invoke(IComparable a, IComparable b) {
            return a.CompareTo(b) == 1 || object.Equals(a, b);
        }
    }

    [Category("Logic Operators/Any")]
    [Name("<")]
    [Description("Any Less Than")]
    public class AnyLessThan : PureFunctionNode<bool, IComparable, IComparable>
    {
        public override bool Invoke(IComparable a, IComparable b) {
            return a.CompareTo(b) == -1;
        }
    }

    [Category("Logic Operators/Any")]
    [Name("≤")]
    [Description("Any Less Or Equal Than")]
    public class AnyLessEqualThan : PureFunctionNode<bool, IComparable, IComparable>
    {
        public override bool Invoke(IComparable a, IComparable b) {
            return a.CompareTo(b) == -1 || object.Equals(a, b);
        }
    }

    [Category("Logic Operators/Any")]
    [Name("=")]
    [Description("Any Equal To")]
    public class AnyEqual : PureFunctionNode<bool, object, object>
    {
        public override bool Invoke(object a, object b) {
            return object.Equals(a, b);
        }
    }

    [Category("Logic Operators/Any")]
    [Name("≠")]
    [Description("Any Not Equal To")]
    public class AnyNotEqual : PureFunctionNode<bool, object, object>
    {
        public override bool Invoke(object a, object b) {
            return !object.Equals(a, b);
        }
    }





    ////////////////////////////////////////
    ///////////////FLOATS///////////////////
    ////////////////////////////////////////

    [Category("Logic Operators/Floats")]
    [Name("+")]
    [Description("Float Add")]
    public class FloatAdd : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float a, float b) {
            return a + b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("-")]
    [Description("Float Subtract")]
    public class FloatSubtract : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float a, float b) {
            return a - b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("×")]
    [Description("Float Mutliply")]
    public class FloatMultiply : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float a, float b) {
            return a * b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("÷")]
    [Description("Float Divide")]
    public class FloatDivide : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float a, float b) {
            return a / b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("%")]
    [Description("Float Modulo")]
    public class FloatModulo : PureFunctionNode<float, float, float>
    {
        public override float Invoke(float value, float mod) {
            return value % mod;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name(">")]
    [Description("Float Greater Than")]
    public class FloatGreaterThan : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a > b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("≥")]
    [Description("Float Greater Or Equal Than")]
    public class FloatGreaterEqualThan : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a >= b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("<")]
    [Description("Float Less Than")]
    public class FloatLessThan : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a < b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("≤")]
    [Description("Float Less Or Equal Than")]
    public class FloatLessEqualThan : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a <= b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("=")]
    [Description("Float Equal To")]
    public class FloatEqual : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a == b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("≠")]
    [Description("Float Not Equal To")]
    public class FloatNotEqual : PureFunctionNode<bool, float, float>
    {
        public override bool Invoke(float a, float b) {
            return a != b;
        }
    }

    [Category("Logic Operators/Floats")]
    [Name("Invert")]
    [Description("Float Invert the input ( value = value * -1 )")]
    public class FloatInvert : PureFunctionNode<float, float>
    {
        public override float Invoke(float value) {
            return value * -1;
        }
    }

    [Category("Logic Operators/Floats")]
    [Description("Float Round value to closest of interval ( round(value / interval) * interval )")]
    public class FloatSnap : PureFunctionNode<int, float, int>
    {
        public override int Invoke(float value, int interval) {
            return (int)Mathf.Round(value / interval) * interval;
        }
    }


    ////////////////////////////////////////
    ///////////////INTEGER//////////////////
    ////////////////////////////////////////

    [Category("Logic Operators/Integers")]
    [Name("+")]
    [Description("Integer Add")]
    public class IntegerAdd : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int a, int b) {
            return a + b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("-")]
    [Description("Integer Subtract")]
    public class IntegerSubtract : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int a, int b) {
            return a - b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("×")]
    [Description("Integer Multiply")]
    public class IntegerMultiply : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int a, int b) {
            return a * b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("÷")]
    [Description("Integer Divide")]
    public class IntegerDivide : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int a, int b) {
            return b == 0 ? 0 : a / b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("%")]
    [Description("Integer Modulo")]
    public class IntegerModulo : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int value, int mod) {
            return value % mod;
        }
    }


    [Category("Logic Operators/Integers")]
    [Name(">")]
    [Description("Integer Greater Than")]
    public class IntegerGreaterThan : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a > b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("≥")]
    [Description("Integer Greater Or Equal Than")]
    public class IntegerGreaterEqualThan : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a >= b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("<")]
    [Description("Integer Less Than")]
    public class IntegerLessThan : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a < b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("≤")]
    [Description("Integer Less Or Equal Than")]
    public class IntegerLessEqualThan : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a <= b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("=")]
    [Description("Integer Equal To")]
    public class IntegerEqual : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a == b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("≠")]
    [Description("Integer Not Equal To")]
    public class IntegerNotEqual : PureFunctionNode<bool, int, int>
    {
        public override bool Invoke(int a, int b) {
            return a != b;
        }
    }

    [Category("Logic Operators/Integers")]
    [Name("Invert")]
    [Description("Integer Invert the input ( value = value * -1 )")]
    public class IntegerInvert : PureFunctionNode<int, int>
    {
        public override int Invoke(int value) {
            return value * -1;
        }
    }

    [Category("Logic Operators/Integers")]
    [Description("Integer Round value to closest of interval ( round(value / interval) * interval )")]
    public class IntegerSnap : PureFunctionNode<int, int, int>
    {
        public override int Invoke(int value, int interval) {
            return (int)Mathf.Round(value / interval) * interval;
        }
    }

    ////////////////////////////////////////
    ///////////////BOOLEAN//////////////////
    ////////////////////////////////////////

    [Category("Logic Operators/Boolean")]
    [Name("=")]
    [Description("Boolean Equal To")]
    public class BooleanEqual : PureFunctionNode<bool, bool, bool>
    {
        public override bool Invoke(bool a, bool b) {
            return a == b;
        }
    }

    [Category("Logic Operators/Boolean")]
    [Name("≠")]
    [Description("Boolean Not Equal To")]
    public class BooleanNotEqual : PureFunctionNode<bool, bool, bool>
    {
        public override bool Invoke(bool a, bool b) {
            return a != b;
        }
    }

    [Category("Logic Operators/Boolean")]
    [Description("True if A and B are both true")]
    public class AND : PureFunctionNode<bool, bool, bool>
    {
        public override bool Invoke(bool a, bool b) {
            return a && b;
        }
    }

    [Category("Logic Operators/Boolean")]
    [Description("True if A or B is true")]
    public class OR : PureFunctionNode<bool, bool, bool>
    {
        public override bool Invoke(bool a, bool b) {
            return a || b;
        }
    }

    [Category("Logic Operators/Boolean")]
    [Description("True if A or B is true, but not both")]
    public class XOR : PureFunctionNode<bool, bool, bool>
    {
        public override bool Invoke(bool a, bool b) {
            return ( a || b ) && ( a != b );
        }
    }

    [Category("Logic Operators/Boolean")]
    [Description("Inverts the input")]
    public class NOT : PureFunctionNode<bool, bool>
    {
        public override bool Invoke(bool value) {
            return !value;
        }
    }

    ////////////////////////////////////////
    ///////////////VECTORS//////////////////
    ////////////////////////////////////////

    [Category("Logic Operators/Vector3")]
    [Name("=")]
    [Description("Vector3 Equal To")]
    public class Vector3Equal : PureFunctionNode<bool, Vector3, Vector3>
    {
        public override bool Invoke(Vector3 a, Vector3 b) {
            return a == b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("≠")]
    [Description("Vector3 Not Equal To")]
    public class Vector3NotEqual : PureFunctionNode<bool, Vector3, Vector3>
    {
        public override bool Invoke(Vector3 a, Vector3 b) {
            return a != b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("+")]
    [Description("Vector3 Add")]
    public class Vector3Add : PureFunctionNode<Vector3, Vector3, Vector3>
    {
        public override Vector3 Invoke(Vector3 a, Vector3 b) {
            return a + b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("-")]
    [Description("Vector3 Subtract")]
    public class Vector3Subtract : PureFunctionNode<Vector3, Vector3, Vector3>
    {
        public override Vector3 Invoke(Vector3 a, Vector3 b) {
            return a - b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("×")]
    [Description("Vector3 Multiply")]
    public class Vector3Multiply : PureFunctionNode<Vector3, Vector3, float>
    {
        public override Vector3 Invoke(Vector3 a, float b) {
            return a * b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("÷")]
    [Description("Vectro3 Divide")]
    public class Vector3Divide : PureFunctionNode<Vector3, Vector3, float>
    {
        public override Vector3 Invoke(Vector3 a, float b) {
            return a / b;
        }
    }

    [Category("Logic Operators/Vector3")]
    [Name("Invert")]
    [Description("Vector3 Invert the input ( value = value * -1 )")]
    public class Vector3Invert : PureFunctionNode<Vector3, Vector3>
    {
        public override Vector3 Invoke(Vector3 value) {
            return value * -1;
        }
    }

}