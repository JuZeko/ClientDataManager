using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ClientDataManager.Models
{
    [Index(nameof(Name), IsUnique = true)]
    public class Client 
    {
        [Key]
        public int ClientID { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        public string? Name { get; set; }

        [Required]
        public string? Address { get; set; }

        public string? PostCode { get; set; }
    }
}
