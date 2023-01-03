namespace MycoMgmt.API.Models;

public class CultureParameterList
{
    public string  name         { get; set;}
    public string  type         { get; set;}
    public string  strain       { get; set;}
    public string? recipe       { get; set;}
    public string? notes        { get; set;}
    public string? location     { get; set;}
    public string? parent       { get; set;}
    public string? parentType   { get; set;}
    public string? child        { get; set;}
    public string? childType    { get; set;}
    public bool?   successful   { get; set;}
    public bool    finished     { get; set;}
    public bool?   purchased    { get; set;}
    public string? vendor       { get; set;}
    public string? finishedOn   { get; set;}
    public string? inoculatedOn { get; set;}
    public string? inoculatedBy { get; set;}
    public string createdOn     { get; set;}
    public string createdBy     { get; set;}
    public int?   count         { get; set;} = 1;
}