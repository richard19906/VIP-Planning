using Postgrest.Attributes;
using Postgrest.Models;
using System;

namespace VIP_Planning.Models {
    [Table("planning")]
    public class PlanningModel : BaseModel {
        [PrimaryKey("id", false)] public int Id { get; set; }
        [Column("medewerker")] public string Medewerker { get; set; } = "";
        [Column("locatie")] public string Locatie { get; set; } = "";
        [Column("datum")] public string Datum { get; set; } = ""; // Als string om conversiefouten te voorkomen
        [Column("uren")] public string Uren { get; set; } = "";
    }

    [Table("profiles")]
    public class ProfielModel : BaseModel {
        [PrimaryKey("id", false)] public Guid Id { get; set; }
        [Column("email")] public string Email { get; set; } = "";
        [Column("voornaam")] public string Voornaam { get; set; } = "";
        [Column("achternaam")] public string Achternaam { get; set; } = "";
        [Column("pincode")] public string Pincode { get; set; } = "";
    }
}