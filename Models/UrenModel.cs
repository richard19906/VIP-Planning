using Postgrest.Attributes;
using Postgrest.Models;

namespace VIP_Planning.Models
{
    [Table("uren")]
    public class UrenModel : BaseModel
    {
        [PrimaryKey("id", false)] // Supabase id
        public long Id { get; set; }

        [Column("datum")]
        public DateTime Datum { get; set; }

        [Column("locatie")]
        public string Locatie { get; set; }

        [Column("uren")]
        public float AantalUren { get; set; }

        [Column("periode_naam")]
        public string PeriodeNaam { get; set; }

        [Column("is_uitbetaald")]
        public bool IsUitbetaald { get; set; }
    }
}