using System;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using EntityCX.Atoms.Restorables.Sets;

namespace EntityCX.Atoms.Restorables.Appearance
{
    #region Set of Storables
    /// <summary>
    /// Implements the ComponentSetRestorable class for components that manage appearance-related properties.
    /// </summary>
    /// <typeparam name="T">The type of JSONStorable component.</typeparam>
    public class AppearanceSetRestorable<T> : ComponentSetRestorable<T> where T : JSONStorable
    {
        /// <summary>
        /// Initializes a new instance of the AppearanceSetRestorable class.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        public AppearanceSetRestorable(Atom atom) : base(atom) { }

        /// <summary>
        /// Default predicate to filter components of type T based on appearance lock.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>True if the item's appearance is not locked; otherwise, false.</returns>
        protected override bool defaultTypePredicate(T item)
        {
            return !item.appearanceLocked;
        }

        /// <summary>
        /// Updates the set's state with the specified Atom.
        /// </summary>
        /// <param name="atom">The Atom to update from.</param>
        /// <returns>The updated JSON representation of the set's state.</returns>
        /// <exception cref="Exception">Thrown when the update fails.</exception>
        public override JSONClass Update(Atom atom)
        {
            EntityUtils.Logger.Message($"We made it {typeof(T)}");
            atom = atom ?? ContainingAtom;

            T[] qualifying = new T[0];
            Func<T, bool> tmpPredicate = CustomTypePredicate ?? defaultTypePredicate;

            try {
                if (CustomTypePredicate == null) {
                    qualifying = atom.GetComponentsInChildren<T>().Where(defaultTypePredicate).OrderByDescending(s => -s.name.Length).ToArray();
                }
                else {
                    qualifying = atom.GetComponentsInChildren<T>().Where(CustomTypePredicate).OrderByDescending(s => -s.name.Length).ToArray();
                }
            }
            catch (Exception e) {
                throw new Exception($"Failure getting components of type {typeof(T)} w/ Predicate {tmpPredicate}: {e.Message}");
            }

            if (Storables == null)
                Init();
            else
                Storables.Clear();

            foreach (T cmo in qualifying) {
                try {
                    AppearanceRestorable<T> aStorable = new AppearanceRestorable<T>(atom, cmo);
                    aStorable.Init();
                    aStorable.Update();
                    Storables.Add($"{cmo.name} {cmo.storeId}", aStorable);
                }
                catch (Exception e) {
                    foreach (KeyValuePair<string, IComponentRestorable> cm in Storables) {
                        SuperController.LogError($"({cm.Key})");
                    }
                    throw new Exception($"Failed to set restorable: ({Storables}){cmo.name} {cmo.storeId}: {e.Message}");
                }
            }
            return Restorable;
        }

        /// <summary>
        /// Gets the JSON representation of the set.
        /// </summary>
        /// <param name="includePhysical">Whether to include physical properties.</param>
        /// <param name="includeAppearance">Whether to include appearance properties.</param>
        /// <param name="forceStore">Whether to force storing the properties.</param>
        /// <returns>The JSON representation of the set.</returns>
        public override JSONClass GetJSON(bool includePhysical = false, bool includeAppearance = true, bool forceStore = false)
        {
            SuperController.LogError("not supposed to be here yet");
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
        public override void RestoreFromJSON(JSONClass jc, bool restorePhysical = false, bool restoreAppearance = true, JSONArray presetAtoms = null, bool setMissingToDefault = true)
        {
            SuperController.LogError("not supposed to be here yet");
            Restorable = jc;
            Restore();
        }
    }
    #endregion
}
