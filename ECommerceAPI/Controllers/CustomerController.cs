using AutoMapper;
using ECommerceAPI.Data;
using ECommerceAPI.Data.Repositories;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _imapper;
        private readonly EcommerceContext _context;

        public CustomerController(ICustomerRepository customerRepository, IMapper mapper, EcommerceContext context)
        {
            _customerRepository = customerRepository;
            _imapper = mapper;
            _context = context;
        }

        // GET: api/customer
        [HttpGet("All")]
        public async Task<APIResponse<List<Customer>>> GetAllCustomers()
        {
            try
            {
                var customers = await _customerRepository.GetAllCustomersAsync();
                return new APIResponse<List<Customer>>(customers, "Retrieved all customers successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<Customer>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // GET: api/customer/5
        [HttpGet("ById/{id}")]
        public async Task<APIResponse<Customer>> GetCustomerById(int id)
        {
            try
            {
                var customer = await _customerRepository.GetCustomerByIdAsync(id);
                if (customer == null)
                {
                    return new APIResponse<Customer>(HttpStatusCode.NotFound, "Customer not found.");
                }
                return new APIResponse<Customer>(customer, "Customer retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Customer>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // POST: api/customer
        [HttpPost("Create")]
        public async Task<APIResponse<CustomerResponseDTO>> CreateCustomer([FromBody] CustomerDTO customerDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            try
            {
                var newCustomerId = await _customerRepository.InsertCustomerAsync(customerDto);
                customerDto.CustomerId = _context.Customers.Count() + 1 ;
                var ResponseDTO = _imapper.Map<CustomerResponseDTO>(customerDto);
                return new APIResponse<CustomerResponseDTO>(ResponseDTO, "Customer Added successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<CustomerResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }

        // PUT: api/customer/5
        [HttpPut("{id}/Update")]
        public async Task<APIResponse<bool>> UpdateCustomer(int id, [FromBody] CustomerDTO customerDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }

            if (id != customerDto.CustomerId)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Mismatched Customer ID");
            }

            try
            {
                var existingCustomer = await _customerRepository.GetCustomerByIdAsync(customerDto.CustomerId);
                if (existingCustomer == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Customer not found.");
                }
               
                await _customerRepository.UpdateCustomerAsync(customerDto);
                return new APIResponse<bool>(true, "Customer Updated Successfully.");
            }
            
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);

            }
        }

        // DELETE: api/customer/5
        [HttpDelete("{id}/Delete")]
        public async Task<APIResponse<bool>> DeleteCustomer(int id)
        {
            try
            {
                var Customer = await _customerRepository.GetCustomerByIdAsync(id);

                if (Customer == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Customer not found.");
                }

                await _customerRepository.DeleteCustomerAsync(id);
                return new APIResponse<bool>(true, "Customer Deleted Successfully.");
            }

            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
            
        }
    }
}
