using LLVGFJCOJDMR.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;

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

        public async Task<IActionResult> GenerarReporte(string firstName, string lastName, string opcion)
        {
            var customers = await _context.Customers.Include(c => c.PhoneNumbers).Where(c => c.FirstName == firstName).ToListAsync();
            if (customers.Count == 0)
            {
                ViewBag.ErrorMessage = "No se encontraron ventas para el cliente proporcionado.";
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
                        worksheet.Column(2).Width = 20; // Establecer el ancho de la columna B en 30
                        worksheet.Column(3).Width = 30; // Establecer el ancho de la columna C en 30
                        worksheet.Column(4).Width = 10; // Establecer el ancho de la columna D en 30
                        worksheet.Column(5).Width = 25; // Establecer el ancho de la columna E en 30
                        worksheet.Column(6).Width = 15; // Establecer el ancho de la columna F en 30
                        worksheet.Column(7).Width = 19; // Establecer el ancho de la columna G en 30
                        worksheet.Column(8).Width = 8; // Establecer el ancho de la columna H en 30
                        worksheet.Column(9).Width = 14; // Establecer el ancho de la columna I en 30

                        worksheet.Cells["A1"].Value = "Cliente";
                        worksheet.Cells["B1"].Value = "Empleado";
                        worksheet.Cells["C1"].Value = "Detalles";
                        worksheet.Cells["D1"].Value = "Color";
                        worksheet.Cells["E1"].Value = "Detalles de estilo";
                        worksheet.Cells["F1"].Value = "Marca";
                        worksheet.Cells["G1"].Value = "Zapato";
                        worksheet.Cells["H1"].Value = "Talla";
                        worksheet.Cells["I1"].Value = "Fecha de venta";

                        int fila = 2;
                        foreach (var item in ventas)
                        {
                            int columna = 1;
                            worksheet.Cells[fila, columna].Value = item.Cliente.Nombre;

                            columna = 2;
                            worksheet.Cells[fila, columna].Value = item.Empleado.Nombre;

                            foreach (var item1 in item.DetalleVentas)
                            {
                                columna = 3;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.Descripcion;

                                columna = 4;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.DetalleEstilo.Color;

                                columna = 5;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.DetalleEstilo.Nombre;

                                columna = 6;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.DetalleEstilo.Marca.Nombre;

                                columna = 7;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.Descripcion;

                                columna = 8;
                                worksheet.Cells[fila, columna].Value = item1.Zapato.Talla.Nombre;
                            }

                            columna = 9;
                            worksheet.Cells[fila, columna].Style.Numberformat.Format = "yyyy-MM-dd";
                            worksheet.Cells[fila, columna].Value = item.FechaVenta;
                            fila++;

                            int lastRow = worksheet.Dimension.End.Row;

                            // Obtener el rango de celdas que deseas aplicar el borde
                            ExcelRange range1 = worksheet.Cells[1, 1, lastRow, 9];

                            // Aplicar bordes
                            range1.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            range1.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            range1.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            range1.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        }
                        var range = worksheet.Cells["A1:I" + fila];

                        // Agregar un filtro a ese rango
                        range.AutoFilter = true;

                        byte[]? filecontents = package.GetAsByteArray();

                        return File(filecontents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte de Venta.xlsx");

                    }
                }
                  
            }
           
           

        }

    }
}
