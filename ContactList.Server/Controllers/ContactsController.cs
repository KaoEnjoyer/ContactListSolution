using ContactList.Server.Data;
using ContactList.Shared.Dto;
using ContactList.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ContactList.Server.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class ContactsController : ControllerBase
    //{

    //    private readonly ContactListDbContext _context;

    //    public ContactsController(ContactListDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet]
    //    [AllowAnonymous]
    //    public async Task<ActionResult<IEnumerable<ContactDto>>> GetAllContacts()
    //    {
    //        var contacts = await _context.Contacts
    //            .Select(c => new ContactDto
    //            {
    //                Id = c.Id,
    //                FirstName = c.FirstName,
    //                LastName = c.LastName,
    //                Email = c.Email,
    //                Phone = c.Phone,
    //                BirthDate = c.BirthDate,
    //                Category = c.Category,
    //                Subcategory = c.Subcategory
    //            })
    //            .ToListAsync();

    //        return Ok(contacts);
    //    }

    //    [HttpGet("{id}")]
    //    [AllowAnonymous]
    //    public async Task<ActionResult<ContactDto>> GetById(int id)
    //    {
    //        var c = await _context.Contacts.FindAsync(id);
    //        if (c == null) return NotFound();

    //        return Ok(new ContactDto
    //        {
    //            Id = c.Id,
    //            FirstName = c.FirstName,
    //            LastName = c.LastName,
    //            Email = c.Email,
    //            Phone = c.Phone,
    //            BirthDate = c.BirthDate,
    //            Category = c.Category,
    //            Subcategory = c.Subcategory
    //        });
    //    }

    //    [HttpPost]
    //    [Authorize]
    //    public async Task<IActionResult> Create(ContactDto dto)
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

    //        // Sprawdź czy email nie istnieje
    //        if (await _context.Contacts.AnyAsync(c => c.Email == dto.Email))
    //            return BadRequest("Email already used.");

    //        var contact = new Contact
    //        {
    //            FirstName = dto.FirstName,
    //            LastName = dto.LastName,
    //            Email = dto.Email,
    //            Phone = dto.Phone,
    //            BirthDate = dto.BirthDate,
    //            Category = dto.Category,
    //            Subcategory = dto.Subcategory,
    //            AppUserId = userId
    //        };

    //        _context.Contacts.Add(contact);
    //        await _context.SaveChangesAsync();

    //        return Ok("Contact created.");
    //    }

    //    [HttpPut("{id}")]
    //    [Authorize]
    //    public async Task<IActionResult> Update(int id, ContactDto dto)
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        var contact = await _context.Contacts.FindAsync(id);
    //        if (contact == null) return NotFound();
    //        if (contact.AppUserId != userId) return Forbid();

    //        // Edytuj dane
    //        contact.FirstName = dto.FirstName;
    //        contact.LastName = dto.LastName;
    //        contact.Email = dto.Email;
    //        contact.Phone = dto.Phone;
    //        contact.BirthDate = dto.BirthDate;
    //        contact.Category = dto.Category;
    //        contact.Subcategory = dto.Subcategory;

    //        await _context.SaveChangesAsync();
    //        return Ok("Contact updated.");
    //    }

    //    // 5. Usuń kontakt (tylko właściciel)
    //    [HttpDelete("{id}")]
    //    [Authorize]
    //    public async Task<IActionResult> Delete(int id)
    //    {
    //        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    //        var contact = await _context.Contacts.FindAsync(id);
    //        if (contact == null) return NotFound();
    //        if (contact.AppUserId != userId) return Forbid();

    //        _context.Contacts.Remove(contact);
    //        await _context.SaveChangesAsync();

    //        return Ok("Contact deleted.");
    //    }
    //}
}
