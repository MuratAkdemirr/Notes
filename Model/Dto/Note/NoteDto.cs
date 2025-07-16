using Notes.Model.Entities;

namespace Notes.Model.Dto;

public class NoteDto
{
    public int Id { get; set; }
    public string Title { get; set; }

    public string Text { get; set; }

    public List<string> Tags { get; set; }

    public DateTime CreatedOn { get; set; }
    public DateTime ModifiedOn { get; set; }
}