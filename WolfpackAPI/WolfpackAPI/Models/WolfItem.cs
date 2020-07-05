using System;

namespace WolfAPI.Models
{
    /// <summary>
    /// Data object for a wolf.
    /// </summary>
    public class WolfItem
    {
        /// <summary>
        /// Database ID given to the wolf.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Name of the wolf.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// Birthdate of the wolf.
        /// </summary>
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// Gender of the wolf.
        /// </summary>
        public WolfGender Gender { get; set; }

        /// <summary>
        /// Property for the location of the wolf.
        /// </summary>
        public WolfLocation Location { get; set; }
    }

    /// <summary>
    /// Enumeration of the ISO/IEC 5218 standard for including gender with one additional field.
    /// </summary>
    public enum WolfGender 
    { 
        Not_known = 0,
        Male = 1,
        Female = 2,
        Non_binary = 3,
        Not_applicable = 9
    }

    /// <summary>
    /// Struct to save the position of a wolf following ISO 6709. Every coordinate is converted into a float representation of the original coordinate.
    /// </summary>
    public struct WolfLocation
    {
        float Latitude;
        float Longtitude;
        float Elevation;
    }
}
