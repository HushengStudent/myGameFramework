using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParadoxNotion.Serialization.FullSerializer
{

    /// The actual type that a JsonData instance can store.
    public enum fsDataType
    {
        Array,
        Object,
        Double,
        Int64,
        Boolean,
        String,
        Null
    }

    /// A union type that stores a serialized value. The stored type can be one of six different
    /// types: null, boolean, double, Int64, string, Dictionary, or List.
    public sealed class fsData
    {

        /// The raw value that this serialized data stores. It can be one of six different types; a
        /// boolean, a double, Int64, a string, a Dictionary, or a List.
        private object _value;

        public readonly static fsData True = new fsData(true);
        public readonly static fsData False = new fsData(false);
        public readonly static fsData Null = new fsData();

        public fsDataType Type {
            get
            {
                if ( _value == null ) return fsDataType.Null;
                if ( _value is double ) return fsDataType.Double;
                if ( _value is Int64 ) return fsDataType.Int64;
                if ( _value is bool ) return fsDataType.Boolean;
                if ( _value is string ) return fsDataType.String;
                if ( _value is Dictionary<string, fsData> ) return fsDataType.Object;
                if ( _value is List<fsData> ) return fsDataType.Array;
                throw new InvalidOperationException("unknown JSON data type");
            }
        }

        ///----------------------------------------------------------------------------------------------

        /// Creates a fsData instance that holds null.
        public fsData() {
            _value = null;
        }

        /// Creates a fsData instance that holds a boolean.
        public fsData(bool boolean) {
            _value = boolean;
        }

        /// Creates a fsData instance that holds a double.
        public fsData(double f) {
            _value = f;
        }

        /// Creates a new fsData instance that holds an integer.
        public fsData(Int64 i) {
            _value = i;
        }

        /// Creates a fsData instance that holds a string.
        public fsData(string str) {
            _value = str;
        }

        /// Creates a fsData instance that holds a dictionary of values.
        public fsData(Dictionary<string, fsData> dict) {
            _value = dict;
        }

        /// Creates a fsData instance that holds a list of values.
        public fsData(List<fsData> list) {
            _value = list;
        }

        ///----------------------------------------------------------------------------------------------     

        /// Helper method to create a fsData instance that holds a dictionary.
        public static fsData CreateDictionary() {
            return new fsData(new Dictionary<string, fsData>(
                fsGlobalConfig.IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase));
        }

        /// Helper method to create a fsData instance that holds a list.
        public static fsData CreateList() {
            return new fsData(new List<fsData>());
        }

        /// Helper method to create a fsData instance that holds a list with the initial capacity.
        public static fsData CreateList(int capacity) {
            return new fsData(new List<fsData>(capacity));
        }

        ///----------------------------------------------------------------------------------------------

        /// Transforms the internal fsData instance into a dictionary.
        internal void BecomeDictionary() {
            _value = new Dictionary<string, fsData>(StringComparer.Ordinal);
        }

        /// Returns a shallow clone of this data instance.
        internal fsData Clone() {
            var clone = new fsData();
            clone._value = _value;
            return clone;
        }

        ///----------------------------------------------------------------------------------------------

        /// Returns true if this fsData instance maps back to null.
        public bool IsNull {
            get { return _value == null; }
        }

        /// Returns true if this fsData instance maps back to a double.
        public bool IsDouble {
            get { return _value is double; }
        }

        /// Returns true if this fsData instance maps back to an Int64.
        public bool IsInt64 {
            get { return _value is Int64; }
        }

        /// Returns true if this fsData instance maps back to a boolean.
        public bool IsBool {
            get { return _value is bool; }
        }

        /// Returns true if this fsData instance maps back to a string.
        public bool IsString {
            get { return _value is string; }
        }

        /// Returns true if this fsData instance maps back to a Dictionary.
        public bool IsDictionary {
            get { return _value is Dictionary<string, fsData>; }
        }

        /// Returns true if this fsData instance maps back to a List.
        public bool IsList {
            get { return _value is List<fsData>; }
        }

        ///----------------------------------------------------------------------------------------------

        /// Casts this fsData to a double. Throws an exception if it is not a double.
        public double AsDouble {
            get { return Cast<double>(); }
        }

        /// Casts this fsData to an Int64. Throws an exception if it is not an Int64.
        public Int64 AsInt64 {
            get { return Cast<Int64>(); }
        }

        /// Casts this fsData to a boolean. Throws an exception if it is not a boolean.
        public bool AsBool {
            get { return Cast<bool>(); }
        }

        /// Casts this fsData to a string. Throws an exception if it is not a string.
        public string AsString {
            get { return Cast<string>(); }
        }

        /// Casts this fsData to a Dictionary. Throws an exception if it is not a
        /// Dictionary.
        public Dictionary<string, fsData> AsDictionary {
            get { return Cast<Dictionary<string, fsData>>(); }
        }

        /// Casts this fsData to a List. Throws an exception if it is not a List.
        public List<fsData> AsList {
            get { return Cast<List<fsData>>(); }
        }

        /// Internal helper method to cast the underlying storage to the given type or throw a
        /// pretty printed exception on failure.
        private T Cast<T>() {
            if ( _value is T ) { return (T)_value; }

            throw new InvalidCastException("Unable to cast <" + this + "> (with type = " +
                _value.GetType() + ") to type " + typeof(T));
        }

        ///----------------------------------------------------------------------------------------------

        public override string ToString() {
            return fsJsonPrinter.CompressedJson(this);
        }

        /// Determines whether the specified object is equal to the current object.
        public override bool Equals(object obj) {
            return Equals(obj as fsData);
        }

        /// Determines whether the specified object is equal to the current object.
        public bool Equals(fsData other) {
            if ( other == null || Type != other.Type ) {
                return false;
            }

            switch ( Type ) {
                case fsDataType.Null:
                    return true;

                case fsDataType.Double:
                    return AsDouble == other.AsDouble || Math.Abs(AsDouble - other.AsDouble) < double.Epsilon;

                case fsDataType.Int64:
                    return AsInt64 == other.AsInt64;

                case fsDataType.Boolean:
                    return AsBool == other.AsBool;

                case fsDataType.String:
                    return AsString == other.AsString;

                case fsDataType.Array:
                    var thisList = AsList;
                    var otherList = other.AsList;

                    if ( thisList.Count != otherList.Count ) return false;

                    for ( int i = 0; i < thisList.Count; ++i ) {
                        if ( thisList[i].Equals(otherList[i]) == false ) {
                            return false;
                        }
                    }

                    return true;

                case fsDataType.Object:
                    var thisDict = AsDictionary;
                    var otherDict = other.AsDictionary;

                    if ( thisDict.Count != otherDict.Count ) return false;

                    foreach ( string key in thisDict.Keys ) {
                        if ( otherDict.ContainsKey(key) == false ) {
                            return false;
                        }

                        if ( thisDict[key].Equals(otherDict[key]) == false ) {
                            return false;
                        }
                    }

                    return true;
            }

            throw new Exception("Unknown data type");
        }

        /// Returns true iff a == b.
        public static bool operator ==(fsData a, fsData b) {
            // If both are null, or both are same instance, return true.
            if ( ReferenceEquals(a, b) ) {
                return true;
            }

            // If one is null, but not both, return false.
            if ( ( (object)a == null ) || ( (object)b == null ) ) {
                return false;
            }

            if ( a.IsDouble && b.IsDouble ) {
                return Math.Abs(a.AsDouble - b.AsDouble) < double.Epsilon;
            }

            return a.Equals(b);
        }

        /// Returns true iff a != b.
        public static bool operator !=(fsData a, fsData b) {
            return !( a == b );
        }

        /// Returns a hash code for this instance.
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data
        /// structures like a hash table.</returns>
        public override int GetHashCode() {
            return _value.GetHashCode();
        }

    }

}