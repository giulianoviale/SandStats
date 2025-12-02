using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SandStats.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IWebHostEnvironment _env;

        public IndexModel(IWebHostEnvironment env)
        {
            _env = env;
        }

        [BindProperty]
        public IFormFile? BackgroundImage { get; set; }

        public string CacheBuster { get; set; } = DateTime.UtcNow.Ticks.ToString();

        public string? Message { get; set; }

        public void OnGet()
        {
            // Nada especial, solo mostramos la página.
        }

        public async Task<IActionResult> OnPostUploadBackground()
        {
            if (BackgroundImage == null || BackgroundImage.Length == 0)
            {
                Message = "Por favor seleccioná una imagen.";
                return Page();
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(BackgroundImage.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                Message = "Formato no válido. Solo se permiten JPG y PNG.";
                return Page();
            }

            if (BackgroundImage.Length > 3 * 1024 * 1024)
            {
                Message = "La imagen es muy grande. Máximo 3 MB.";
                return Page();
            }

            var imgFolder = Path.Combine(_env.WebRootPath, "img");
            if (!Directory.Exists(imgFolder))
            {
                Directory.CreateDirectory(imgFolder);
            }

            var filePath = Path.Combine(imgFolder, "home-bg.jpg");

            // Guardamos / reemplazamos
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await BackgroundImage.CopyToAsync(stream);
            }

            // Para refrescar caché
            CacheBuster = DateTime.UtcNow.Ticks.ToString();
            Message = "Imagen actualizada correctamente.";

            return Page();
        }
    }
}
