using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Go_grocery.Data;
using Go_grocery.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Routing;

namespace Go_grocery.Controllers
{
    public class CourierController : Controller
    {
        private readonly Go_groceryContext _context;
        public static int currentcourier;
        public List<Orderdet> Currentord = new List<Orderdet>();
        public IActionResult Login()
        {
            return View();
        }


        public async Task<IActionResult> Orders(int id)
        {

            List<Order> orders = await _context.Order.ToListAsync();
            currentcourier = id;
            var courier = await _context.Courier.FirstOrDefaultAsync(x => x.Id == currentcourier);
            foreach (Order ord in orders)
            {
                if (ord.CourierId == currentcourier)
                {
                    var product = await _context.Product.FirstOrDefaultAsync(x => x.Id == ord.ProductId);
                    var customer = await _context.Customer.FirstOrDefaultAsync(x => x.Id == ord.CustomerId);
                    Orderdet orderdet = new Orderdet();
                    orderdet.Id = ord.Id;
                    orderdet.couriername = courier.Fullname;
                    orderdet.productname = product.Name;
                    orderdet.customername = customer.Fullname;
                    Currentord.Add(orderdet);
                }
            }
            return View(Currentord);


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] Courier courier)
        {
            var info = _context.Courier.FirstOrDefault(x => x.Username == courier.Username && x.Password == courier.Password);

            if (info != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,courier.Username)
                };
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                currentcourier = info.Id; 
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error");
            }

        }

        public async Task<IActionResult> Deliver(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Order.FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
        // POST: Courier/Delete/5
        [HttpPost, ActionName("Deliver")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeliveryConfirmed(int id)
        {
            var order = await _context.Order.FindAsync(id);
            _context.Order.Remove(order);
            foreach(Orderdet item in Currentord)
            {
                if(item.Id == id)
                {
                    Currentord.Remove(item);
                    break;
                }

            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Orders", new RouteValueDictionary(
                                     new { controller = "Courier", action = "Orders", Id = currentcourier }));
        }



        public CourierController(Go_groceryContext context)
        {
            _context = context;
        }

        // GET: Courier

        public async Task<IActionResult> Index()
        {
            return View(await _context.Courier.ToListAsync());
        }

        // GET: Courier/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courier == null)
            {
                return NotFound();
            }

            return View(courier);
        }

        // GET: Courier/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courier/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname,PhoneNumber,Birthyear,Address,Username,Password")] Courier courier)
        {
            if (ModelState.IsValid)
            {
                List<Courier> crs = await _context.Courier.ToListAsync();
                foreach(var cr in crs)
                {
                    if(cr.Username == courier.Username)
                    {
                        return View("Error");
                    }
                }
                _context.Add(courier);
                await _context.SaveChangesAsync();
                return RedirectPreserveMethod("~/Home/Login");
            }
            return View(courier);
        }

        // GET: Courier/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier.FindAsync(id);
            if (courier == null)
            {
                return NotFound();
            }
            return View(courier);
        }



        // POST: Courier/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,PhoneNumber,Birthyear,Address,Username,Password")] Courier courier)
        {
            if (id != courier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(courier);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourierExists(courier.Id))
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
            return View(courier);
        }

        // GET: Courier/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var courier = await _context.Courier
                .FirstOrDefaultAsync(m => m.Id == id);
            if (courier == null)
            {
                return NotFound();
            }

            return View(courier);
        }

        // POST: Courier/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var courier = await _context.Courier.FindAsync(id);
            _context.Courier.Remove(courier);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourierExists(int id)
        {
            return _context.Courier.Any(e => e.Id == id);
        }

    }
}
