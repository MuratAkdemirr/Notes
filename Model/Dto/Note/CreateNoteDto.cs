using System.ComponentModel.DataAnnotations;

namespace Notes.Model.Dto;

public class CreateNoteDto
{
    [Required]
    public string Text { get; set; }

    public List<string> Tags { get; set; } = new();
}