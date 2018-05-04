// (c) Andreas Emch 2018
// ETHZ - GAME PROGRAMMING LAB

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace BRS.Engine.Utilities {
    /// <summary>
    /// Extensions for the enum-types
    /// </summary>
    static class EnumExtensions {

        /// <summary>
        /// Get the description property of the display-attribute if there is any, otherwise the ToString-value
        /// </summary>
        /// <param name="value">Enum-type</param>
        /// <returns>String of the display-value if existing, otherwise the ToString-value</returns>
        public static string GetDescriptionFromEnumValue(Enum value) {
            DisplayAttribute attribute = value.GetType()
                .GetField(value.ToString())
                .GetCustomAttributes(typeof(DisplayAttribute), false)
                .SingleOrDefault() as DisplayAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }


        /// <summary>
        /// Get the description property of the display-attribute if there is any, otherwise the ToString-value
        /// </summary>
        /// <param name="value">Enum-type</param>
        /// <returns>String of the display-value if existing, otherwise the ToString-value</returns>
        public static string GetDescription(this Enum value) {
            return GetDescriptionFromEnumValue(value);
        }
    }
}
