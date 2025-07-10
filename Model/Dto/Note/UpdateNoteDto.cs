namespace Notes.Model.Dto;

public class UpdateNoteDto
{
    public int Id { get; set; } 

    public string? Text { get; set; } 

    public string? Status { get; set; } 

    public List<string>? Tags { get; set; } 
}