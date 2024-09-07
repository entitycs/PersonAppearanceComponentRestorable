using EntityCX.Atoms.Restorables.Appearance.Collections;
using EntityCX.Atoms.Restorables.Sets;
using EntityCX.EntityUtils;
using SimpleJSON;
using System.Collections.Generic;

namespace EntityCX.Atoms.Restorables.Appearance.Person.Sets
{
    /// <summary>
    /// Manages a collection of appearance-related restorables for a person.
    /// Extends the AppearanceRestorableCollection class.
    /// </summary>
    public class PersonAppearanceRestorableCollection : AppearanceRestorableCollection
    {
        private JSONClass _restorable;
        private DAZCharacterSelector _selector;

        /// <summary>
        /// Gets the keys for geometry-related properties.
        /// </summary>
        public string[] GeometryKeys { get; } = new string[] { "character", "clothing", "hair", "morphs" };

        /// <summary>
        /// Gets the restorable for the DAZCharacterSelector.
        /// </summary>
        public AppearanceRestorable<DAZCharacterSelector> Geometry { get; private set; }

        /// <summary>
        /// Gets the restorable set for the DAZCharacterMaterialOptions.
        /// </summary>
        public AppearanceSetRestorable<DAZCharacterMaterialOptions> CharacterMaterials { get; private set; }
        public override JSONClass Restorable {
            get {
                return _restorable ?? GetJSON();
            }
            protected set {
                _restorable = value;
            }
        }

        public override int Count {
            get {
                return base.Count + CharacterMaterials.Count + 1;// 1-Geometry
            }
        }
        #region Construct
        /// <summary>
        /// Initializes a new instance of the PersonAppearanceRestorableCollection class.
        /// </summary>
        /// <param name="atom">The containing Atom.</param>
        public PersonAppearanceRestorableCollection(Atom atom) : base(atom)
        {
            _selector = ContainingAtom.GetStorableByID("geometry") as DAZCharacterSelector;
        }
        #endregion

        #region Build
        /// <summary>
        /// Adds a restorable for the DAZCharacterSelector.
        /// </summary>
        /// <param name="attribute">The restorable component to add.</param>
        public void AddRestorable(PersonAppearanceRestorable<DAZCharacterSelector> attribute)
        {
            Geometry = attribute;
        }

        /// <summary>
        /// Adds a set of restorables for the DAZCharacterMaterialOptions.
        /// </summary>
        /// <param name="attribute">The set of restorables to add.</param>
        public void AddRestorables(AppearanceSetRestorable<DAZCharacterMaterialOptions> attribute)
        {
            Logger.Message("Inside of add charmatopts");
            foreach (KeyValuePair<string, IComponentRestorable> cmo in attribute.Storables) {
                ((AppearanceRestorable<DAZCharacterMaterialOptions>)cmo.Value).Init();
            }
            CharacterMaterials = attribute;
        }
        #endregion

        #region Init
        /// <summary>
        /// Initializes the collection.
        /// </summary>
        public override void Init()
        {
            base.Init();
            Geometry?.Init();
            CharacterMaterials?.Init();
            foreach (IComponentSetRestorable attribute in RestorableSetCollection) {
                attribute.Init();
            }
            foreach (IComponentRestorable attribute in RestorableCollection) {
                attribute.Init();
            }
        }
        #endregion // Initialize

        #region Update
        /// <summary>
        /// Updates the collection's state.
        /// </summary>
        /// <param name="a">An optional Atom parameter for the update.</param>
        /// <returns>The updated JSON representation of the collection's state.</returns>
        public override JSONClass Update(Atom a = null)
        {
            Geometry.Update();
            CharacterMaterials.Update();
            foreach (var attribute in RestorableSetCollection) {
                attribute.Update();
            }
            foreach (var attribute in RestorableCollection) {
                attribute.Update();
            }
            return Restorable;
        }
        #endregion // Update

        #region Restore
        /// <summary>
        /// Restores the collection's state.
        /// </summary>
        /// <param name="atom">The Atom to restore to.</param>
        public override void Restore(Atom atom = null)
        {
            if (atom == null) {
                Logger.Error("No Atom provided for restoration.");
                return;
            }

            try {
                // Restore Geometry
                Geometry?.Restore(atom);
                // Restore Character Materials
                foreach (var key in GeometryKeys) {
                    if (_restorable.HasKey(key)) {
                        CharacterMaterials.Restore(atom);
                    }
                }
                // Restore other attributes in RestorablesCollection
                foreach (var attribute in RestorableSetCollection) {
                    attribute.Restore(atom);
                }
                // Restore other attributes in RestorableCollection
                foreach (var attribute in RestorableCollection) {
                    attribute.Restore(atom);
                }
                Logger.Message("Restoration complete.");
            }
            catch (System.Exception e) {
                Logger.Error($"Restoration failed: {e.Message}");
            }
        }
        public override JSONClass GetJSON(JSONClass jsonBase = null)
        {

            JSONClass jclass = jsonBase ?? new JSONClass(){
              //  {"id", (JSONNode) _person.name + " Attributes"},
                {"on", (JSONNode) "true"},
                {"type", (JSONNode) "Person"},
                {"storables", new JSONArray {
                    new JSONClass{
                        {"id", "geometry"}
                   }
                }}
            };
            foreach (string s in GeometryKeys) {
                if (null != Geometry.Restorable[s]) {
                    jclass["storables"][0].Add(s, Geometry.Restorable[s]);
                }
            }
            if (CharacterMaterials.Restorable?["storables"]?.Count > 0) {
                foreach (JSONClass attributeJSON in CharacterMaterials.Restorable["storables"] as JSONArray) {
                    jclass["storables"].Add(attributeJSON);
                }
            }
            return base.GetJSON(jclass);
        }
        public override string ToString()
        {
            return $"{nameof(PersonAppearanceRestorableCollection)} {ContainingAtom.name}";
        }
        #endregion // Restore
    }
}