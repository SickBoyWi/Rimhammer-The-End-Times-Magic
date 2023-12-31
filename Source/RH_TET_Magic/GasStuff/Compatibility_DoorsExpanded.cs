using HugsLib.Utils;
using System;
using System.Reflection;
using System.Reflection.Emit;
using Verse;

namespace TheEndTimes_Magic
{
    internal static class Compatibility_DoorsExpanded
    {
        public static void OnDefsLoaded()
        {
            System.Type typeInAnyAssembly1 = GenTypes.GetTypeInAnyAssembly("DoorsExpanded.Building_DoorExpanded", (string)null);
            if (!(typeInAnyAssembly1 != (System.Type)null))
                return;
            try
            {
                if (!typeof(Building).IsAssignableFrom(typeInAnyAssembly1))
                    throw new Exception("Expected DoorsExpanded.Building_DoorExpanded to extend " + typeof(Building).Name);
                FieldInfo field = typeInAnyAssembly1.GetField("openInt", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field == (FieldInfo)null || field.FieldType != typeof(bool))
                    throw new Exception("Expected DoorsExpanded.Building_DoorExpanded to have field openInt");
                System.Type typeInAnyAssembly2 = GenTypes.GetTypeInAnyAssembly("DoorsExpanded.Building_DoorRemote", (string)null);
                if (typeInAnyAssembly2 == (System.Type)null || !typeInAnyAssembly1.IsAssignableFrom(typeInAnyAssembly2))
                    throw new Exception("Expected type DoorsExpanded.Building_DoorRemote, extending DoorsExpanded.Building_DoorExpanded");
                ThingDef curtainDoorDef = DefDatabase<ThingDef>.GetNamedSilentFail("HeronCurtainTribal");
                if (curtainDoorDef == null)
                    LogDefWarning("HeronCurtainTribal");
                ThingDef jailDoorDef = DefDatabase<ThingDef>.GetNamedSilentFail("PH_DoorJail");
                if (jailDoorDef == null)
                    LogDefWarning("PH_DoorJail");
                Func<Building, bool> isOpenGetter = Compatibility_DoorsExpanded.CreateGetterForField(field);
                GasCloud.TraversibleBuildings.Add(typeInAnyAssembly1, (GasCloud.TraversibilityTest)((building, _) =>
                {
                    ThingDef def = building.def;
                    return def == curtainDoorDef || def == jailDoorDef || isOpenGetter(building);
                }));
                GasCloud.TraversibleBuildings.Add(typeInAnyAssembly2, (GasCloud.TraversibilityTest)((building, _) => isOpenGetter(building)));
                //Log.Message("Applied compatibility layer for Doors Expanded mod.");
            }
            catch (Exception ex)
            {
                Log.Error(string.Format("Failed to apply compatibility layer for {0}: {1}", (object)"Doors Expanded mod", (object)ex));
            }

            void LogDefWarning(string defName)
            {
                Log.Warning("Expected to find def " + defName + " in Doors Expanded mod.");
            }
        }

        private static Func<Building, bool> CreateGetterForField(FieldInfo field)
        {
            System.Type reflectedType = field.ReflectedType;
            if (reflectedType == (System.Type)null)
                throw new Exception("Unexpected reflection error");
            DynamicMethod dynamicMethod = new DynamicMethod(reflectedType.FullName + ".get_" + field.Name, typeof(bool), new System.Type[1]
            {
        typeof (Building)
            }, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Castclass, reflectedType);
            ilGenerator.Emit(OpCodes.Ldfld, field);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<Building, bool>)dynamicMethod.CreateDelegate(typeof(Func<Building, bool>));
        }
    }
}
