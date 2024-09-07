using System;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Appearance.Person
{
    #region Single Storable
    /// <summary>
    /// Represents a restorable appearance for a person.
    /// </summary>
    /// <typeparam name="T">The type of JSON storable.</typeparam>
    public class PersonAppearanceRestorable<T> : AppearanceRestorable<T> where T : JSONStorable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PersonAppearanceRestorable{T}"/> class.
        /// </summary>
        /// <param name="atom">The atom associated with this restorable.</param>
        /// <param name="storable">The storable object.</param>
        public PersonAppearanceRestorable(Atom atom, T storable) : base(atom, storable) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="PersonAppearanceRestorable{T}"/> class.
        /// </summary>
        /// <param name="atom">The atom associated with this restorable.</param>
        public PersonAppearanceRestorable(Atom atom) : base(atom) { }

        /// <summary>
        /// Restores the appearance from JSON data.
        /// </summary>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public override void Restore()
        {
            try {
                if (typeof(T) == typeof(DAZCharacterSelector)) {
                    DAZCharacterSelector tmp = ContainingAtom.GetStorableByID("geometry") as DAZCharacterSelector;
                    tmp.RestoreFromJSON(
                        new JSONClass {
                            {"id",  "geometry"},
                            {"clothing", new JSONArray()},
                            {"hair", new JSONArray()},
                            {"morphs", new JSONArray()}
                        }, false, true, null, false
                    );
                    Storable.RestoreFromJSON(Restorable, false, true, null, false);
                }
                else {
                    Storable.RestoreFromJSON(Restorable, false, true, null, false);
                }
            }
            catch (Exception e) {
                throw new AccessViolationException($"PersonAppearanceStorable() {Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }

        /// <summary>
        /// Restores the appearance from JSON data for a specific person.
        /// </summary>
        /// <param name="person">The person whose appearance is to be restored.</param>
        /// <exception cref="AccessViolationException">Thrown when the restoration fails.</exception>
        public override void Restore(Atom person = null)
        {
            try {
                if (typeof(T) == typeof(DAZCharacterSelector)) {
                    person?.RestoreFromJSON(Restorable, false, true, null, true);
                }
                else {
                    person?.RestoreFromJSON(Restorable, false, true, null, true);
                }
            }
            catch (Exception e) {
                throw new AccessViolationException($"PersonAppearanceStorable(p) {Name} {typeof(T)} failed to restore: {e.Message}");
            }
        }
    }
    #endregion
}
