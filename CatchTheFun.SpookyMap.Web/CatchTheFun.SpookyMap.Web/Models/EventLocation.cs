// EventLocation 모델을 수정할 때마다 새로운 마이그레이션을 생성해야함

using System.ComponentModel.DataAnnotations;

namespace CatchTheFun.SpookyMap.Web.Models
{
    public class EventLocation
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "What type of candy do you have?")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Do you have something other than candy?")]
        public bool SomethingElse { get; set; }

        // Geocoding 결과로 저장되는 위치 정보
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
