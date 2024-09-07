using System.Collections.Generic;
using EntityCX.Atoms.Restorables.Sets;
using EntityCX.EntityUtils;
using SimpleJSON;

namespace EntityCX.Atoms.Restorables.Collections
{
    /// <summary>
    /// Manages a collection of standalone restorables and restorable sets.
    /// Implements the IComponentSetRestorable interface.
    /// </summary>
    public class ComponentRestorableCollection : IComponentSetRestorable
    {
        private JSONClass _restorable;

        /// <summary>
        /// Gets a value indicating whether to include physical properties.
        /// </summary>
        protected virtual bool IncludePhysical => true;

        /// <summary>
        /// Gets a value indicating whether to include appearance properties.
        /// </summary>
        protected virtual bool IncludeAppearance => true;

        /// <summary>
        /// Gets a value indicating whether to force storing the properties.
        /// </summary>
        protected virtual bool ForceStore => false;

        /// <summary>
        /// Gets the containing Atom.
        /// </summary>
        public Atom ContainingAtom { get; }

        /// <summary>
        /// Gets the list of standalone restorables.
        /// </summary>
        public List<IComponentRestorable> RestorableCollection { get; private set; } = new List<IComponentRestorable>();

        /// <summary>
        /// Gets the list of restorable sets.
        /// </summary>
        public List<IComponentSetRestorable> RestorableSetCollection { get; private set; } = new List<IComponentSetRestorable>();

        /// <summary>
        /// Gets the total count of restorables in the collection.
        /// </summary>
        public virtual int Count => RestorableCollection.Count + RestorableSetCollection.Count;

        /// <summary>
        /// Gets the name of the restorable collection.
        /// </summary>
        public virtual string Name => ContainingAtom.name + ":ComponentRestorableSet";

        /// <summary>
        /// Gets or sets the JSON representation of the collection's restorable state.
        /// </summary>
        public virtual JSONClass Restorable {
            get {
                return _restorable ?? GetJSON();
            }
            protected set {
                _restorable = value;
            }
        }

        #region Construct
        /// <summary>
        /// Initializes a new instance of the ComponentRestorableCollection class.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        public ComponentRestorableCollection(Atom atom)
        {
            ContainingAtom = atom;
            Init();
        }
        #endregion

        #region Init
        /// <summary>
        /// Initializes the collection.
        /// </summary>
        public virtual void Init()
        {
            // Initialization logic here
        }
        #endregion

        #region Build
        /// <summary>
        /// Adds a standalone restorable to the collection.
        /// </summary>
        /// <typeparam name="T">The type of JSONStorable component.</typeparam>
        /// <param name="attribute">The restorable component to add.</param>
        public void AddRestorable<T>(ComponentRestorable<T> attribute) where T : JSONStorable
        {
            RestorableCollection.Add(attribute);
        }

        /// <summary>
        /// Adds a set of restorables to the collection.
        /// </summary>
        /// <typeparam name="T">The type of JSONStorable component.</typeparam>
        /// <param name="attribute">The set of restorables to add.</param>
        public void AddRestorables<T>(ComponentSetRestorable<T> attribute) where T : JSONStorable
        {
            foreach (KeyValuePair<string, IComponentRestorable> cmo in attribute.Storables) {
                cmo.Value.Init();
                cmo.Value.Update();
            }
            RestorableSetCollection.Add(attribute);
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the collection's state.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the update.</param>
        /// <returns>The updated JSON representation of the collection's state.</returns>
        public virtual JSONClass Update(Atom a = null)
        {
            Logger.Message("Not called, ever, in my app");
            foreach (IComponentSetRestorable attribute in RestorableSetCollection) {
                attribute.Update();
            }
            foreach (IComponentRestorable attribute in RestorableCollection) {
                attribute.Update();
            }
            return Restorable;
        }
        #endregion

        #region Restore
        /// <summary>
        /// Restores the collection's state.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the restoration.</param>
        public virtual void Restore(Atom a = null)
        {
            foreach (IComponentRestorable attribute in RestorableCollection) {
                attribute.Restore();
            }
            foreach (IComponentSetRestorable attributes in RestorableSetCollection) {
                attributes.Restore();
            }
        }
        #endregion

        /// <summary>
        /// Gets the JSON representation of the collection.
        /// </summary>
        /// <param name="jsonBase">An optional base JSON object to build upon.</param>
        /// <returns>The JSON representation of the collection.</returns>
        public virtual JSONClass GetJSON(JSONClass jsonBase = null)
        {
            JSONClass jclass = jsonBase ?? new JSONClass(){
                {
                "storables", new JSONArray {new JSONClass{ }}
                }
            };
            foreach (IComponentRestorable attribute in RestorableCollection) {
                if (attribute.Restorable != null) {
                    jclass["storables"].Add(attribute.Restorable);
                }
            }
            foreach (IComponentSetRestorable attributes in RestorableSetCollection) {
                if (attributes.Restorable?["storables"]?.Count > 0) {
                    foreach (JSONClass attributeJSON in attributes.Restorable["storables"] as JSONArray) {
                        jclass["storables"].Add(attributeJSON);
                    }
                }
            }
            return jclass;
        }
    }
}
