using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ConferenceContext>(options => options.UseInMemoryDatabase("ConferenceDb"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Hello World!");


app.MapGet("/api/attendees", async (ConferenceContext context) => await context.Attendees.ToListAsync());
app.MapGet("/api/attendees/{id}", async (ConferenceContext context, int id) => await context.Attendees.FindAsync(id));
app.MapPost("/api/attendees", async (ConferenceContext context, Attendee attendee) =>
{
    context.Attendees.Add(attendee);
    await context.SaveChangesAsync();
    return Results.Created($"/api/attendees/{attendee.Id}", attendee);
});
app.MapPut("/api/attendees/{id}", async (ConferenceContext context, int id, Attendee inputAttendee) =>
{
    var attendee = await context.Attendees.FindAsync(id);
    if (attendee == null) return Results.NotFound();

    attendee.Name = inputAttendee.Name;
    attendee.Email = inputAttendee.Email;

    await context.SaveChangesAsync();
    return Results.NoContent();
});
app.MapDelete("/api/attendees/{id}", async (ConferenceContext context, int id) =>
{
    var attendee = await context.Attendees.FindAsync(id);
    if (attendee == null) return Results.NotFound();

    context.Attendees.Remove(attendee);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();

public class ConferenceContext : DbContext
{
    public ConferenceContext(DbContextOptions<ConferenceContext> options) : base(options) { }

    public DbSet<Attendee> Attendees { get; set; }
    // Add similar DbSet properties for speakers, sessions, tracks, and sponsors
}

public class Attendee
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
}
