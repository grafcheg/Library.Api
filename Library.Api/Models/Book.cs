﻿namespace Library.Api.Models;

public class Book
{
    public string Isbn { get; set; } = default!;
    public string Title { get; set; } = default!;
    public string Author { get; set; } = default!;
    public string Description { get; set; } = default!;
    public int PageCount { get; set; }
    public DateTime PublishDate { get; set; }
}