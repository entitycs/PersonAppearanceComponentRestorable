using EntityCX.Atoms.Restorables.Collections;

namespace EntityCX.Atoms.Restorables.Appearance.Collections
{
    /// <summary>
    /// Manages a collection of appearance-related restorables.
    /// Extends the ComponentRestorableCollection class.
    /// </summary>
    public class AppearanceRestorableCollection : ComponentRestorableCollection
    {
        /// <summary>
        /// Gets a value indicating whether to include physical properties.
        /// </summary>
        protected override bool IncludePhysical => false;

        /// <summary>
        /// Initializes a new instance of the AppearanceRestorableCollection class.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        public AppearanceRestorableCollection(Atom atom) : base(atom) { }
    }
}
