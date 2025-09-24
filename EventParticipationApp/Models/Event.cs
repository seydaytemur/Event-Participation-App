using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 

namespace EventParticipationApp.Models
{
    public class Event
    {
        public Event() // Yapıcı metod (Constructor)
        {
            Participations = new List<Participation>(); 
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık alanı zorunludur.")] 
        public required string Title { get; set; }

        [Required(ErrorMessage = "Açıklama alanı zorunludur.")] 
        public required string Description { get; set; }

        [Required(ErrorMessage = "Tarih alanı zorunludur.")] 
        [Display(Name = "Etkinlik Tarihi")] 
        [DataType(DataType.DateTime)] 
        public DateTime EventDate { get; set; }

        
        public ICollection<Participation> Participations { get; set; }
    }
}