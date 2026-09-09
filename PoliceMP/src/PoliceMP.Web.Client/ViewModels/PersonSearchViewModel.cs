using System.ComponentModel.DataAnnotations;

namespace PoliceMP.Web.Client.ViewModels
{
    public class PersonSearchViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}