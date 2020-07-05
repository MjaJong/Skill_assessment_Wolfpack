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
        public string BirthDate { get; set; }

        /// <summary>
        /// Gender of the wolf. This can be:
        /// 0 = Not known;
        ///1 = Male;
        ///2 = Female;
        ///3 = Non binary;
        ///9 = Not applicable.
        /// </summary>
        public int Gender { get; set; }

        /// <summary>
        /// Property for the location of the wolf. This should always be made up of an array of at least size 2 and not bigger than 3. Omitting the third
        /// element is assumed to be an elevation of zero.
        /// </summary>
        public string Location { get; set; }
    }
}
