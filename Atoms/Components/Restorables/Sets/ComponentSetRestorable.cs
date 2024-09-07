using System;
using System.Collections.Generic;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Sets
{
    /// <summary>
    /// Abstract class that implements the IComponentSetRestorable interface for a set of components that can be restored.
    /// </summary>
    /// <typeparam name="T">The type of JSONStorable component.</typeparam>
    public abstract class ComponentSetRestorable<T> : IComponentSetRestorable where T : JSONStorable
    {
        private JSONClass _restorable;
        protected Atom ContainingAtom;

        /// <summary>
        /// Gets or sets the name of the restorable set of components.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Gets or sets a custom predicate to filter components of type T.
        /// </summary>
        public Func<T, bool> CustomTypePredicate { get; set; }

        /// <summary>
        /// Gets or sets the dictionary of restorable components.
        /// </summary>
        public virtual Dictionary<string, IComponentRestorable> Storables { get; protected set; }

        /// <summary>
        /// Gets or sets the JSON representation of the set's restorable state.
        /// </summary>
        public virtual JSONClass Restorable {
            get {
                if (_restorable != null && !_restorable.Equals(new JSONClass()))
                    return _restorable;
                _restorable = restorableFromStorables();
                return _restorable;
            }
            set { _restorable = value; }
        }

        /// <summary>
        /// Initializes a new instance of the ComponentSetRestorable class.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        public ComponentSetRestorable(Atom atom)
        {
            ContainingAtom = atom;
            Name = $"{typeof(T)}:{atom.name}";
        }

        /// <summary>
        /// Creates a JSON representation from the storables.
        /// </summary>
        /// <returns>The JSON representation of the storables.</returns>
        /// <exception cref="Exception">Thrown when a component fails to restore.</exception>
        protected JSONClass restorableFromStorables()
        {
            JSONClass result = new JSONClass();
            JSONArray items = new JSONArray();
            foreach (KeyValuePair<string, IComponentRestorable> cmo in Storables) {
                try {
                    items.Add(cmo.Value?.Restorable);
                }
                catch (Exception e) {
                    throw new Exception($"Failed to restore: {cmo.Key}: {e}");
                }
            }
            result.Add("storables", items);
            Restorable = items.Count > 0 ? result : new JSONClass();
            return Restorable;
        }

        /// <summary>
        /// Gets the count of components in the set.
        /// </summary>
        public virtual int Count {
            get {
                return Restorable.Count;
            }
        }

        /// <summary>
        /// Default predicate to filter components of type T.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item matches the predicate; otherwise, false.</returns>
        protected virtual bool defaultTypePredicate(T item)
        {
            return true;
        }

        /// <summary>
        /// Initializes the set of components.
        /// </summary>
        public virtual void Init()
        {
            Storables = new Dictionary<string, IComponentRestorable>();
            Restorable = null;
        }

        /// <summary>
        /// Updates the set's state.
        /// </summary>
        /// <returns>The updated JSON representation of the set's state.</returns>
        public virtual JSONClass Update()
        {
            return Update(ContainingAtom);
        }

        /// <summary>
        /// Updates the set's state with the specified Atom.
        /// </summary>
        /// <param name="atom">The Atom to update from.</param>
        /// <returns>The updated JSON representation of the set's state.</returns>
        public abstract JSONClass Update(Atom atom);

        /// <summary>
        /// Restores the set's state.
        /// </summary>
        public virtual void Restore()
        {
            Restore(ContainingAtom);
        }

        /// <summary>
        /// Restores the set's state with the specified Atom.
        /// </summary>
        /// <param name="atom">The Atom to restore to.</param>
        /// <exception cref="Exception">Thrown when a component fails to restore.</exception>
        public virtual void Restore(Atom atom = null)
        {
            foreach (KeyValuePair<string, IComponentRestorable> cmo in Storables) {
                try {
                    if (atom == null) cmo.Value.Restore();
                    else cmo.Value.Restore(atom);
                }
                catch (Exception e) {
                    throw new Exception($"Failed to restore: {cmo.Key}: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Gets the JSON representation of the set.
        /// </summary>
        /// <param name="includePhysical">Whether to include physical properties.</param>
        /// <param name="includeAppearance">Whether to include appearance properties.</param>
        /// <param name="forceStore">Whether to force storing the properties.</param>
        /// <returns>The JSON representation of the set.</returns>
        public virtual JSONClass GetJSON(bool includePhysical = false, bool includeAppearance = true, bool forceStore = false)
        {
            return Restorable;
        }

        /// <summary>
        /// Restores the set's state from the specified JSON representation.
        /// </summary>
        /// <param name="jc">The JSON representation to restore from.</param>
        /// <param name="restorePhysical">Whether to restore physical properties.</param>
        /// <param name="restoreAppearance">Whether to restore appearance properties.</param>
        /// <param name="presetAtoms">The preset atoms to use for restoration.</param>
        /// <param name="setMissingToDefault">Whether to set missing properties to default values.</param>
        public virtual void RestoreFromJSON(JSONClass jc, bool restorePhysical = false, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
        {
            Restorable = jc;
            Restore();
        }
    }
}
