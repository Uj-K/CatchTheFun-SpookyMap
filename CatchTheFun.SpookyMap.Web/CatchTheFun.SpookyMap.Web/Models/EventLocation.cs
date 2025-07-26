// EventLocation 모델을 수정할 때마다 새로운 마이그레이션을 생성해야함

using System.ComponentModel.DataAnnotations;

namespace CatchTheFun.SpookyMap.Web.Models
{
    public class EventLocation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Location Name")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Display(Name = "Description (optional)")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        // Geocoding 결과로 저장되는 위치 정보
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
