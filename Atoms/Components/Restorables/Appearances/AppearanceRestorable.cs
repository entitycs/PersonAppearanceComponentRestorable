using System;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Appearance
{
    #region Single Storable
    public class AppearanceRestorable<T> : ComponentRestorable<T> where T : JSONStorable
    {
        private JSONClass _restorable;
        public override JSONClass Restorable {
            get {
                return (_restorable) ?? (Storable?.GetJSON(false, true));
            }
            set { _restorable = value; }
        }
        public AppearanceRestorable(Atom atom, T storable) : base(atom, storable) { }
        public AppearanceRestorable(Atom atom) : base(atom) { }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Init()
        {
            Storable = Storable ?? GetAssuredComponent(ContainingAtom);
            Restorable = null;
            Restorable = Storable.GetJSON(false, true);
            setName();
        }
        /// <summary>
        ///  Updates the component's state from implemented Storable.
        /// </summary>
        /// <param name="a">The Atom to update from.</param>
        /// <returns>The updated JSON representation of the component's state.</returns>
        /// <exception cref="AccessViolationException">Thrown when the update fails.</exception>
        public override JSONClass Update()
        {
            try {// suspect             
                if (null != Storable) {
                    Restorable = Storable.GetJSON(false, true);
                }
                else {
                    Init();
                }
            }
            catch (Exception e) {
                throw new AccessViolationException($"{nameof(AppearanceRestorable<T>)} Failed to update: {e.Message}");
            }
            return Restorable;
        }
        /// <summary>
        ///  Updates the component's state from an Atom argument.
        /// </summary>
        /// <param name="a">The Atom to update from.</param>
        /// <returns>The updated JSON representation of the component's state.</returns>
        /// <exception cref="AccessViolationException">Thrown when the update fails.</exception>
        public override JSONClass Update(Atom a)
        {
            try {// suspect
                a = a ?? ContainingAtom;
                Storable = GetAssuredComponent(a);
                Init(Storable);
                Restorable = Storable?.GetJSON(false, true);
            }
            catch (Exception e) {
                throw new AccessViolationException($"{nameof(IComponentRestorable)} Failed to update: {e.Message}");
            }
            return Restorable;
        }
        /// <summary>
        /// Restores the component's state (appearance only) from implemented Storable.
        /// </summary>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public override void Restore()
        {
            try {
                Storable.RestoreFromJSON(Restorable, false, true, null, false);
            }
            catch (Exception e) {
                throw new AccessViolationException($"AppearanceStorable() {Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }
        /// <summary>
        /// Restores the component's state (appearance only), as a component of atom.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the restoration.</param>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public override void Restore(Atom atom = null)
        {
            atom = atom ?? ContainingAtom;
            try {
                atom.RestoreFromJSON(Restorable, false, true, null, true);
            }
            catch (Exception e) {
                throw new AccessViolationException($"AppearanceStorable(Atom) {Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }
    }
    #endregion
}