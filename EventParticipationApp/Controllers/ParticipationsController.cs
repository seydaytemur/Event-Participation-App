using Microsoft.AspNetCore.Mvc;
using EventParticipationApp.Data;
using Microsoft.EntityFrameworkCore;
using EventParticipationApp.Models;
using System.Threading.Tasks;

namespace EventParticipationApp.Controllers
{
    public class ParticipationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ParticipationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Participations/Create/5 (EventId)
        public IActionResult Create(int eventId)
        {
            // Etkinliği veritabanından bul
            var @event = _context.Events.Find(eventId);

            // Etkinlik yoksa 404 hatası döndür
            if (@event == null)
            {
                return NotFound();
            }

            // View'e veri taşımak için ViewBag kullan
            ViewBag.EventTitle = @event.Title;
            ViewBag.EventId = eventId;

            return View();
        }

        // GET: Participations/ByParticipant
        public IActionResult ByParticipant()
        {
            return View();
        }

        // POST: Participations/ByParticipant
        [HttpPost]
        public async Task<IActionResult> ByParticipant(string participantName)
        {
            var participations = await _context.Participations
                .Include(p => p.Event)
                .Where(p => p.ParticipantName == participantName)
                .OrderBy(p => p.Event.EventDate)
                .ToListAsync();

            ViewBag.ParticipantName = participantName;
            return View(participations);
        }

        // POST: Participations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ParticipantName,EventId")] Participation participation)
        {
            // 1. Aynı kişinin aynı etkinliğe tekrar katılımını kontrol et
            var existingParticipation = await _context.Participations
                .FirstOrDefaultAsync(p => p.EventId == participation.EventId &&
                                         p.ParticipantName == participation.ParticipantName);

            if (existingParticipation != null)
            {
                TempData["ErrorMessage"] = "Bu etkinliğe zaten katıldınız!";
                return RedirectToAction("Details", "Events", new { id = participation.EventId });
            }

            // 2. Model doğrulaması
            if (ModelState.IsValid)
            {
                try
                {
                    // 3. Katılımı veritabanına ekle
                    _context.Add(participation);
                    await _context.SaveChangesAsync();

                    // 4. Başarı mesajı ayarla
                    TempData["SuccessMessage"] = "Etkinliğe başarıyla katıldınız!";

                    // 5. Etkinlik detay sayfasına yönlendir
                    return RedirectToAction("Details", "Events", new { id = participation.EventId });
                }
                catch (Exception ex)
                {
                    // 6. Hata durumunda (_context.SaveChangesAsync() hatası)
                    TempData["ErrorMessage"] = "Kayıt sırasında bir hata oluştu: " + ex.Message;
                    Console.WriteLine("Hata Detayı: " + ex.ToString()); // Konsola hatayı yazdırır
                    ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                }
            }
            else // ModelState.IsValid false döndüğünde buraya girecek
            {
                // Bu kısım, View'daki formdan gelen verilerin model'e doğru bağlanıp bağlanmadığını kontrol eder.
                Console.WriteLine("ModelState geçerli değil. Hatalar:");
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        Console.WriteLine("- " + error.ErrorMessage);
                    }
                }
                TempData["ErrorMessage"] = "Formda eksik veya hatalı bilgi var. Lütfen kontrol ediniz."; // Tarayıcıya genel hata mesajı
            }

            // 7. Validasyon hatası varsa formu tekrar göster
            var @event = await _context.Events.FindAsync(participation.EventId);
            ViewBag.EventTitle = @event?.Title;
            return View(participation);
        }
    }
}