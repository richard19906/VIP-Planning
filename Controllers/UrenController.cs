using Microsoft.AspNetCore.Mvc;
using Supabase;
using VIP_Planning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace VIP_Planning.Controllers
{
    public class UrenController : Controller
    {
        private readonly Supabase.Client _supabase;

        public UrenController(Supabase.Client supabase)
        {
            _supabase = supabase;
        }

        // --- DE NIEUWE VERWIJDER METHODE (Die de 404 oplost) ---
        [HttpPost]
        public async Task<IActionResult> VerwijderUren(long id, string email, string naam, string maand)
        {
            try
            {
                // Verwijder de specifieke regel uit Supabase op basis van ID
                await _supabase.From<UrenModel>().Where(x => x.Id == id).Delete();

                // Stuur de gebruiker terug naar het overzicht
                return RedirectToAction("UrenOverzicht", "Home", new { email = email, naam = naam, maand = maand });
            }
            catch (Exception ex)
            {
                // Bij fouten tonen we de melding
                return Content("Fout bij verwijderen: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkeerAlsBetaald(long id, string email, string naam, string maand)
        {
            // Veiligheidscheck voor ID
            if (id <= 0) return RedirectToAction("UrenOverzicht", "Home", new { email = email, naam = naam, maand = maand });

            await _supabase.From<UrenModel>().Where(x => x.Id == id).Set(x => x.IsUitbetaald, true).Update();
            return RedirectToAction("UrenOverzicht", "Home", new { email = email, naam = naam, maand = maand });
        }

        public async Task<IActionResult> ExportPdf(string email, string naam, string maand)
        {
            var culture = new CultureInfo("nl-NL");
            int jaar = 2026;
            DateTime geselecteerdeMaandDatum = DateTime.ParseExact(maand, "MMM", culture);

            DateTime eindPeriode = new DateTime(jaar, geselecteerdeMaandDatum.Month, 20);
            DateTime startPeriode = eindPeriode.AddMonths(-1).AddDays(1);

            var response = await _supabase.From<UrenModel>().Where(x => x.UserEmail == email).Get();
            var alleUren = response.Models ?? new List<UrenModel>();

            var urenVoorPdf = alleUren
                .Where(u => {
                    if (DateTime.TryParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    {
                        return d.Date >= startPeriode.Date && d.Date <= eindPeriode.Date;
                    }
                    return false;
                })
                .OrderBy(u => DateTime.ParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .ToList();

            double totaalOpenstaand = urenVoorPdf
                .Where(u => !u.IsUitbetaald && !(u.Locatie?.ToLower().Contains("vrij") ?? false))
                .Sum(u => u.Uren);

            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 30, 30, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var fontH1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                doc.Add(new Paragraph(new Phrase("GEWERKTE UREN 2026", fontH1)));
                doc.Add(new Paragraph(new Phrase($"Naam: {naam}", fontNormal)));
                doc.Add(new Paragraph(new Phrase("Bedrijf: V.I.P Security Service", fontNormal)));

                string periodeLabel = $"{startPeriode.Day} {culture.TextInfo.ToTitleCase(startPeriode.ToString("MMMM", culture))} - " +
                                     $"{eindPeriode.Day} {culture.TextInfo.ToTitleCase(eindPeriode.ToString("MMMM", culture))}";
                doc.Add(new Paragraph(new Phrase($"Periode: {periodeLabel}", fontNormal)));
                doc.Add(new Paragraph(" "));

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2f, 2.5f, 4f, 2f, 1.5f });

                string[] headers = { "Dag", "Datum", "Locatie", "Status", "Uren" };
                foreach (var h in headers)
                {
                    table.AddCell(new PdfPCell(new Phrase(h, fontBold)) { Border = Rectangle.BOTTOM_BORDER, PaddingBottom = 5 });
                }

                foreach (var u in urenVoorPdf)
                {
                    DateTime d = DateTime.ParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    table.AddCell(new PdfPCell(new Phrase(d.ToString("dddd", culture), fontNormal)) { Border = 0, PaddingTop = 5 });
                    table.AddCell(new PdfPCell(new Phrase(u.DatumString, fontNormal)) { Border = 0, PaddingTop = 5 });
                    table.AddCell(new PdfPCell(new Phrase(u.Locatie ?? "", fontNormal)) { Border = 0, PaddingTop = 5 });
                    table.AddCell(new PdfPCell(new Phrase(u.IsUitbetaald ? "Betaald" : "Open", fontNormal)) { Border = 0, PaddingTop = 5 });

                    string display = u.IsUitbetaald ? "✓" : (u.Locatie?.ToLower().Contains("vrij") == true ? "*" : u.Uren.ToString("N1", culture));
                    table.AddCell(new PdfPCell(new Phrase(display, fontNormal)) { Border = 0, PaddingTop = 5 });
                }

                doc.Add(table);
                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(new Phrase("TOTAAL PERIODE OPENSTAAND", fontBold)));
                doc.Add(new Paragraph(new Phrase($"{totaalOpenstaand.ToString("N1", culture)} UUR", fontH1)));

                doc.Close();
                return this.File(ms.ToArray(), "application/pdf", $"Uren_{naam}_{maand}.pdf");
            }
        }
    }
}