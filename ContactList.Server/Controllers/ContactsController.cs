using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ContactList.Shared.Dto;
using ContactList.Shared.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using ContactList.Contacts;
using System.Text.Json;

namespace ContactList.Server.Controllers
{
    // Controller for managing contacts
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactsDatabase _database; // Dependency for interacting with contact storage

        // Constructor for dependency injection
        public ContactsController(IContactsDatabase database)
        {
            _database = database;
        }

        // Endpoint to retrieve all contacts (accessible to all users)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetAllContacts()
        {
            // Fetch all contacts from the database
            var contacts = _database.SelectAll();

            // Return the list of contacts as a serialized JSON object
            return Ok(JsonSerializer.Serialize(contacts));
        }

        // Endpoint to create a new contact (requires user authentication)
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ContactDto dto)
        {
            // Fetch the user's ID from the claims (authentication context)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // If the user is not logged in, return an Unauthorized response
            if (userId == null)
            {
                return Unauthorized("User is not logged in.");
            }

            // Map the incoming DTO (data transfer object) to the domain model
            var contact = new Contact
            {
                FirstName = dto.FirstName,   // Contact's first name
                LastName = dto.LastName,     // Contact's last name
                Email = dto.Email,           // Contact's email
                Phone = dto.Phone,           // Contact's phone number
                BirthDate = dto.BirthDate,   // Contact's birthdate
                Category = dto.Category,     // Category assigned to the contact
                Subcategory = dto.Subcategory // Subcategory within the category
            };

            // Add the contact to the database
            _database.AddContact(contact);

            // Return success response
            return Ok("Contact added successfully.");
        }

        // Endpoint to update an existing contact (requires user authentication)
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] ContactDto dto)
        {
            // Fetch the user's ID from the claims (authentication context)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the existing contact from the database by ID
            var contact = _database.Select(id.ToString());

            // If the contact is not found, return a 404 response
            if (contact == null) 
                return NotFound("Contact not found.");

            // Update the contact's properties with values from the DTO
            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.Email = dto.Email;
            contact.Phone = dto.Phone;
            contact.BirthDate = dto.BirthDate;
            contact.Category = dto.Category;
            contact.Subcategory = dto.Subcategory;

            // Save the updated contact in the database
            _database.Edit(id.ToString(), contact);

            // Return success response
            return Ok("Contact updated successfully.");
        }

        // Endpoint to delete a contact (requires user authentication)
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            // Fetch the user's ID from the claims (authentication context)
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Fetch the contact from the database by ID
            var contact = _database.Select(id.ToString());

            // If the contact is not found, return a 404 response
            if (contact == null) 
                return NotFound("Contact not found.");

            // Delete the contact from the database
            _database.Delete(id.ToString());

            // Return success response
            return Ok("Contact deleted successfully.");
        }
    }
}