using UnityEngine;


namespace ParadoxNotion{

	/// Has some prety common operations amongst values.
    public static class OperationTools {


		public static string GetOperationString(OperationMethod om){

			if (om == OperationMethod.Set)
				return " = ";

			if (om == OperationMethod.Add)
				return " += ";

			if (om == OperationMethod.Subtract)
				return " -= ";

			if (om == OperationMethod.Multiply)
				return " *= ";

			if (om == OperationMethod.Divide)
				return " /= ";

			return string.Empty;
		}

		public static float Operate(float a, float b, OperationMethod om, float delta = 1f ){
			if (om == OperationMethod.Set)
				return b;
			if (om == OperationMethod.Add)
				return a + (b * delta);
			if (om == OperationMethod.Subtract)
				return a - (b * delta);
			if (om == OperationMethod.Multiply)
				return a * (b * delta);
			if (om == OperationMethod.Divide)
				return a / (b * delta);
			return a;
		}

		public static int Operate(int a, int b, OperationMethod om){
			if (om == OperationMethod.Set)
				return b;
			if (om == OperationMethod.Add)
				return a + b;
			if (om == OperationMethod.Subtract)
				return a - b;
			if (om == OperationMethod.Multiply)
				return a * b;
			if (om == OperationMethod.Divide)
				return a / b;
			return a;
		}


		public static Vector3 Operate(Vector3 a, Vector3 b, OperationMethod om, float delta = 1f){
			if (om == OperationMethod.Set)
				return b;
			if (om == OperationMethod.Add)
				return a + (b * delta);
			if (om == OperationMethod.Subtract)
				return a - (b * delta);
			if (om == OperationMethod.Multiply)
				return Vector3.Scale(a, (b * delta));
			if (om == OperationMethod.Divide){
				b *= delta;
				return new Vector3( (a).x/(b).x, (a).y/(b).y, (a).z/(b).z );
			}
			return a;
		}

		public static string GetCompareString(CompareMethod cm){

			if (cm == CompareMethod.EqualTo)
				return " == ";

			if (cm == CompareMethod.GreaterThan)
				return " > ";

			if (cm == CompareMethod.LessThan)
				return " < ";

			if (cm == CompareMethod.GreaterOrEqualTo)
				return " >= ";

			if (cm == CompareMethod.LessOrEqualTo)
				return " <= ";

			return string.Empty;
		}

		public static bool Compare(float a, float b, CompareMethod cm, float floatingPoint){
			if (cm == CompareMethod.EqualTo)
				return Mathf.Abs(a - b) <= floatingPoint;
			if (cm == CompareMethod.GreaterThan)
				return a > b;
			if (cm == CompareMethod.LessThan)
				return a < b;
			if (cm == CompareMethod.GreaterOrEqualTo)
				return a >= b;
			if (cm == CompareMethod.LessOrEqualTo)
				return a <= b;
			return true;
		}

		public static bool Compare(int a, int b, CompareMethod cm){
			if (cm == CompareMethod.EqualTo)
				return a == b;
			if (cm == CompareMethod.GreaterThan)
				return a > b;
			if (cm == CompareMethod.LessThan)
				return a < b;
			if (cm == CompareMethod.GreaterOrEqualTo)
				return a >= b;
			if (cm == CompareMethod.LessOrEqualTo)
				return a <= b;
			return true;
		}
	}
}