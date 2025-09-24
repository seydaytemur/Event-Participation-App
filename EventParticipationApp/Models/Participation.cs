using System.ComponentModel.DataAnnotations; 

namespace EventParticipationApp.Models
{
    public class Participation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Katılımcı Adı zorunludur.")] 
        public string ParticipantName { get; set; }

        public int EventId { get; set; }

        public Event? Event { get; set; } 
    }
}