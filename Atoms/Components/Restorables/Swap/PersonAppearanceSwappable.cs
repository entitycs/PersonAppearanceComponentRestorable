using System;
using UnityEngine.Events;
using SimpleJSON;
using System.Collections.Generic;
using EntityCX.Atoms.Restorables.Appearance;
using EntityCX.Atoms.Restorables.Appearance.Person;
using EntityCX.Atoms.Restorables.Appearance.Person.Sets;
using EntityCX.EntityUtils;

namespace EntityCX.Atoms.Restorables.Swap
{
    /// <summary>
    /// Manages the appearance swapping functionality for a person atom.
    /// </summary>
    public class AppearanceSwappable
    {
        private int _current = 0;
        private Atom _atom;
        public List<PersonAppearanceRestorableCollection> collectionSet;
        public SwapUX inGameUI;
        public JSONStorableFloat IndexBubbleLifetime;
        public JSONStorableBool _keepClothes;
        public JSONStorableBool _keepHair;
        public JSONStorableBool _logger;
        public UnityAction restoreAction;
        public UnityAction setAppearanceAction;
        public System.Action<int> CurrentChoiceAction;
        public UnityAction AddAppearanceSlot;

        /// <summary>
        /// Gets the next index in the collection set.
        /// </summary>
        private int NextIndex => (_current + 1) % collectionSet.Count;

        /// <summary>
        /// Gets the previous index in the collection set.
        /// </summary>
        private int PrevIndex => (_current - 1) < 0 ? collectionSet.Count - 1 : (_current - 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="AppearanceSwappable"/> class.
        /// </summary>
        /// <param name="person">The person atom whose appearance can be swapped.</param>
        public AppearanceSwappable(Atom person)
        {
            EntityUtils.Logger.Message($"SwapAppearance: {person.name}");
            try {
                _atom = person;

                collectionSet = new List<PersonAppearanceRestorableCollection>();
                setAppearanceAction = setAppearance;
                restoreAction = Restore;
                AddAppearanceSlot = AddCollectionSlot;
                CurrentChoiceAction = SetCurrentFromUI;
            }
            catch (Exception e) {
                EntityUtils.Logger.Message($"--Initialization Failure {e}");
            }
        }

        /// <summary>
        /// Moves to the next index in the collection set.
        /// </summary>
        private void MoveNext() => _current = NextIndex;

        /// <summary>
        /// Sets the appearance to the current state.
        /// </summary>
        public void setAppearance() => UpdateCurrent();

        /// <summary>
        /// Restores the appearance to the next state.
        /// </summary>
        public void Restore() => RestoreNext();

        /// <summary>
        /// Adds a new collection to the collection set.
        /// </summary>
        /// <param name="collection">The collection to add.</param>
        public void AddCollection(PersonAppearanceRestorableCollection collection)
        {
            collection.Init();
            collection.Update();
            collectionSet.Add(collection);
        }

        /// <summary>
        /// Adds a default collection slot to the collection set.
        /// </summary>
        public void AddCollectionSlot()
        {
            AddDefaultCollection();
            EntityUtils.Logger.Message($"appearance sets: {collectionSet.Count}");
        }

        /// <summary>
        /// Sets the current index from the UI.
        /// </summary>
        /// <param name="current">The current index from the UI.</param>
        public void SetCurrentFromUI(int current)
        {
            _current = current;
            _current = PrevIndex; // sanitizes and shifts beg. index to 0 from UI 1
        }

        /// <summary>
        /// Creates a default collection of appearance restorables.
        /// </summary>
        /// <returns>A collection of person appearance restorables.</returns>
        public PersonAppearanceRestorableCollection DefaultCollection()
        {
            PersonAppearanceRestorableCollection set = new PersonAppearanceRestorableCollection(_atom);
            // Single attributes
            set.AddRestorable(new PersonAppearanceRestorable<DAZCharacterSelector>(_atom, _atom.GetStorableByID("geometry") as DAZCharacterSelector));
            AppearanceRestorable<DAZCharacterTextureControl> skinTextures;
            skinTextures = new AppearanceRestorable<DAZCharacterTextureControl>(_atom);
            skinTextures.Init();
            set.AddRestorable(new AppearanceRestorable<DAZCharacterTextureControl>(_atom));
            // Attribute sets
            AppearanceSetRestorable<DAZCharacterMaterialOptions> characterMaterials;
            characterMaterials = new AppearanceSetRestorable<DAZCharacterMaterialOptions>(_atom);
            characterMaterials.Init();
            Logger.Message("adding charmaterialoptionsfromcollection");
            set.AddRestorables(characterMaterials);
            AppearanceSetRestorable<DAZSkinWrapMaterialOptions> skinWrapMaterials;
            skinWrapMaterials = new AppearanceSetRestorable<DAZSkinWrapMaterialOptions>(_atom);
            skinWrapMaterials.Init();
            set.AddRestorables(skinWrapMaterials);
            HairAppearanceRestorables<HairSimControl> simHair = new HairAppearanceRestorables<HairSimControl>(_atom);
            simHair.Init();
            set.AddRestorables(simHair);
            return set;
        }

        /// <summary>
        /// Adds a default collection to the collection set.
        /// </summary>
        /// <returns>The added collection.</returns>
        public PersonAppearanceRestorableCollection AddDefaultCollection()
        {
            PersonAppearanceRestorableCollection set = DefaultCollection();
            AddCollection(set);
            return set;
        }

        /// <summary>
        /// Gets the current collection in the collection set.
        /// </summary>
        private PersonAppearanceRestorableCollection Current => collectionSet[_current];

        /// <summary>
        /// Updates the current collection.
        /// </summary>
        public void UpdateCurrent()
        {
            EntityUtils.Logger.Message($"Updating index {_current}");
            Current.Init();
            EntityUtils.Logger.Message($"Init called. Calling update {_current}");
            Current.Update();
            EntityUtils.Logger.Message($"Updating finished");
        }

        /// <summary>
        /// Restores the appearance to the next state in the collection set.
        /// </summary>
        public void RestoreNext()
        {
            try {
                UpdateCurrent();
                MoveNext();
                DAZCharacterSelector dcs = _atom.GetStorableByID("geometry") as DAZCharacterSelector;
                dcs.selectedCharacter.skin.Mesh.ClearBlendShapes();
                _atom.PreRestore(false, true);
                JSONClass restoreJSON = Current.GetJSON();
                if (restoreJSON != null && restoreJSON.Count > 0) {
                    EntityUtils.Logger.Message($"Restoring {_current} from {restoreJSON}");
                    _atom.Restore(restoreJSON, false, true, false, null, false, false, true, false);
                }
            }
            catch (System.Exception e) {
                EntityUtils.Logger.Error($"{e} {Current}");
            }
        }
    }
}
