// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System.ComponentModel.DataAnnotations;

namespace BRS.Scripts.Elements.Lighting {
    public enum FollowerType {
        [Display(Description = "dynamicShadow")]
        DynamicShadow,
        [Display(Description = "lightYellow")]
        LightYellow,
        [Display(Description = "lightBlue")]
        LightBlue,
        [Display(Description = "lightRed")]
        LightRed
    }
}
