using System;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables
{
    public abstract class ComponentRestorable<T> : IComponentRestorable where T : JSONStorable
    {
        private JSONClass _restorable;
        private T _storable;
        protected Atom ContainingAtom { get; set; }
        public virtual string Name { get; protected set; }
        public bool Standalone { get; protected set; }
        /// <summary>
        /// Gets or sets the storable component.
        /// </summary>
        /// <exception cref="MemberAccessException">Thrown when the reference to the component in the set is lost.</exception>
        public T Storable {
            get {
                if (null != _storable) {
                    return _storable;
                }
                else if (Standalone) {
                    return GetAssuredComponent(ContainingAtom);
                }
                else {
                    throw new MemberAccessException($"{nameof(Storable)}: Reference to component in set of type {typeof(T)} was lost (null)");
                }
            }
            protected set {
                if (null != value)
                    _storable = value;
            }
        }
        /// <summary>
        /// Gets or sets the JSON representation of the component's restorable state.
        /// </summary>
        public virtual JSONClass Restorable {
            get {
                return (_restorable) ?? (Storable?.GetJSON());
            }
            set { _restorable = value; }
        }
        /// <summary>
        /// Ensures the storable component is present in the Atom and returns it.
        /// </summary>
        /// <param name="a">The Atom to get the component from.</param>
        /// <returns>The assured component.</returns>
        /// <exception cref="Exception">Thrown when the Atom or component is null.</exception>
        protected T GetAssuredComponent(Atom a)
        {
            T result;
            a = a ?? ContainingAtom;
            result = a.GetComponent<T>();
            if (result) {
                EntityUtils.Logger.Message("Assured GetC");
            }
            else if (result = a.GetComponentInChildren<T>()) {
                EntityUtils.Logger.Message("Assured GetInCh");
            }
            else if (result = a.gameObject.AddComponent<T>()) {
                EntityUtils.Logger.Message("Assured Add");
            }
            if (null == ContainingAtom || null == result)
                throw new System.Exception("Failed to get component. Atom cannot be null");
            return result;
        }
        /// <summary>
        /// Constructs a new instance of the ComponentRestorable class, after retrieving
        /// a storable of the generic type from the Atom parameter.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        /// <exception cref="Exception">Thrown when the Atom is null.</exception>
        public ComponentRestorable(Atom atom)
        {
            ContainingAtom = atom;
            if (null == ContainingAtom) throw new Exception("AppearanceRestorable: Failed to initialize. Atom cannot be null");
            Storable = GetAssuredComponent(atom);
            Init(Storable);
            Update();
        }
        /// <summary>
        /// Constructs a new instance of the ComponentRestorable class, with the given storable
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        /// <exception cref="Exception">Thrown when the Atom is null.</exception>
        public ComponentRestorable(Atom atom, T storable)// used by ComponentsRestorable, to diff. gathering "by type" ()
        {
            ContainingAtom = atom;
            Standalone = false;
            Storable = storable;
            if (null == ContainingAtom) throw new Exception("AppearanceRestorable: Failed to initialize. Atom cannot be null");
            Init(Storable);
            Update();
        }
        /// <summary>
        /// Sets the name of the restorable component.
        /// </summary>
        protected virtual void setName()
        {
            Name = $"{Storable.name}:{Storable.storeId}";
        }
        /// <summary>
        /// Initializes the component.
        /// </summary>
        public virtual void Init()
        {
            Init(Storable);
        }
        /// <summary>
        /// Initializes the component with the specified storable.
        /// </summary>
        /// <param name="storable">The storable component.</param>
        public virtual void Init(T storable)
        {
            Storable = storable ?? GetAssuredComponent(ContainingAtom); ;
            Restorable = null;
            setName();
        }
        /// <summary>
        /// Updates the component's state.
        /// </summary>
        /// <returns>The updated JSON representation of the component's state.
        public virtual JSONClass Update()
        {
            if (null != Storable) {
                Restorable = Storable.GetJSON();
                return Restorable;
            }
            return Update(ContainingAtom);
        }
        /// <summary>
        ///  Updates the component's state from an Atom argument.
        /// </summary>
        /// <param name="a">The Atom to update from.</param>
        /// <returns>The updated JSON representation of the component's state.</returns>
        /// <exception cref="AccessViolationException">Thrown when the update fails.</exception>
        public virtual JSONClass Update(Atom a)
        {
            try {// suspect
                a = a ?? ContainingAtom;
                Storable = GetAssuredComponent(a);
                Init(Storable);
                Restorable = Storable?.GetJSON();
            }
            catch (Exception e) {
                throw new AccessViolationException($"{nameof(IComponentRestorable)} Failed to update: {e.Message}");
            }
            return Restorable;
        }
        /// <summary>
        /// Restores the component's state from implemented Storable.
        /// </summary>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public virtual void Restore()
        {
            try {
                Storable.RestoreFromJSON(Restorable);
            }
            catch (Exception e) {
                throw new AccessViolationException($"{Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }
        /// <summary>
        /// Restores the component's state, as a component of atom.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the restoration.</param>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public virtual void Restore(Atom atom = null)
        {
            atom = atom ?? ContainingAtom;
            try {
                atom?.RestoreFromJSON(Restorable);
            }
            catch (Exception e) {
                throw new AccessViolationException($"{Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }
    }
}