// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

namespace BRS.Engine.Rendering {
    /// <summary>
    /// Render type which reflects the available techniques for the hardware-instancing-shader
    /// </summary>
    public enum RenderingType {
        // Texture and baked lightmap
        Baked,
        // Only texture
        Texture,
        // Only texture which can have transparecy
        TextureTransparent,
        // Only texture which is set to a given transparency for all pixels
        TextureAlpha,
        // Only texture with an animated alpha-level
        TextureAlphaAnimated
    }
}
