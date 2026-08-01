using System.Collections.Generic;
using HarmonyLib;
using KMod;
using UnityEngine;
using System.IO;
using static Localization;

namespace MoveThisHere
{
    /// <summary>
    /// Main mod entry point and Harmony patches for MoveThisHere.
    /// </summary>
    public class MoveThisHere_Patch : UserMod2
    {

        /// <summary>
        /// Loads localization .po files and registers STRINGS with the game.
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public class Localization_Initialize_Patch
        {
            private static readonly string ModPath = GetModPath();

            public static void Postfix()
            {
                RegisterForTranslation(typeof(STRINGS));
                GenerateStringsTemplate(typeof(STRINGS), Path.Combine(Manager.GetDirectory(), "strings_templates"));
                LoadStrings();
                LocString.CreateLocStringKeys(typeof(STRINGS), null);
            }

            private static void LoadStrings()
            {
                string localeCode = GetLocale()?.Code;
                if (string.IsNullOrEmpty(localeCode))
                    return;

                string path = Path.Combine(ModPath, "locales", localeCode + ".po");
                if (File.Exists(path))
                    OverloadStrings(LoadStringsFile(path, false));
            }

            private static string GetModPath()
            {
                var assembly = typeof(Localization_Initialize_Patch).Assembly;
                return Path.GetDirectoryName(assembly.Location);
            }
        }
        

        /// <summary>
        /// Adds the Hauling Point building to the Base > storage build menu.
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {
            public static void Prefix()
            {
                Utils.AddPlan("Base", "storage", HaulingPointConfig.Id, "StorageLocker");
            }
        }

        /// <summary>
        /// Hides the material selector in the build info panel because the hauling point costs no materials.
        /// </summary>
        [HarmonyPatch(typeof(ProductInfoScreen))]
        [HarmonyPatch(nameof(ProductInfoScreen.SetMaterials))]
        public static class ProductInfoScreen_SetMaterials_Patch
        {
            public static void Postfix(BuildingDef def, ref ProductInfoScreen __instance)
            {
                if (def.name == "HaulingPoint")
                {
                    __instance.materialSelectionPanel.gameObject.SetActive(false); //remove material selector since no materials
                }
            }
        }

        /// <summary>
        /// Overrides the resource-remaining hover text to show "No resources required" for the hauling point.
        /// </summary>
        [HarmonyPatch(typeof(ResourceRemainingDisplayScreen))]
        [HarmonyPatch(nameof(ResourceRemainingDisplayScreen.GetString))]
        public static class ResourceRemainingDisplayScreen_Patch
        {
            public static string Postfix(string __result, Recipe ___currentRecipe)
            {
                if (___currentRecipe.Ingredients[0].amount == 1f)
                {
                    if (BuildTool.Instance.GetComponent<BuildToolHoverTextCard>().currentDef.name == "HaulingPoint")
                    {
                        __result = STRINGS.BUILDINGS.BUTTONS.HAULINGPOINT.NO_RESOURCES_REQUIRED;
                    }
                }
                return __result;
            }
        }

        /// <summary>
        /// Forces the hauling point to be built from Diamond with no resource cost.
        /// </summary>
        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch(nameof(BuildingDef.Instantiate))]
        public static class BuildingDef_Instantiate_Patch
        {
            public static bool Prefix(Vector3 pos, Orientation orientation, IList<Tag> selected_elements, int layer, BuildingDef __instance, ref GameObject __result)
            {
                BuildingDef def = __instance;
                if (__instance.name != "HaulingPoint")
                {
                    return true;
                }
                else
                {
                    IList<Tag> diamondElement = new List<Tag> { TagManager.Create("Diamond") };
                    __result = __instance.Build(Grid.PosToCell(pos), orientation, null, diamondElement, 293.15f, playsound: false, GameClock.Instance.GetTime());
                    ForceDiamondPrimaryElement(__result);
                    return false;
                }
            }

            private static void ForceDiamondPrimaryElement(GameObject gameObject)
            {
                if (gameObject == null)
                {
                    return;
                }

                PrimaryElement primaryElement = gameObject.GetComponent<PrimaryElement>();
                if (primaryElement != null)
                {
                    primaryElement.SetElement(SimHashes.Diamond);
                    primaryElement.Mass = 1f;
                    primaryElement.Temperature = 293.15f;
                }
            }
        }
    }

    /// <summary>
    /// Helper utilities for registering building strings and build menu plans.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Registers the name, description, and effect strings for a building.
        /// </summary>
        /// <param name="buildingId">The building ID (prefab name) to register strings for.</param>
        /// <param name="name">The in-game display name.</param>
        /// <param name="description">The short description shown in the build menu.</param>
        /// <param name="effect">The longer effect description shown in the build menu.</param>
        public static void AddBuildingStrings(string buildingId, string name, string description, string effect)
        {
           Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", global::STRINGS.UI.FormatAsLink(name, buildingId));
           Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
           Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
        }

        /// <summary>
        /// Adds a building to the given plan (build menu) category and subcategory.
        /// </summary>
        /// <param name="category">Top-level plan category, e.g. "Base".</param>
        /// <param name="subcategory">Subcategory within the plan, e.g. "storage".</param>
        /// <param name="idBuilding">Building ID to add.</param>
        /// <param name="addAfter">Optional building ID to insert after.</param>
        public static void AddPlan(HashedString category, string subcategory, string idBuilding, string addAfter = null)
        {
            Debug.Log("Adding " + idBuilding + " to category " + category);
            foreach (PlanScreen.PlanInfo menu in TUNING.BUILDINGS.PLANORDER)
            {
                if (menu.category == category)
                {
                    AddPlanToCategory(menu, subcategory, idBuilding, addAfter);
                    return;
                }
            }

            Debug.Log($"Unknown build menu category: ${category}");
        }

        private static void AddPlanToCategory(PlanScreen.PlanInfo menu, string subcategory, string idBuilding, string addAfter = null)
        {
            List<KeyValuePair<string, string>> data = menu.buildingAndSubcategoryData;
            if (data != null)
            {
                if (addAfter == null)
                {
                    data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                }
                else
                {
                    int index = data.IndexOf(new KeyValuePair<string, string>(addAfter, subcategory));
                    if (index == -1)
                    {
                        Debug.Log($"Could not find building {subcategory}/{addAfter} to add {idBuilding} after. Adding at the end !");
                        data.Add(new KeyValuePair<string, string>(idBuilding, subcategory));
                        return;
                    }
                    data.Insert(index + 1, new KeyValuePair<string, string>(idBuilding, subcategory));
                }
            }
        }
    }
}

