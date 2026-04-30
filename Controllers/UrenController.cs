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

        public async Task<IActionResult> ExportPdf(string email, string naam, string maand)
        {
            var culture = new CultureInfo("nl-NL");

            // 1. Bereken de loonperiode (21e vorige maand t/m 20e huidige maand)
            // Voorbeeld: Als 'maand' Apr is, dan wordt het 21 Maart t/m 20 April
            int jaar = 2026;
            DateTime geselecteerdeMaandDatum = DateTime.ParseExact(maand, "MMM", culture);

            DateTime eindPeriode = new DateTime(jaar, geselecteerdeMaandDatum.Month, 20); // 20ste van geselecteerde maand
            DateTime startPeriode = eindPeriode.AddMonths(-1).AddDays(1); // 21ste van de vorige maand

            // 2. Haal ALLE uren op voor deze medewerker
            var response = await _supabase.From<UrenModel>().Where(x => x.UserEmail == email).Get();
            var alleUren = response.Models ?? new List<UrenModel>();

            // 3. Filter op de fysieke datum (ongeacht de 'periode_naam' kolom)
            var urenVoorPdf = alleUren
                .Where(u => {
                    if (DateTime.TryParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime d))
                    {
                        // Check of de datum echt tussen de 21e en 20e valt
                        return d.Date >= startPeriode.Date && d.Date <= eindPeriode.Date;
                    }
                    return false;
                })
                .OrderBy(u => DateTime.ParseExact(u.DatumString, "dd-MM-yyyy", CultureInfo.InvariantCulture))
                .ToList();

            // 4. Totaal berekenen van openstaande uren in deze periode
            double totaalOpenstaand = urenVoorPdf
                .Where(u => !u.IsUitbetaald && !(u.Locatie?.ToLower().Contains("vrij") ?? false))
                .Sum(u => u.Uren);

            // 5. PDF Genereren
            using (MemoryStream ms = new MemoryStream())
            {
                Document doc = new Document(PageSize.A4, 30, 30, 40, 40);
                PdfWriter.GetInstance(doc, ms);
                doc.Open();

                var fontH1 = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var fontBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Header
                doc.Add(new Paragraph(new Phrase("GEWERKTE UREN 2026", fontH1)));
                doc.Add(new Paragraph(new Phrase($"Naam: {naam}", fontNormal)));
                doc.Add(new Paragraph(new Phrase("Bedrijf: V.I.P Security Service", fontNormal)));

                // Toon de loonperiode in de kop
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

                    // Gebruik de ✓ en * tekens
                    string display = u.IsUitbetaald ? "✓" : (u.Locatie?.ToLower().Contains("vrij") == true ? "*" : u.Uren.ToString("N1", culture));
                    table.AddCell(new PdfPCell(new Phrase(display, fontNormal)) { Border = 0, PaddingTop = 5 });
                }

                if (!urenVoorPdf.Any())
                {
                    table.AddCell(new PdfPCell(new Phrase("Geen uren gevonden voor de periode 21-20.", fontNormal)) { Colspan = 5, Border = 0, PaddingTop = 10 });
                }

                doc.Add(table);

                doc.Add(new Paragraph(" "));
                doc.Add(new Paragraph(new Phrase("TOTAAL PERIODE OPENSTAAND", fontBold)));
                doc.Add(new Paragraph(new Phrase($"{totaalOpenstaand.ToString("N1", culture)} UUR", fontH1)));

                doc.Close();
                return this.File(ms.ToArray(), "application/pdf", $"Uren_{naam}_{maand}.pdf");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MarkeerAlsBetaald(long id, string email, string naam, string maand)
        {
            await _supabase.From<UrenModel>().Where(x => x.Id == id).Set(x => x.IsUitbetaald, true).Update();
            return RedirectToAction("UrenOverzicht", "Home", new { email = email, naam = naam, maand = maand });
        }
    }
}