namespace ContactList.Contacts;
using ContactList.Shared.Models;
public interface IContactsDatabase
{

    List<Contact> SelectAll();
    List<Contact> SelectAllAsync();

    Contact Select(string id);
    
    Contact Edit(string id, Contact new_contact);
    
    void Delete(string id);
    
    Contact AddContact(Contact new_contact);
}