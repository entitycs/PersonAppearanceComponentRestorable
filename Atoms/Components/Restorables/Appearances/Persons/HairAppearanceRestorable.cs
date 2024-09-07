using System.Collections.Generic;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Appearance.Person
{
    #region Specialized Storables
    /// <summary>
    /// Represents a set of restorables for hair appearance.
    /// </summary>
    /// <typeparam name="T">The type of JSON storable.</typeparam>
    public class HairAppearanceRestorables<T> : AppearanceSetRestorable<T> where T : JSONStorable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HairAppearanceRestorables{T}"/> class.
        /// </summary>
        /// <param name="person">The atom associated with this restorable.</param>
        public HairAppearanceRestorables(Atom person) : base(person)
        {
            ContainingAtom = person;
        }

        /// <summary>
        /// Updates the hair appearance restorables.
        /// </summary>
        /// <param name="atom">The atom to update. If null, the containing atom is used.</param>
        /// <returns>A JSON class representing the updated state.</returns>
        public override JSONClass Update(Atom atom = null)
        {
            EntityUtils.Logger.Message($"We made it {typeof(T)}");
            atom = atom ?? ContainingAtom;
            if (Storables == null)
                Init();
            else
                Storables.Clear();
            foreach (DAZHairGroupControl hairGroup in atom.GetComponentsInChildren<DAZHairGroupControl>()) {
                foreach (T cmo in hairGroup.GetComponentsInChildren<T>()) {
                    Storables.Add($"{cmo.name} {cmo.storeId}", new AppearanceRestorable<T>(atom, cmo));
                }
            }
            foreach (DAZHairMesh hairGroup in atom.GetComponentsInChildren<DAZHairMesh>()) {
                foreach (T cmo in hairGroup.GetComponentsInChildren<T>()) {
                    Storables.Add($"{cmo.name} {cmo.storeId}", new AppearanceRestorable<T>(atom, cmo));
                }
            }
            return Restorable;
        }

        /// <summary>
        /// Restores the hair appearance from JSON data.
        /// </summary>
        /// <param name="atom">The atom to restore. If null, the containing atom is used.</param>
        /// <exception cref="System.Exception">Thrown when the restoration fails.</exception>
        public override void Restore(Atom atom = null)
        {
            foreach (KeyValuePair<string, IComponentRestorable> cmo in Storables) {
                try {
                    SuperController.LogMessage(cmo.Value.GetType().ToString());
                    if (null == atom)
                        cmo.Value.Restore();
                    else
                        cmo.Value.Restore(atom);
                }
                catch (System.Exception e) {
                    throw new System.Exception($"Failed to restore: {cmo.Key}: {e.Message}");
                }
            }
        }
    }
    #endregion
}
