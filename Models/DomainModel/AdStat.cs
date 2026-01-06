using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mini_Social_Media.Models {
    public class AdStat {
        [Key]
        public int Id { get; set; }

        public int AdvertisementId { get; set; }
        [ForeignKey("AdvertisementId")]
        public virtual Advertisement Advertisement { get; set; }

        public DateTime Date { get; set; }

        public int Impressions { get; set; }
        public int Clicks { get; set; }
    }
}