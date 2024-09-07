using EntityCX.Atoms.Restorables.Appearance;
using SimpleJSON;

namespace EntityCX
{
    public class SwapAppearanceSerializer
    {
        public JSONClass serialize(AppearanceSetRestorable<DAZSkinWrapMaterialOptions> appearanceRestorables)
        {
            JSONClass jc = new JSONClass();
            return jc;
        }
    }

    public static class AppearanceStorableSerializer
    {
        public static JSONClass serialize(AppearanceRestorable<DAZCharacterSelector> dcs)
        {
            JSONClass jc = new JSONClass();
            var appearanceStorable = new JSONClass
            {
                { "CharacterSelector", dcs.Restorable },
            };
            jc.Add(appearanceStorable);
            return jc;
        }
        public static JSONClass serialize(AppearanceRestorable<DAZCharacterTextureControl> dcs)
        {
            JSONClass jc = new JSONClass();
            var appearanceStorable = new JSONClass
            {
                { "CharacterTextureControl", dcs.Restorable },
            };
            jc.Add(appearanceStorable);
            return jc;
        }
    }
    public class AppearanceStorablesSerializer
    {
        public JSONClass serialize()
        {
            JSONClass jc = new JSONClass();
            return jc;
        }
    }
}