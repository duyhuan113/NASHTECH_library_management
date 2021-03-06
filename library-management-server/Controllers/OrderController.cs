using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using R5.Models;
using R5.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace R5.Controllers
{
    [Route("orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderRepository _repository;
        private readonly IBookRepository _bookRepository;

        public OrderController(IOrderRepository repository, IBookRepository bookRepository)
        {
            _repository = repository;
            _bookRepository = bookRepository;
        }

        // GET: api/<BookController>
        [Authorize(Roles = "admin")]
        [HttpGet("all")]
        public ActionResult<List<Order>> Get()
        {
            try
            {
                var list = _repository.GetAll(b => b.OrderDetails).ToList();

                if (list != null) return list;

                return BadRequest("Canot Found.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data");
            }
        }

        [Authorize(Roles = "user")]
        [HttpGet("{Id}")]
        public ActionResult<List<Order>> GetAllByUserId(int Id)
        {
            try
            {
                if (Id == null)
                {
                    return BadRequest("ID Invalid! ");
                }
                var listOrder = _repository.GetAll(o => o.OrderDetails).Where(o => o.UserId == Id).ToList();

                if (listOrder != null)
                {
                    return listOrder;
                }
                return BadRequest("Cannot Found");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data");
            }
        }

        [Authorize(Roles = "user,admin")]
        [HttpGet("detail/{Id}")]
        public ActionResult<Order> GetOneById(int Id)
        {
            try
            {
                if (Id != null)
                {
                    Console.WriteLine(Id);

                    var listOrder = _repository.GetOneById(Id);

                    if (listOrder != null)
                    {
                        return listOrder;
                    }
                    return (Order)BadRequest("Cannot Found");

                }
                return (Order)BadRequest("ID Invalid! ");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error Retrieving Data");
            }
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public ActionResult CreateOrder(Order model)
        {
            try
            {
                var OrderPerMonth = _repository.GetAll().Count(o => o.UserId == model.UserId && o.CreatedDate.Month == DateTime.Now.Month);
                Console.WriteLine(OrderPerMonth);

                if (model.OrderDetails.Count() > 5) return BadRequest("Cannot borrow more than 5 books");

                if (OrderPerMonth > 300) return BadRequest("The number of order / month has exceeded");

                model.CreatedDate = DateTime.Now;
                _repository.Insert(model);

                foreach (var item in model.OrderDetails)
                {
                    var book = _bookRepository.UpdateQuantity(true, item.BookId, item.ItemQuantity);
                    if (book != null)
                    {
                        _bookRepository.Update(book);
                    }
                    else
                    {
                        return BadRequest("Book Not Available");
                    }
                }
                return Ok("Success, please check your history.");
            }
            catch (Exception ex)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public ActionResult Put(int Id, Order model)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest();
                var order = _repository.Get(Id);
                if (order != null)
                {
                    order.Status = model.Status;
                    order.ModifiedDate = DateTime.Now;

                    _repository.Update(order);
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return new StatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
        }
    }
}