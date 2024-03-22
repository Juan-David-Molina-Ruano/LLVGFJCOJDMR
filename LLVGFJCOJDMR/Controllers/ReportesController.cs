using LLVGFJCOJDMR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace LLVGFJCOJDMR.Controllers
{
    public class ReportesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> GenerarReporte(string firstName, string lastName, string opcion)
        {
            var customers = await _context.Customers.Include(c => c.PhoneNumbers).Where(c => c.FirstName == firstName).ToListAsync();
            if (customers.Count == 0)
            {
                ViewBag.ErrorMessage = "No se encontraron clientes.";
                return View("Index");
            }
            else 
            {
                if (opcion == "PDF")
                {
                    return new ViewAsPdf("GenerarReporte", customers);
                }
                else if (opcion == "EXCEL")
                {
                    using (var package = new ExcelPackage())
                    {
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Reportes");
                        worksheet.Column(1).Width = 20;
                        worksheet.Column(2).Width = 20;
                        worksheet.Column(3).Width = 30;
                        worksheet.Column(4).Width = 10;
                        worksheet.Column(5).Width = 25;

                        // Establecer colores de fondo para los encabezados
                        worksheet.Cells["A1:C1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells["A1:C1"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);


                        worksheet.Cells["A1"].Value = "Nombre";
                        worksheet.Cells["B1"].Value = "Apellido";
                        worksheet.Cells["C1"].Value = "Email";
                        worksheet.Cells["D1"].Value = "Número de Teléfono";
                        worksheet.Cells["E1"].Value = "Nota";

                        int fila = 2;
                        foreach (var item in customers)
                        {
                            int columna = 1;
                            worksheet.Cells[fila, columna].Value = item.FirstName;

                            columna = 2;
                            worksheet.Cells[fila, columna].Value = item.LastName;

                            columna = 3;
                            worksheet.Cells[fila, columna].Value = item.Email;

                            // Agregar números de teléfono y notas debajo del cliente actual
                            foreach (var item1 in item.PhoneNumbers)
                            {
                                fila++;

                                columna = 4;
                                worksheet.Cells[fila, columna].Value = item1.NumberPhone;

                                columna = 5;
                                worksheet.Cells[fila, columna].Value = item1.Note;
                            }

                            fila++; // Saltar a la siguiente fila para el próximo cliente
                        }

                        int lastRow = worksheet.Dimension.End.Row;

                        // Obtener el rango de celdas para aplicar bordes a todos los datos
                        ExcelRange range1 = worksheet.Cells[1, 1, lastRow, 5];
                        range1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        range1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        range1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        range1.Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        // Obtener el rango de celdas que deseas aplicar el filtro (solo para los encabezados de clientes)
                        ExcelRange range2 = worksheet.Cells["A1:C1"];
                        range2.AutoFilter = true;

                        byte[]? filecontents = package.GetAsByteArray();

                        return File(filecontents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte de Clientes.xlsx");
                    }



                }
                else
                {
                    return Content("Opcion no valida");
                }

            }
        }

    }
}
