using EventParticipationApp.Data;
using EventParticipationApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System; // Exception için gerekli
using System.Threading.Tasks;

namespace EventParticipationApp.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Participations) // Katılımları da dahil etmeyi unutmayın
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,EventDate")] Event @event)
        {
            // ModelState.IsValid kontrolü
            if (ModelState.IsValid)
            {
                try
                {
                 

                    _context.Add(@event);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Etkinlik başarıyla eklendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Hata durumunda (SaveChangesAsync hatası)
                    TempData["ErrorMessage"] = "Etkinlik eklenirken bir hata oluştu: " + ex.Message;
                    Console.WriteLine("Hata Detayı (Events/Create): " + ex.ToString()); // Konsola hatayı yazdırır
                    ModelState.AddModelError("", "Etkinlik eklenirken bir hata oluştu: " + ex.Message);
                }
            }
            else // ModelState.IsValid false döndüğünde buraya girecek
            {
                // Bu kısım, View'daki formdan gelen verilerin model'e doğru bağlanıp bağlanmadığını kontrol eder.
                Console.WriteLine("ModelState geçerli değil (Events/Create). Hatalar:");
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine("- " + error.ErrorMessage);
                    }
                }
                TempData["ErrorMessage"] = "Formda eksik veya hatalı bilgi var. Lütfen kontrol ediniz."; // Tarayıcıya genel hata mesajı
            }

            // Validasyon hatası varsa veya catch bloğuna düşerse formu tekrar göster
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,EventDate")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Etkinlik başarıyla güncellendi!"; // Güncelleme mesajı
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = "Etkinlik güncellenirken bir hata oluştu. Lütfen kontrol ediniz."; // Güncelleme hata mesajı
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // İlişkili katılımları da yüklemek için Include kullandık
                var @event = await _context.Events
                                    .Include(e => e.Participations)
                                    .FirstOrDefaultAsync(e => e.Id == id);

                if (@event == null)
                {
                    TempData["ErrorMessage"] = "Silinecek etkinlik bulunamadı!";
                    return RedirectToAction(nameof(Index));
                }

                // Önce ilişkili katılımları sil
                _context.Participations.RemoveRange(@event.Participations);

                // Sonra etkinliği sil
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Etkinlik ve ilgili tüm katılımcıları başarıyla silindi!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Hata oluşursa kullanıcıya mesaj göster ve konsola detayları yazdır
                TempData["ErrorMessage"] = "Etkinlik silinirken bir hata oluştu: " + ex.Message;
                Console.WriteLine("Hata Detayı (Events/DeleteConfirmed): " + ex.ToString());
                return RedirectToAction(nameof(Index)); // Hata durumunda Index'e geri yönlendir
            }
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
