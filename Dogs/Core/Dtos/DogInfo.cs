using Core.Validators;
using System.ComponentModel.DataAnnotations;

namespace Core.Dtos
{
    public class DogInfo
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        [NoLessThan(0)]
        public int TailLength { get; set; }
        [Required]
        [NoLessThan(0)]
        public int Weight { get; set; }
    }
}