using System;
using System.Collections.Generic;
using UnityEngine;


namespace NodeCanvas.Framework {

    /// An interface for Blackboards, or otherwise for a Variables container.
    [ParadoxNotion.Design.SpoofAOT]
    public interface IBlackboard
    {
        event Action<Variable> onVariableAdded;
        event Action<Variable> onVariableRemoved;

        string name { get; set; }
        Dictionary<string, Variable> variables { get; set; }
        GameObject propertiesBindTarget { get; }

        Variable AddVariable(string varName, Type type);
        Variable AddVariable(string varName, object value);
        Variable RemoveVariable(string varName);
        Variable GetVariable(string varName, Type ofType = null);
        Variable GetVariableByID(string ID);
        Variable<T> GetVariable<T>(string varName);
        T GetValue<T>(string varName);
        Variable SetValue(string varName, object value);
        string[] GetVariableNames();
        string[] GetVariableNames(Type ofType);
    }
}