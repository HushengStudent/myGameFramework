using System.Collections.Generic;

namespace ParadoxNotion
{

    public class HierarchyTree
    {

        ///A simple general purpose hierarchical element wrapper.
        public class Element
        {

            public object reference;
            public Element parent;
            public List<Element> children;
            public Element(object reference) {
                this.reference = reference;
            }

            ///Add a child element
            public void AddChild(Element child) {
                if ( children == null ) { children = new List<Element>(); }
                child.parent = this;
                children.Add(child);
            }

            ///Remove a child element
            public void RemoveChild(Element child) {
                if ( children != null ) {
                    children.Remove(child);
                }
            }

            ///Get root element
            public Element GetRoot() {
                var current = parent;
                while ( current != null ) {
                    current = current.parent;
                }
                return current;
            }

            ///Returns the first found Element that references target object
            public Element FindReferenceElement(object target) {
                if ( this.reference == target ) {
                    return this;
                }
                if ( children == null ) { return null; }
                for ( var i = 0; i < children.Count; i++ ) {
                    var sub = children[i].FindReferenceElement(target);
                    if ( sub != null ) {
                        return sub;
                    }
                }
                return null;
            }

            ///Get first parent reference of type T, including self element
            public T GetFirstParentReferenceOfType<T>() {
                if ( this.reference is T ) {
                    return (T)reference;
                }
                return parent != null ? parent.GetFirstParentReferenceOfType<T>() : default(T);
            }

            ///Get all children references of type T recursively
            public List<T> GetAllChildrenReferencesOfType<T>() {
                var result = new List<T>();
                if ( children == null ) { return result; }
                for ( var i = 0; i < children.Count; i++ ) {
                    var element = children[i];
                    if ( element.reference is T ) {
                        result.Add((T)element.reference);
                    }
                    result.AddRange(element.GetAllChildrenReferencesOfType<T>());
                }
                return result;
            }
        }
    }
}