using Postgrest.Attributes;
using Postgrest.Models;
namespace VIP_Planning.Models {
    [Table("uren")]
    public class UrenModel : BaseModel {
        [PrimaryKey("id", false)]
        public long Id { get; set; }
        
        [Column("user_email")] // Dit was de missende link!
        public string? UserEmail { get; set; }
        
        [Column("uren")]
        public double? AantalUren { get; set; }
        
        [Column("datum_string")]
        public string? DatumString { get; set; }
        
        [Column("locatie")]
        public string? Locatie { get; set; }
        
        [Column("periode_naam")]
        public string? PeriodeNaam { get; set; }
    }
}