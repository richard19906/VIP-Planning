using Postgrest.Attributes;
using Postgrest.Models;
using Newtonsoft.Json;
using System;

namespace VIP_Planning.Models
{
    [Table("uren")]
    public class UrenModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public int Id { get; set; }

        [Column("user_email")]
        public string UserEmail { get; set; } = "";

        [Column("user_naam")]
        public string UserNaam { get; set; } = "";

        [Column("datum_string")]
        public string DatumString { get; set; } = "";

        [Column("locatie")]
        public string Locatie { get; set; } = "";

        [Column("uren")]
        public double Uren { get; set; }

        [Column("periode_naam")]
        public string PeriodeNaam { get; set; } = "";

        [Column("is_uitbetaald")]
        public bool IsUitbetaald { get; set; }

        [Column("status")]
        public string Status { get; set; } = "Concept";

        [JsonIgnore]
        public double AantalUren => Uren;

        [JsonIgnore]
        public string DisplayUren
        {
            get
            {
                if (IsUitbetaald) return "✓";
                if (!string.IsNullOrEmpty(Locatie) && Locatie.ToLower().Contains("vrij")) return "*";
                return Uren.ToString("N1");
            }
        }

        [JsonIgnore]
        public int MaandVolgorde => PeriodeNaam switch
        {
            "Jan" => 1,
            "Feb" => 2,
            "Mrt" => 3,
            "Apr" => 4,
            "Mei" => 5,
            "Jun" => 6,
            "Jul" => 7,
            "Aug" => 8,
            "Sep" => 9,
            "Okt" => 10,
            "Nov" => 11,
            "Dec" => 12,
            _ => 99
        };
    }

    [Table("planning")]
    public class PlanningModel : BaseModel
    {
        [PrimaryKey("id", false)]
        public long Id { get; set; }

        [Column("user_email")]
        public string UserEmail { get; set; } = "";

        [Column("user_naam")]
        public string UserNaam { get; set; } = "";

        [Column("datum")]
        public string Datum { get; set; } = "";

        [Column("locatie")]
        public string Locatie { get; set; } = "";

        [Column("start_tijd")]
        public string StartTijd { get; set; } = "";

        [Column("eind_tijd")]
        public string EindTijd { get; set; } = "";

        [Column("uren")]
        public double Uren { get; set; }

        [Column("is_gepusht")]
        public bool IsGepusht { get; set; } = false;

        // TOEGEVOEGD: Nodig voor status-badges en voltooid-vinkjes
        [Column("status")]
        public string Status { get; set; } = "Gepland";
    }

    [Table("profielen")]
    public class ProfielModel : BaseModel
    {
        [PrimaryKey("email", false)]
        public string Email { get; set; } = "";

        [Column("naam")]
        public string Naam { get; set; } = "";
    }
}