// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

namespace BRS.Engine.Rendering {

    /// <summary>
    /// Defines the model-type which is used for the hardware-instanciation.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: The order is used for the draw-order -> The lower it is, the later it's drawn!
    /// This becomes crucial for the transparent-elements, for the other elements it doesn't matter since there is the z-buffer activated.
    /// </remarks>
    public enum ModelType {
        NoHardwareInstanciation,
        Skybox,
        Ground,
        InsideScene,
        OutsideScene,
        Cash,
        Gold,
        Diamond,
        Bomb,
        Capacity,
        Stamina,
        Key,
        Health,
        Shield,
        Speed,
        Trap,
        Explodingbox,
        Weight,
        Magnet,
        Police,
        Crate,
        Oil,
        Speedpad,
        Chair,
        Plant,
        Cart,
        Stack,
        WheelFl,
        WheelBz,
        WheelPolice,
        Player0,
        Player1,
        Player2,
        Player3,
        Base0,
        Base1,
        ArrowEnemy,
        ArrowBase,
        Vault,

        // The ones below have all a transparent texture => have to remain at the bottom!
        TracksOil,
        TracksSpeed,
        Shadow,
        YellowLight,
        RedLight,
        BlueLight
    }
}
