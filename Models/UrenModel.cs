using Postgrest.Attributes;
using Postgrest.Models;
namespace VIP_Planning.Models {
    [Table("uren")] 
    public class UrenModel : BaseModel {
        [PrimaryKey("id", false)] public long Id { get; set; }
        [Column("user_email")] public string UserEmail { get; set; } = string.Empty;
        [Column("datum_string")] public string DatumString { get; set; } = string.Empty;
        [Column("locatie")] public string Locatie { get; set; } = string.Empty;
        [Column("uren")] public double AantalUren { get; set; }
        [Column("status")] public string Status { get; set; } = string.Empty;
        [Column("is_uitbetaald")] public bool IsUitbetaald { get; set; }
        public string PeriodeNaam { get; set; } = string.Empty;
    }
}