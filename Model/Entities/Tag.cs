using Microsoft.AspNetCore.Identity;

namespace Notes.Model.Entities;

public class Tag
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public ICollection<Note> Notes { get; set; } = new List<Note>();
}