using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Go_grocery.Data;
using Go_grocery.Models;
using System.Security.Claims;
using System.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Routing;
using System;

namespace Go_grocery.Controllers
{
    public class CustomersController : Controller
    {
        private readonly Go_groceryContext _context;
        private static List<Product> basket = new List<Product>();
        private static int currentcustomer;
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] Customer customer)
        {
            var info = _context.Customer.FirstOrDefault(x => x.Username == customer.Username && x.Password == customer.Password);

            
            if (info != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,customer.Username)
                };
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);

                currentcustomer = info.Id;
                return RedirectToAction("Products", new RouteValueDictionary(
                           new { controller = "Customers", action = "Products", Id = currentcustomer }));

            }
            else
            {
                return View("Error");
            }

        }

        public async Task<IActionResult> Products()

        {
            return View(_context.Product.ToList());
        }



        public CustomersController(Go_groceryContext context)
        {
            _context = context;
        }

        // GET: Customers

        public async Task<IActionResult> Index()
        {
            return View(await _context.Customer.ToListAsync());
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Signup
        public IActionResult Create()
        {
            return View();
        }

        // POST: Customers/Signup
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname,PhoneNumber,Address,Mail,Username,Password")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                List<Customer> cuss = await _context.Customer.ToListAsync();
                foreach (var cus in cuss)
                {
                    if (cus.Username == customer.Username)
                    {
                        return View("Error");
                    }
                }
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectPreserveMethod("~/Home/Login"); 
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,PhoneNumber,Address,Mail,Username,Password")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            _context.Customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CustomerExists(int id)
        {
            return _context.Customer.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Checkout(int id) 
        {
            List<Courier> couriers = await _context.Courier.ToListAsync();
            
            foreach(var item in basket)
            {
                Order ord = new Order();
                var random = new Random();
                int index = random.Next(couriers.Count);
                Courier courier = couriers[index];
                ord.CourierId = courier.Id;
                ord.CustomerId = currentcustomer;
                ord.ProductId = item.Id;
                _context.Add(ord); 
                await _context.SaveChangesAsync();
            }
            Customer customer  = _context.Customer.FirstOrDefault(x => x.Id == currentcustomer);


            basket.Clear();
            return View(customer);

        }
        public async Task<IActionResult>  AddtoBskt(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return View("Error");
            }
            else
            {
               
                basket.Add(product);
                return RedirectToAction("Products", new RouteValueDictionary(
                            new { controller = "Customers", action = "Products", Id = currentcustomer }));

            }
        }

        public async Task<IActionResult> ViewBasket(int id)
        {

            return View(basket);

        }

        public async Task<IActionResult> DeleteItm(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var product = await _context.Product
               .FirstOrDefaultAsync(m => m.Id == id);
            if(product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("DeleteItm")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteItmConfirmed(int id)
        {
            foreach(var item in basket)
            {
                if(item.Id == id)
                {
                    basket.Remove(item);
                    break;
                }

            }
            return RedirectToAction("ViewBasket", new RouteValueDictionary(
                            new { controller = "Customers", action = "ViewBasket", Id = currentcustomer }));
        }
    }
}
