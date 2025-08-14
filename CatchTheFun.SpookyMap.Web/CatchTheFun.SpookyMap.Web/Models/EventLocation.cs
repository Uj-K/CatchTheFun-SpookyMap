// EventLocation 모델을 수정할 때마다 새로운 마이그레이션을 생성해야함

using System.ComponentModel.DataAnnotations;

namespace CatchTheFun.SpookyMap.Web.Models
{
    public class EventLocation
    {
        public int Id { get; set; }

        // Actual name of the house owner, or nick name
        [Required]
        [Display(Name = "Name or Nickname")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; }

        // Address where the event is happening
        [Required]
        public string Address { get; set; }

        [Required]
        [Display(Name = "What type of candy do you have?")]
        public string? Description { get; set; }

        // User can update and show off their decoration
        public string? PhotoUrl { get; set; }

        // Starting time of the event
        public TimeOnly? StartTime { get; set; }
        // closing time of the event
        public TimeOnly? EndTime { get; set; }  


        [Required]
        [Display(Name = "Do you have something other than candy?")]
        public bool SomethingElse { get; set; }

        // Geocoding 결과로 저장되는 위치 정보
        public double? Lat { get; set; }
        public double? Lng { get; set; }
    }
}
