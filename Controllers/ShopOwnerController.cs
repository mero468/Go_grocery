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
    public class ShopOwnerController : Controller
    {
        private readonly Go_groceryContext _context;
        private static int currentshopowner;

        public ShopOwnerController(Go_groceryContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind("Username,Password")] ShopOwner Shopowner)
        {
            var info = _context.ShopOwner.FirstOrDefault(x => x.Username == Shopowner.Username && x.Password == Shopowner.Password);

            if (info != null)
            {

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,Shopowner.Username)
                };
                currentshopowner = info.Id;
                var useridentity = new ClaimsIdentity(claims, "Login");
                ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                await HttpContext.SignInAsync(principal);
                return RedirectToAction("Inventory", new RouteValueDictionary(
                          new { controller = "ShopOwner", action = "Inventory", Id = currentshopowner }));

            }
            else
            {
                return View("Error");
            }

        }

        public async Task<IActionResult> Inventory(int id)
        {

            var info = await _context.ShopOwner.FirstOrDefaultAsync(x => x.Id == id);

            List<Product> products = _context.Product.ToList();
            List<Product> Inventory = new List<Product>();

            foreach (Product product in products)
            {
                if (product.ShopOwnerID == id)
                {
                    Inventory.Add(product);
                }
            }


            return View(Inventory);
        }


        // GET: ShopOwner
        public async Task<IActionResult> Index()
        {
            return View(await _context.ShopOwner.ToListAsync());
        }

        public async Task<IActionResult> DetailsInv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }


        // GET: ShopOwner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopOwner == null)
            {
                return NotFound();
            }

            return View(shopOwner);
        }

        // GET: ShopOwner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ShopOwner/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Fullname,PhoneNumber,Address,ShopName,Username,Password")] ShopOwner shopOwner)
        {
            if (ModelState.IsValid)
            {
                List<ShopOwner> shos = await _context.ShopOwner.ToListAsync();
                foreach (var sho in shos)
                {
                    if (sho.Username == shopOwner.Username)
                    {
                        return View("Error");
                    }
                }
                _context.Add(shopOwner);
                await _context.SaveChangesAsync();
                return RedirectPreserveMethod("~/Home/Login");
            }
            return View(shopOwner);
        }

        public IActionResult CreateInv()
        {
            return View();
        }

        // POST: Products/Signup
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInv(int id, [Bind("Name,CategoryId,Price,Weight")] Product product)
        {
            if (ModelState.IsValid)
            {
                product.ShopOwnerID = id; 
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction("Inventory", new RouteValueDictionary(
                             new { controller = "shopowner", action = "Inventory", Id = id }));
            }
            return View(product);
        }

        // GET: ShopOwner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwner.FindAsync(id);
            if (shopOwner == null)
            {
                return NotFound();
            }
            return View(shopOwner);
        }

        // POST: ShopOwner/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Fullname,PhoneNumber,Address,ShopName,Username,Password")] ShopOwner shopOwner)
        {
            if (id != shopOwner.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shopOwner);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShopOwnerExists(shopOwner.Id))
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
            return View(shopOwner);
        }

        public async Task<IActionResult> EditInv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInv(int id, [Bind("Id,Name,Price,Weight,CategoryId,ShopOwnerID")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Inventory", new RouteValueDictionary(
                             new { controller = "shopowner", action = "Inventory", Id = currentshopowner }));
            }
            return View(product);
        }

        // GET: ShopOwner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var shopOwner = await _context.ShopOwner
                .FirstOrDefaultAsync(m => m.Id == id);
            if (shopOwner == null)
            {
                return NotFound();
            }

            return View(shopOwner);
        }

        // POST: ShopOwner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shopOwner = await _context.ShopOwner.FindAsync(id);
            _context.ShopOwner.Remove(shopOwner);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteInv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: ShopOwner/Delete/5
        [HttpPost, ActionName("DeleteInv")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteInvConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            _context.Product.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction("Inventory", new RouteValueDictionary(
                         new { controller = "shopowner", action = "Inventory", Id = currentshopowner }));
        }



        private bool ShopOwnerExists(int id)
        {
            return _context.ShopOwner.Any(e => e.Id == id);
        }
        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.Id == id);
        }

    }
}
