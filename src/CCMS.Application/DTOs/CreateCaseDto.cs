using System.ComponentModel.DataAnnotations;
using CCMS.Domain.Enums;

namespace CCMS.Application.DTOs
{
    public class CreateCaseDto
    {
        [Required]
        public string DefendantName { get; set; } = string.Empty;
        
        [Required]
        public string TargetBank { get; set; } = string.Empty;
        
        [Required]
        public string AccountNumber { get; set; } = string.Empty;
        
        [Required]
        public string AadhaarNumber { get; set; } = string.Empty;
        
        [Required]
        public string PanNumber { get; set; } = string.Empty;
        
        [Required]
        public OrderType OrderType { get; set; }
        
        public decimal? RequestedFreezeAmount { get; set; } 
    }
}
