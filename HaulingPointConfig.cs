using TUNING;
using UnityEngine;
using System.Linq;

namespace MoveThisHere
{
    /// <summary>
    /// Building configuration for the Hauling Point, a 1×1 temporary storage for liquids and gases.
    /// </summary>
    public class HaulingPointConfig : IBuildingConfig
    {
        /// <summary>
        /// The in-game prefab/building ID for the Hauling Point.
        /// </summary>
        public const string Id = "HaulingPoint";

        /// <summary>
        /// Defines the building's size, animation, placement rules, and invincibility flags.
        /// </summary>
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef obj = BuildingTemplates.CreateBuildingDef(
                Id,
                1, 1,
                //"storagelocker_kanim",
                "haulingpoint_kanim", //I'm never using spriter again what a hassle!!
                30,
                3f,
                new float[1] { 1f }, //building mass is 1kg; less than 1kg causes graphical issues, zero mass causes error
                MATERIALS.ANY_BUILDABLE,
                9999f,
                BuildLocationRule.Anywhere,
                noise: NOISE_POLLUTION.NONE,
                decor: BUILDINGS.DECOR.PENALTY.TIER1); //decor -10 because it's a box of junk
            obj.Floodable = false;
            obj.AudioCategory = "Metal";
            obj.Overheatable = false;
            obj.Repairable = false;
            obj.Disinfectable = false;
            obj.Invincible = true; //nothing but the player can destroy the powerful haulingpoint
            obj.ObjectLayer = ObjectLayer.Canvases; // Different layer from Building to allow overlap, with better click priority


            return obj;
        }

        /// <summary>
        /// Sets up storage filters, the HaulingPoint behavior component, and replaces vanilla deconstruction.
        /// </summary>
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SoundEventVolumeCache.instance.AddVolume("storagelocker_kanim", "StorageLocker_Hit_metallic_low", NOISE_POLLUTION.NOISY.TIER1);
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = true;
            storage.allowItemRemoval = false;
            storage.showDescriptor = true;
            storage.storageFilters = STORAGEFILTERS.LIQUIDS.Concat(STORAGEFILTERS.GASES).ToList();
            //only gases and liquids
            storage.storageFullMargin = 0f;//STORAGE.STORAGE_LOCKER_FILLED_MARGIN;
            storage.fetchCategory = Storage.FetchCategory.GeneralStorage;
            storage.showCapacityStatusItem = true;
            storage.showCapacityAsMainStatus = true;
            go.AddOrGet<HaulingPoint>().totalMaxCapacity = 20000f;
            go.AddOrGetDef<RocketUsageRestriction.Def>(); //I wish I had the DLC, somebody post an issue if whatever this is doesn't work
            Object.Destroy(go.AddOrGet<Reconstructable>()); //remove vanilla reconstructable; material is forced by code and costs no resources
            Object.Destroy(go.AddOrGet<Deconstructable>());
            //also, custom deconstruction prevents the building material from being dropped
            go.AddOrGet<DeconstructableHaulingPoint>();


        }

        /// <summary>
        /// Finalizes the building prefab after configuration is complete.
        /// </summary>
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGetDef<StorageController.Def>();
        }
    }
}

