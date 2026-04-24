using Postgrest.Attributes;
using Postgrest.Models;
using System;
namespace VIP_Planning.Models {
    [Table("planning")]
    public class PlanningModel : BaseModel {
        [PrimaryKey("id", false)] public long Id { get; set; }
        [Column("user_email")] public string UserEmail { get; set; } = string.Empty;
        [Column("datum")] public DateTime Datum { get; set; }
        [Column("locatie")] public string Locatie { get; set; } = string.Empty;
        [Column("uren")] public double AantalUren { get; set; } // Gefixt: kolomnaam naar 'uren'
        [Column("is_gepusht")] public bool IsGepusht { get; set; }
    }
}