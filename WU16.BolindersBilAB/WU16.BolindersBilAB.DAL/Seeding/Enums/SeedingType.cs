namespace WU16.BolindersBilAB.DAL.Seeding.Enums
{
    enum SeedingType
    {
        /// <summary>
        /// Will only seed propertys with a SeedAttribute or any of the seeding control attributes for example SeedDataType or SeedChooseFrom 
        /// </summary>
        Explicit,
        /// <summary>
        /// Will seed anything it can not marked with SeedIgnore
        /// </summary>
        Implicit
    }
}
