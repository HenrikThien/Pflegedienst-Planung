using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.draw;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PflegedientPlan.Utils
{
    class CustomPageEventHandler : PdfPageEventHelper
    {
        public override void OnStartPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = PageSize.A3.Rotate().Width - 20; // -20 margin space
            table.HorizontalAlignment = 0;
            table.LockedWidth = true;
            float[] widths = new float[] { 20f, 80f, 80f, 80f, 80f, 30f, 20f, 10f, 20, 10f };
            table.SetWidths(widths);

            // first row
            AddCellToTable(table, "Nr.", true, 1);
            AddCellToTable(table, "Problem", true, 1);
            AddCellToTable(table, "Ressource", true, 1);
            AddCellToTable(table, "Ziel", true, 1);
            AddCellToTable(table, "Maßnahmen", true, 1);
            AddCellToTable(table, "Häufigkeit", true, 1);
            AddCellToTable(table, "Datum an", true, 1);
            AddCellToTable(table, "HZ", true, 1);
            AddCellToTable(table, "Datum ab", true, 1);
            AddCellToTable(table, "HZ", true, 1);

            Chunk glue = new Chunk(new VerticalPositionMark());

            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\VIS VITALIS\\images\\logo.PNG");
            image.ScalePercent(60);

            var p = new iTextSharp.text.Paragraph();
            p.Add(image);
            p.Add(new Chunk(glue));
            p.Add("Pflegeplanung");
            p.Add(new Chunk(glue));
            p.Add("Patient: " + MainWindow.SelectedPatient.PatientVorname + " " + MainWindow.SelectedPatient.PatientNachname);

            document.Add(p);

            p = new iTextSharp.text.Paragraph();
            p.Add(new Chunk(glue));
            p.Add("Pat.-Nr: " + MainWindow.SelectedPatient.PatientId);

            document.Add(p);

            p = new iTextSharp.text.Paragraph();
            p.Add(new Chunk(glue));
            p.Add("Blatt: " + writer.CurrentPageNumber);
            p.SpacingAfter = 20;

            document.Add(p);
            table.SpacingAfter = 20;
            document.Add(table);

            base.OnStartPage(writer, document);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            PdfPTable table = new PdfPTable(10);
            table.TotalWidth = PageSize.A3.Rotate().Width - 20; // -20 margin space
            table.HorizontalAlignment = 0;
            table.LockedWidth = true;
            float[] widths = new float[] { 20f, 80f, 80f, 80f, 80f, 30f, 20f, 10f, 20, 10f };
            table.SetWidths(widths);

            AddFooterToTable(table, "");

            document.Add(table);

            base.OnEndPage(writer, document);
        }

        private void AddCellToTable(PdfPTable table, string text, bool header = false, int span = 1, int border = iTextSharp.text.Rectangle.TOP_BORDER | iTextSharp.text.Rectangle.BOTTOM_BORDER | iTextSharp.text.Rectangle.LEFT_BORDER | iTextSharp.text.Rectangle.RIGHT_BORDER)
        {
            BaseFont bfTimes = BaseFont.CreateFont(BaseFont.TIMES_ROMAN, BaseFont.CP1252, false);

            iTextSharp.text.Font times = new iTextSharp.text.Font(bfTimes, 11, iTextSharp.text.Font.NORMAL, iTextSharp.text.BaseColor.BLACK);

            if (header)
                times.SetStyle(Font.BOLD);

            PdfPCell cell = new PdfPCell(new Phrase(text, times));
            cell.Border = border;
            cell.Rowspan = span;
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.Padding = 5;
            table.AddCell(cell);
        }

        private void AddFooterToTable(PdfPTable table, string text)
        {
            PdfPCell cell = new PdfPCell(new Phrase(text));
            cell.HorizontalAlignment = PdfPCell.ALIGN_CENTER;
            cell.VerticalAlignment = PdfPCell.ALIGN_MIDDLE;
            cell.Colspan = 10;
            cell.Padding = 5;
            table.AddCell(cell);
        }
    }
}
