using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public interface IContactService
    {
        Task<IList<Contact>> GetContacts(string q = "");
        Task<Contact> AddContact(Contact contact);
        Task<Contact?> GetContact(int id);
        Task<Contact?> UpdateContact(Contact request);
        Task<bool> DeleteContact(int id);
    }

    public class ContactService : IContactService
    {
        private readonly AppDbContext _appDbContext;

        public ContactService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IList<Contact>> GetContacts(string q = "")
        {
            var query = _appDbContext.Contacts.Where(w => !w.IsDeleted);

            if (!string.IsNullOrEmpty(q))
            {
                query = query.Where(
                    w => (w.Name != null && EF.Functions.Like(w.Name.ToLower(), $"%{q.ToLower()}%")) ||
                    (w.Email != null && EF.Functions.Like(w.Email.ToLower(), $"%{q.ToLower()}%")) ||
                    (w.Phone != null && EF.Functions.Like(w.Phone.ToLower(), $"%{q.ToLower()}%")) ||
                    (w.Address != null && EF.Functions.Like(w.Address.ToLower(), $"%{q.ToLower()}%"))
                );
            }

            return await query.OrderBy(o => o.Id)
                .Select(s => new Contact
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address
                })
                .ToListAsync();
        }

        public async Task<Contact> AddContact(Contact contact)
        {
            _appDbContext.Contacts.Add(contact);
            await _appDbContext.SaveChangesAsync();

            return contact;
        }

        public async Task<Contact?> GetContact(int id)
        {
            if (id <= 0)
            {
                return null;
            }

            var contact = await _appDbContext.Contacts
                .Where(w => w.Id == id && !w.IsDeleted)
                .OrderBy(o => o.Id)
                .Select(s => new Contact
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email,
                    Phone = s.Phone,
                    Address = s.Address
                })
                .FirstOrDefaultAsync();

            return contact;
        }

        public async Task<Contact?> UpdateContact(Contact request)
        {
            if (request.Id <= 0)
            {
                return null;
            }

            var contact = await _appDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (contact == null)
            {
                return null;
            }

            contact.Address = request.Address;
            contact.Phone = request.Phone;
            contact.Email = request.Email;
            contact.Name = request.Name;

            await _appDbContext.SaveChangesAsync();

            return contact;
        }

        public async Task<bool> DeleteContact(int id)
        {
            if (id == 0)
            {
                return false;
            }

            var contact = await _appDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (contact == null)
            {
                return false;
            }

            // We are using soft-delete approach instead of hard deletion
            contact.IsDeleted = true;

            await _appDbContext.SaveChangesAsync();

            return true;
        }
    }
}
