namespace ParadoxNotion
{
    public static class ObjectUtils
    {
        ///Equals and ReferenceQuals check with added special treat for Unity Objects
		public static bool TrueEquals(object a, object b) {

            //regardless calling ReferenceEquals, unity is still doing magic and this is the only true solution (I've found)
            if ( a is UnityEngine.Object || b is UnityEngine.Object ) {
                return a as UnityEngine.Object == b as UnityEngine.Object;
            }

            return object.Equals(a, b) || object.ReferenceEquals(a, b);
        }
    }
}