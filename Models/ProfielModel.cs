using Postgrest.Attributes;
using Postgrest.Models;

namespace VIP_Planning.Models
{
    [Table("profielen")]
    public class ProfielModel : BaseModel
    {
        [PrimaryKey("email", false)]
        public string Email { get; set; }

        [Column("naam")]
        public string Naam { get; set; }

        [Column("pincode")]
        public string Pincode { get; set; }
    }
}