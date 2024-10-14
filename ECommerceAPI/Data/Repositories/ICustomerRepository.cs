using AutoMapper;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ECommerceAPI.Data.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int customerId);
        Task<int> InsertCustomerAsync(CustomerDTO customer);
        Task UpdateCustomerAsync(CustomerDTO customer);
        Task DeleteCustomerAsync(int customerId);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly EcommerceContext _context;
        private readonly IMapper _mapper;

        public CustomerRepository(EcommerceContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Method to return All Customer
        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        //Method to Find a Customer By Id
        public async Task<Customer?> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        //Method to Add a New Customer and return the created Customer ID
        public async Task<int> InsertCustomerAsync(CustomerDTO customer)
        {
            Customer newCustomer = _mapper.Map<Customer>(customer);
            newCustomer.IsDeleted = false;
            await _context.Customers.AddAsync(newCustomer);
            customer.CustomerId = newCustomer.CustomerId;
            await _context.SaveChangesAsync();
            return customer.CustomerId;
        }

        //Method to Update an Existing Customer
        public async Task UpdateCustomerAsync(CustomerDTO customer)
        {

            var existingCustomer = await GetCustomerByIdAsync(customer.CustomerId);
            existingCustomer.IsDeleted = false;
            customer.CustomerId = existingCustomer.CustomerId;
            _mapper.Map(customer, existingCustomer);
            await _context.SaveChangesAsync();

        }

        //Method to Delete an Existing Customer
        public async Task DeleteCustomerAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            customer.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
    }


}
