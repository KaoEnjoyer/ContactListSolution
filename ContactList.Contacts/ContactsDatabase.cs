using ContactList.Shared.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using ContactList.Exceptions;
using Exceptions;
using MongoDB.Driver;

namespace ContactList.Contacts;

public class ContactsDatabase : IContactsDatabase
{
    private readonly IConfiguration configuration;

    private const string Salt = "neNaXSodvl";

    public ContactsDatabase(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    private IMongoCollection<Contact> GetContactsCollection()
    {
        var client = new MongoClient(this.configuration.GetConnectionString("ContactList"));
        var collection = client.GetDatabase("ContactList").GetCollection<Contact>("ContactList.Contacts");
        if (collection == null)
            throw new DbConnectionFailed("Contact list not found");
        return client.GetDatabase("ContactList").GetCollection<Contact>("ContactList.Contacts");
    }

    public List<Contact> SelectAll()
    {
        var collection = GetContactsCollection();
        List<Contact> contacts = collection.Find(_ => true).ToList();
        return contacts;
    }

    public List<Contact> SelectAllAsync()
    {
        var collection = GetContactsCollection();
        List<Contact> contacts = collection.FindAsync(_ => true).Result.ToList();
        if (collection == null)
        {
            throw new NotFoundException("Contact not found");
        }
        return contacts;
    }

    public Contact Select(string id)
    {
        var collection = GetContactsCollection();
        Contact contact = collection.Find(_ => true).ToList().FirstOrDefault();
        if (contact == null)
            throw new NotFoundException("Contact not found");
        return contact;
    }

    public Contact Edit(string id, Contact new_contact)
    {
        var collection = GetContactsCollection();
        Contact contact = collection.Find(c => c.Id == id).ToList().FirstOrDefault();
        if (contact == null)
            throw new NotFoundException("Contact not found");
        contact.FirstName = new_contact.FirstName;
        contact.LastName = new_contact.LastName;
        contact.Email = new_contact.Email;
        contact.Phone = new_contact.Phone;
        contact.BirthDate = new_contact.BirthDate;
        contact.Category = new_contact.Category;
        contact.Subcategory = new_contact.Subcategory;

        throw new NotImplementedException();
    }

    public void Delete(string id)
    {
        var collection = GetContactsCollection();
        collection.DeleteOne(c => c.Id == id);
    }

    public Contact AddContact(Contact new_contact)
    {
        var collection = GetContactsCollection();
        collection.InsertOne(new_contact);
        return new_contact;
    }
}