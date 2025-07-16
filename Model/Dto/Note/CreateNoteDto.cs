using System.ComponentModel.DataAnnotations;

namespace Notes.Model.Dto;

public class CreateNoteDto
{
    public string Title { get; set; }
    [Required]
    public string Text { get; set; }

    public List<string> Tags { get; set; } = new();
}