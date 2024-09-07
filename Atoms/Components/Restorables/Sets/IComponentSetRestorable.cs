using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Sets
{
    #region Set of Restorables
    /// <summary>
    /// Defines the interface for a set of components that can be restored;
    /// in this case, from a JSONClass object, and updated from an Atom.
    /// </summary>
    public interface IComponentSetRestorable
    {
        /// <summary>
        /// Gets the name of the restorable set of components.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the JSON representation of the set's restorable state.
        /// </summary>
        JSONClass Restorable { get; }

        /// <summary>
        /// Initializes the set of components.
        /// </summary>
        void Init();

        /// <summary>
        /// Updates the set's state; either from the state of a Storable 
        /// if it exists, or from an Atom argument.
        /// </summary>
        /// <param name="a">An optional Atom parameter from which to update.</param>
        /// <returns>The updated JSON representation of the set's state.</returns>
        JSONClass Update(Atom a = null);

        /// <summary>
        /// Restores the set's state; either to an implemented storable, or as a 
        /// new/existing set of components of an Atom argument.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the restoration.</param>
        void Restore(Atom a = null);

        /// <summary>
        /// Gets the count of components in the set.
        /// </summary>
        int Count { get; }
    }
    #endregion
}