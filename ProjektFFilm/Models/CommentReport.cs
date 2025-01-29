using System;
using System.ComponentModel.DataAnnotations;

namespace FilmList.Models
{
    public class CommentReport
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Id komentarza")]
        public int CommentId { get; set; }
        public Comment Comment { get; set; }

        [Required]
        [Display(Name = "Powód zgłoszenia")]
        public string ReportReason { get; set; }

        [Required]
        [Display(Name = "Data zgłoszenia")]
        public DateTime ReportedOn { get; set; }

        [Display(Name = "Zgłoszone przez")]
        public string ReportedBy { get; set; }
    }
}
