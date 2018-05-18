// (c) Alexander Lelidis, Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

namespace BRS.Engine.PostProcessing {

    /// <summary>
    /// All supported postprocessing-efects
    /// </summary>
    public enum PostprocessingType {
        BlackAndWhite,
        Chromatic,
        Vignette,
        GaussianBlur,
        DepthOfField,
        ColorGrading,
        ShockWave,
        Wave,
        TwoPassBlur
    }
}