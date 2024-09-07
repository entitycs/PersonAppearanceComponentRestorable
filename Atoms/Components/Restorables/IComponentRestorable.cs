using SimpleJSON;

namespace EntityCX.Atoms.Restorables
{
    #region Single Restorable
    /// <summary>
    /// Defines the interface for a component that can be restored;
    /// in this case, from a JSONClass object, and updated from an Atom.
    /// </summary>
    public interface IComponentRestorable
    {
        /// <summary>
        /// Gets the name of the restorable component.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a value indicating whether the component is standalone, or part of a set of the same type.
        /// </summary>
        bool Standalone { get; }

        /// <summary>
        /// Gets the JSON representation of the component's restorable state.
        /// </summary>
        JSONClass Restorable { get; }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        void Init();

        /// <summary>
        /// Updates the component's state; either from the state of a Storable 
        /// if it exists, or from an Atom argument.
        /// </summary>
        /// <param name="a">An optional Atom parameter from which to update.</param>
        /// <returns>The updated JSON representation of the component's state.</returns>
        JSONClass Update(Atom a = null);

        /// <summary>
        /// Restores the component's state; either to an implemented Storable, or as a 
        /// new/existing component of an Atom argument.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the restoration.</param>
        void Restore(Atom a = null);
    }
    #endregion
}
