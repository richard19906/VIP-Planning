using Microsoft.AspNetCore.Mvc;
using Supabase;
using VIP_Planning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

// Specifieke PDF bibliotheek verwijzingen
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace VIP_Planning.Controllers {
    public class UrenController : Controller {
        private readonly Supabase.Client _supabase;
        public UrenController(Supabase.Client supabase) { _supabase = supabase; }

        public async Task<IActionResult> PersoonlijkeUren(string email, string naam, int maandOffset = 0) {
            var data = await HaalUrenOp(email, maandOffset);
            
            // BEREKENING: Alleen uren die NIET betaald zijn
            double totaalOpenstaand = data.Gefilterd
                .Where(u => !u.IsUitbetaald && u.Status != "Ziek" && u.Status != "Vrij" && u.AantalUren > 0)
                .Sum(u => u.AantalUren);

            ViewBag.TotaalUren = totaalOpenstaand;
            ViewBag.WerknemerNaam = naam;
            ViewBag.WerknemerEmail = email;
            ViewBag.PeriodeLabel = data.Label;
            ViewBag.MaandOffset = maandOffset;

            return View("~/Views/Home/PersoonlijkeUren.cshtml", data.Gefilterd);
        }

        public async Task<IActionResult> ExportPdf(string email, string naam, int maandOffset = 0) {
            var data = await HaalUrenOp(email, maandOffset);
            double openstaand = data.Gefilterd
                .Where(u => !u.IsUitbetaald && u.Status != "Ziek" && u.Status != "Vrij" && u.AantalUren > 0)
                .Sum(u => u.AantalUren);

            using (MemoryStream ms = new MemoryStream()) {
                // iTextSharp logica
                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var fontTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                var fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);

                doc.Add(new Paragraph("V.I.P SECURITY SERVICE - URENVERANTWOORDING", fontTitle));
                doc.Add(new Paragraph($"Naam: {naam} | Periode: {data.Label}", fontNormal));
                doc.Add(new Paragraph(" "));
                
                PdfPTable table = new PdfPTable(4);
                table.WidthPercentage = 100;
                table.AddCell(new PdfPCell(new Phrase("Datum", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Locatie", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Status", fontBold)));
                table.AddCell(new PdfPCell(new Phrase("Uren", fontBold)));
                
                foreach (var u in data.Gefilterd) {
                    table.AddCell(new Phrase(u.DatumString, fontNormal));
                    table.AddCell(new Phrase(u.Locatie, fontNormal));
                    table.AddCell(new Phrase(u.IsUitbetaald ? "BETAALD" : "Open", fontNormal));
                    
                    string urenTekst = u.Status == "Vrij" ? "*" : (u.Status == "Ziek" ? "**" : u.AantalUren.ToString("N1"));
                    table.AddCell(new Phrase(urenTekst, fontNormal));
                }
                doc.Add(table);
                doc.Add(new Paragraph($"\nOPENSTAAND TOTAAL: {openstaand:N1} UUR", fontBold));
                
                doc.Close();
                byte[] bytes = ms.ToArray();
                return File(bytes, "application/pdf", $"Uren_{naam}.pdf");
            }
        }

        private async Task<(List<UrenModel> Gefilterd, string Label)> HaalUrenOp(string email, int offset) {
            DateTime refDate = DateTime.Now.AddMonths(offset);
            DateTime start = new DateTime(refDate.AddMonths(-1).Year, refDate.AddMonths(-1).Month, 21);
            DateTime eind = new DateTime(refDate.Year, refDate.Month, 20);
            
            var response = await _supabase.From<UrenModel>().Get();
            var lijst = response.Models ?? new List<UrenModel>();

            var gefilterd = lijst
                .Where(u => u.UserEmail.ToLower() == email.ToLower()).ToList()
                .Where(u => {
                    DateTime d;
                    return DateTime.TryParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d) && d >= start && d <= eind;
                })
                .OrderBy(u => DateTime.ParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture)).ToList();
                
            return (gefilterd, $" {start:dd/MM} - {eind:dd/MM/yyyy}");
        }

        [HttpPost] public async Task<IActionResult> MarkeerAlsBetaald(long id) {
            await _supabase.From<UrenModel>().Where(x => x.Id == id).Set(x => x.IsUitbetaald, true).Update();
            return Json(new { success = true });
        }
    }
}