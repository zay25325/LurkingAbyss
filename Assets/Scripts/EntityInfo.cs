using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInfo : MonoBehaviour
{
    [SerializeField] List<EntityTags> tags = new List<EntityTags>();
    public List<EntityTags> Tags { get => tags; }

    public enum EntityTags
    {
        Interactable,

        // super catagories
        Item,
        Environment,
        Creature,

        // creature catagories
        Hunter,
        Wanderer,
        Territorial,

        // creature types
        Player,
        LilBrother,
        BigBrother,
        Charger,
        DroneLauncher,
        ExplosiveDrone,
        EyeMonster,
        CreepGuardian,
        Mimic,
        Trapper,
        Scavenger,
        HiveMother,
        Swarmling,

        // Enviroment Objects
        BearTrap,
        DartTrap,

        Door,
        Exit,
        ChargingStation,

        // Properties
        Breakable,
        CanOpenDoors,

        //Items
        BasicGun,
        Battery,
        DashBooster,
        Grappler,
        GravitonSurgePlateItem,
        Grenade,
        InvisibleBelt,
        MobileShieldGenerator,
        NoiseMaker,
        Revivor,
        Rock,
        Warper,


        TrapperBulb,
        WarpTube,
    }
}
