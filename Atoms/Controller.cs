using System;
using System.Collections;

namespace EntityCX.Atoms
{
    public static class Util
    {

        public static IEnumerator Create(string type, string uid, Action<string> onCreate)
        {
            var superController = SuperController.singleton;
            if (superController.GetAtomByUid(uid) != null) {
                onCreate(uid);
                yield break;
            }

            yield return superController.AddAtomByType(type, uid, true);
            var atom = superController.GetAtomByUid(uid);
            if (atom == null) {
                onCreate("N/A");
                yield break;
            }
            superController.RenameAtom(atom, uid);
            onCreate(atom.uid);
        }
        public static IEnumerator WaitForCreate()
        {

            yield break;
        }
    }
}