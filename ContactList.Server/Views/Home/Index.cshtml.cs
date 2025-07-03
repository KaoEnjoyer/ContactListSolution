using ContactList.Shared.Models;
using ContactList.Contacts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContactList.Server.Views.Home;

public class Index : PageModel
{
    private readonly ContactsDatabase _contactService;

    public void OnGet()
    {
        ViewData["Title"] = "Lista kontaktów";
    }
    public Index(ContactsDatabase contactService)
    {
        ViewData["Title"] = "Lista kontaktów";

        _contactService = contactService;
    }

    public List<Contact> Contacts { get; set; }

    public async Task OnGetAsync()
    {
        Contacts = _contactService.SelectAll();
    }
    

    public async Task<IActionResult> OnPostDeleteAsync(string id)
    {
        _contactService.Delete(id);
        return RedirectToPage();
    }
}
