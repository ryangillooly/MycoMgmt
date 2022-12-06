namespace MycoMgmt.Populator.Models
{
    public enum Permissions
    {
        Delete,
        Update,
        Create,
        Read
    }

    public enum Roles
    {
        Administrator,
        Reader,
        Writer
    }

    public enum Users
    {
        Ryan,
        Calum,
        Callum,
        Aiden
    }

    public enum Strain
    {
        GoldenTeacher,
        BPlus,
        Mazapatec
    }

    public enum Locations
    {
        GrowTent,
        IncubationChamber,
        Fridge,
        WineCooler,
        LongTermStorage
    }

    public enum SpawnTypes
    {
        RyeGrain,
        PopCorn,
        Millet,
        BrownRice
    }

    public enum BulkTypes
    {
        CocoCoir,
        CVG,
        Straw,
        Manure
    }

    public enum CultureTypes
    {
        Agar,
        LiquidCulture,
        SporePrint,
        SporeSyringe
    }

    public enum RecipeTypes
    {
        Agar,
        Bulk,
        LiquidCulture,
        Spawn
    }

    public enum EntityTypes
    {
        GrowTent,
        IncubationChamber,
        Fridge,
        WineCooler,
        LongTermStorage,
        RyeGrain,
        PopCorn,
        Millet,
        BrownRice,
        Culture,
        Spawn,
        Bulk,
        Fruits,
        Location,
        Recipe,
        Ingredient,
        Vendor,
        Account,
        User,
        IamRole,
        Permission,
        Purchase,
        Strain,
        Failed,
        Success,
        Agar,
        LiquidCulture,
        SporePrint,
        SporeSyringe,
        CocoCoir,
        CVG,
        Straw,
        Manure
    }
}