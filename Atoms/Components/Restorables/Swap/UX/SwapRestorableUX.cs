using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace EntityCX.Atoms.Restorables.Swap
{
    /// <summary>
    /// Manages the user experience for swapping atoms.
    /// </summary>
    public class SwapUX
    {
        private readonly MVRScript _parentScript;
        private string _swapCanvasUid = string.Empty;
        private int _numSwappable = 0;
        private Atom _swappableSigTail;
        public Atom SwapTarget { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SwapUX"/> class.
        /// </summary>
        /// <param name="atom">The target atom for swapping.</param>
        /// <param name="parentScript">The parent script managing this instance.</param>
        public SwapUX(Atom atom, MVRScript parentScript)
        {
            SwapTarget = atom;
            _parentScript = parentScript;
        }

        /// <summary>
        /// Creates a new atom asynchronously.
        /// </summary>
        /// <param name="type">The type of the atom to create.</param>
        /// <param name="uid">The unique identifier for the new atom.</param>
        /// <param name="onCreate">The action to perform once the atom is created.</param>
        /// <returns>An enumerator for the coroutine.</returns>
        /// <exception cref="ArgumentException">Thrown when an atom with the specified UID already exists.</exception>
        public static IEnumerator CreateAtom(string type, string uid, Action<Atom> onCreate)
        {
            var superController = SuperController.singleton;
            if (superController.GetAtomByUid(uid) != null)
                throw new ArgumentException($"Atom w/ uid {uid} already exists");
            else
                yield return superController.AddAtomByType(type, uid, true);
            onCreate(superController.GetAtomByUid(uid));
            yield return null;
        }

        /// <summary>
        /// Waits for the atom creation or aborts after a timeout.
        /// </summary>
        /// <returns>An enumerator for the coroutine.</returns>
        private IEnumerator WaitForCreateOrAbort()
        {
            yield return new WaitForSeconds(10);
            _swapCanvasUid = "N/A";
        }

        /// <summary>
        /// Adds a swappable user experience element.
        /// </summary>
        public void AddSwappableUX()
        {
            _parentScript.StartCoroutine(Atoms.Util.Create("ISSphere", $"{SwapTarget.name}_Swapper_Sphere{_numSwappable}", atomUid => {

                if (atomUid.Equals("N/A")) return;
                var superController = SuperController.singleton;
                var swapCanvasAtom = superController.GetAtomByUid(_swapCanvasUid);

                var swapSphereController = superController.GetAtomByUid(atomUid).GetStorableByID("control") as FreeControllerV3;
                var swapCanvasController = swapCanvasAtom.GetStorableByID("control") as FreeControllerV3;

                swapSphereController.containingAtom.transform.SetParent(swapCanvasAtom.transform, false);
                swapSphereController.containingAtom.transform.SetPositionAndRotation(swapCanvasController.transform.position, swapCanvasController.transform.rotation);

                swapSphereController.SelectLinkToRigidbody(swapCanvasAtom.GetComponentsInChildren<Rigidbody>().First());
                swapSphereController.containingAtom.reParentObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

                _swappableSigTail = swapSphereController.containingAtom;
                _numSwappable++;
            }));
        }

        /// <summary>
        /// Asynchronously creates a new UI toggle element.
        /// </summary>
        /// <param name="link">The rigidbody to link the UI toggle to.</param>
        /// <param name="parent">The parent transform for the new UI toggle.</param>
        /// <returns>An enumerator for the coroutine.</returns>
        public IEnumerator AsyncCreate(Rigidbody link, Transform parent)
        {
            var superController = SuperController.singleton;

            yield return _parentScript.StartCoroutine(CreateAtom("UIToggle", $"{SwapTarget.name}_Swapper", toggleCanvas => {

                var toggleUI = toggleCanvas.GetStorableByID("control") as FreeControllerV3;

                toggleCanvas.transform.SetParent(parent, true);
                toggleCanvas.transform.SetPositionAndRotation(parent.position, parent.rotation);
                toggleCanvas.transform.localPosition = new Vector3(0f, 0.25f + (0.1f * _numSwappable), 0f);

                toggleUI.SelectLinkToRigidbody(link);
                _swapCanvasUid = toggleCanvas.uid;
            }));
            yield return new WaitForSecondsRealtime(10f);
            yield return new WaitWhile(() => string.IsNullOrEmpty(_swapCanvasUid));
            AddSwappableUX();
            yield return null;
        }
    }
}
