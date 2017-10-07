using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class AccountController : Controller
    {
        public SignInManager<ApplicationUser> _signInManager { get; set; }

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpGet]
        [Route("/admin")]
        public IActionResult Login()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return View(new LoginViewModel()); 
            }
            else
            {
                return RedirectToAction("CarList", "Cars");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/admin")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName,
                    model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // TODO: Reroute to admin parts
                    return RedirectToAction("CarList", "Cars");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Fel E-post eller Lösenord.");
                    return View(model);
                }
            }

            
            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToLocal("/");
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}