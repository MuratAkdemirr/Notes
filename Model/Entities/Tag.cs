using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Notes.Model.Entities;

public class Tag
{
    public int Id { get; set; }
    [Required]
    [MaxLength(10)]
    public string Name { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}