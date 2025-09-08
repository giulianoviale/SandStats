using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using SandStats.Data;

namespace SandStats.Pages.Admin.Users
{
    public class CreateModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CreateModel(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; } = string.Empty;

            [DataType(DataType.Text)]
            public string? FullName { get; set; }

            [Required, DataType(DataType.Password), MinLength(6)]
            public string Password { get; set; } = string.Empty;

            [Required]
            public string Role { get; set; } = "Coach";

            public bool EmailConfirmed { get; set; } = true;
            public bool IsActive { get; set; } = true; // si usás el campo opcional
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // Asegurar rol
            if (!await _roleManager.RoleExistsAsync(Input.Role))
            {
                var created = await _roleManager.CreateAsync(new IdentityRole(Input.Role));
                if (!created.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "No se pudo crear el rol.");
                    return Page();
                }
            }

            var user = new ApplicationUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = Input.EmailConfirmed,
                // FullName = Input.FullName,      // descomentar si agregaste el campo
                // IsActive = Input.IsActive,      // descomentar si agregaste el campo
            };

            var res = await _userManager.CreateAsync(user, Input.Password);
            if (!res.Succeeded)
            {
                foreach (var e in res.Errors) ModelState.AddModelError(string.Empty, e.Description);
                return Page();
            }

            await _userManager.AddToRoleAsync(user, Input.Role);
            TempData["Ok"] = $"Usuario {Input.Email} creado.";
            return RedirectToPage("/Index");
        }
    }
}
