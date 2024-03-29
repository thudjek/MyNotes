﻿namespace SharedModels.Responses.Notes;
public class NoteResponse
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool Owned { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime Date { get; set; }
}