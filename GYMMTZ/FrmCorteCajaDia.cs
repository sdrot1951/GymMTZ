using GymApp.BLL;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextFont = iTextSharp.text.Font;
using iTextImage = iTextSharp.text.Image;

namespace GYMMTZ
{
    public partial class FrmCorteCajaDia : Form
    {
        // Variables globales del día
        private decimal sysEfectivo = 0;
        private decimal sysTransferencias = 0;
        private decimal sysGastos = 0;
        private decimal sysDevoluciones = 0; // NUEVA VARIABLE PARA DEVOLUCIONES

        private Label lblTotalEfectivo, lblTotalTransferencia, lblTotalGastos, lblTotalDevoluciones;
        private TextBox txtObservaciones;
        private DateTimePicker dtpFechaCorte;

        public FrmCorteCajaDia()
        {
            InitializeComponent();
            ConfigurarUI();
            CargarResumenDiario();
        }

        private void FrmCorteCajaDia_Load(object sender, EventArgs e)
        {
        }

        private void ConfigurarUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;

            // Aumentamos ligeramente la altura para que quepa la nueva sección
            this.Size = new Size(420, 560);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Cierre de Caja General";

            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            Label title = new Label { Text = "🔒 Cierre General del Día", Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.Gold };

            Label lblFechaSelector = new Label { Text = "Fecha a procesar:", Location = new Point(20, 65), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12), ForeColor = Color.DarkGray };
            dtpFechaCorte = new DateTimePicker
            {
                Location = new Point(160, 63),
                Width = 140,
                Format = DateTimePickerFormat.Short,
                Font = new System.Drawing.Font("Segoe UI", 12)
            };
            dtpFechaCorte.ValueChanged += (s, e) => CargarResumenDiario();

            // --- BLOQUE DE EFECTIVO ---
            Label lblEf = new Label { Text = "Efectivo Neto (Ingresos - Devoluciones):", Location = new Point(20, 110), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11) };
            lblTotalEfectivo = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.LimeGreen, Location = new Point(20, 130), AutoSize = true };

            // --- BLOQUE DE TRANSFERENCIAS ---
            Label lblTr = new Label { Text = "Transferencias Netas:", Location = new Point(20, 180), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11) };
            lblTotalTransferencia = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.DeepSkyBlue, Location = new Point(20, 200), AutoSize = true };

            // --- BLOQUE DE DEVOLUCIONES (NUEVO) ---
            Label lblDev = new Label { Text = "Devoluciones (Dinero devuelto):", Location = new Point(20, 250), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11) };
            lblTotalDevoluciones = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Orange, Location = new Point(20, 270), AutoSize = true };

            // --- BLOQUE DE GASTOS ---
            Label lblGa = new Label { Text = "Gastos Totales (Salidas de caja):", Location = new Point(20, 320), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11) };
            lblTotalGastos = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Crimson, Location = new Point(20, 340), AutoSize = true };

            // Observaciones
            Label lblObs = new Label { Text = "Observaciones de Auditoría (Opcional):", Location = new Point(20, 395), AutoSize = true };
            txtObservaciones = new TextBox { Location = new Point(20, 415), Width = 380, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Botones
            Button btnCorte = new Button
            {
                Text = "✅ CERRAR DÍA",
                Location = new Point(20, 495),
                Width = 180,
                Height = 45,
                BackColor = Color.LimeGreen,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCorte.FlatAppearance.BorderSize = 0;
            btnCorte.Click += BtnCorte_Click;

            Button btnCancelar = new Button
            {
                Text = "❌ CANCELAR",
                Location = new Point(220, 495),
                Width = 180,
                Height = 45,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            Button btnCerrar = new Button { Text = "✖", Location = new Point(380, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                title,
                lblFechaSelector, dtpFechaCorte,
                lblEf, lblTotalEfectivo,
                lblTr, lblTotalTransferencia,
                lblDev, lblTotalDevoluciones, // Agregado a la UI
                lblGa, lblTotalGastos,
                lblObs, txtObservaciones,
                btnCorte, btnCancelar, btnCerrar
            });
        }

        private void CargarResumenDiario()
        {
            try
            {
                var bll = new CajaBLL();
                DateTime fechaInicio = dtpFechaCorte.Value.Date;
                DateTime fechaFin = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(fechaInicio, fechaFin, 0);

                sysEfectivo = 0;
                sysTransferencias = 0;
                sysGastos = 0;
                sysDevoluciones = 0;

                if (dtMovimientos != null && dtMovimientos.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMovimientos.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        string tipoMov = row["Tipo"].ToString().ToUpper();
                        string tipoPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString().ToUpper() : "EFECTIVO";

                        if (tipoMov == "GASTO")
                        {
                            sysGastos += monto;
                        }
                        else if (tipoMov == "DEVOLUCIÓN")
                        {
                            sysDevoluciones += monto; // Guardamos para mostrarlo

                            // Restamos la devolución de los ingresos totales
                            if (tipoPago == "TRANSFERENCIA")
                                sysTransferencias -= monto;
                            else
                                sysEfectivo -= monto;
                        }
                        else // VENTA, ABONO, INGRESO NORMAL
                        {
                            if (tipoPago == "TRANSFERENCIA")
                                sysTransferencias += monto;
                            else
                                sysEfectivo += monto;
                        }
                    }
                }

                // Efectivo neto real en caja después de restar los gastos (Las devoluciones ya se restaron arriba)
                decimal efectivoFisicoEnCaja = sysEfectivo - sysGastos;

                // Pintamos en pantalla
                lblTotalEfectivo.Text = efectivoFisicoEnCaja.ToString("$#,##0.00");
                lblTotalTransferencia.Text = sysTransferencias.ToString("$#,##0.00");
                lblTotalDevoluciones.Text = sysDevoluciones.ToString("$#,##0.00");
                lblTotalGastos.Text = sysGastos.ToString("$#,##0.00");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos del día: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCorte_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Estás seguro de cerrar la caja general de HOY? Ya no se podrán modificar los movimientos.", "Confirmar Cierre General", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    decimal fondoInicial = 0;
                    decimal declaradoEfectivo = sysEfectivo - sysGastos;
                    decimal declaradoTransferencia = sysTransferencias;
                    decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;

                    int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
                    string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto ?? "Usuario Admin";

                    var bll = new CajaBLL();
                    DateTime fechaInicioPDF = dtpFechaCorte.Value.Date;
                    DateTime fechaFinPDF = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                    DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(fechaInicioPDF, fechaFinPDF, 0);

                    int idCorteGenerado = bll.GenerarCorte(idEmpleadoLogueado, fondoInicial, declaradoTotal, declaradoTotal, txtObservaciones.Text, dtpFechaCorte.Value.Date);

                    if (idCorteGenerado > 0)
                    {
                        MessageBox.Show("Cierre Diario de caja registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (MessageBox.Show("¿Desea generar el reporte en PDF del día?", "Generar Reporte", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            GenerarReportePDF(idCorteGenerado, fondoInicial, declaradoEfectivo, declaradoTransferencia, nombreEmpleado, txtObservaciones.Text, dtMovimientos);
                        }

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error al generar cierre", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void GenerarReportePDF(int idCorte, decimal fondoInicial, decimal declaradoEfectivo, decimal declaradoTransferencia, string empleado, string observaciones, DataTable dtMovimientos)
        {
            try
            {
                string nombreArchivo = $"Cierre_Diario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(nombreArchivo, FileMode.Create));

                // --- SOLUCIÓN DEL LOGO ---
                // Intentamos buscar logo.jpg, si no existe buscamos logo.png
                string rutaLogo = Path.Combine(Application.StartupPath, "logo.jpg");
                if (!System.IO.File.Exists(rutaLogo))
                {
                    rutaLogo = Path.Combine(Application.StartupPath, "logo.png");
                }

                writer.PageEvent = new MarcaDeAgua(rutaLogo);

                doc.Open();

                iTextFont fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextFont fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                iTextFont fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                iTextFont fontCuerpo = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                Paragraph titulo = new Paragraph("CIERRE GENERAL DIARIO - GYM MTZ DEWS\n", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                Paragraph subtitulo = new Paragraph($"Fecha del Corte: {dtpFechaCorte.Value:dd/MM/yyyy}\n\n", fontSubtitulo);
                subtitulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(subtitulo);

                // --- TABLA DE RESUMEN ---
                PdfPTable tableResumen = new PdfPTable(2);
                tableResumen.WidthPercentage = 65;
                tableResumen.HorizontalAlignment = Element.ALIGN_LEFT;
                tableResumen.SetWidths(new float[] { 55f, 45f });

                decimal ingresosTotales = sysEfectivo + sysTransferencias;
                decimal totalEnEfectivo = sysEfectivo - sysGastos;

                tableResumen.AddCell(new PdfPCell(new Phrase("TOTAL INGRESOS NETOS:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(ingresosTotales.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("En Transferencias:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(sysTransferencias.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                // Agregamos la línea informativa de las devoluciones
                tableResumen.AddCell(new PdfPCell(new Phrase("Devoluciones (Ya restadas):", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(sysDevoluciones.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Total de salidas (Gastos):", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(sysGastos.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Total físico en efectivo:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(totalEnEfectivo.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Diferencia Auditada:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase("$0.00", fontHeader)) { Border = PdfPCell.NO_BORDER });

                doc.Add(tableResumen);

                Paragraph datosPie = new Paragraph($"\nCierre elaborado por: {empleado}\nObservaciones: {(string.IsNullOrWhiteSpace(observaciones) ? "Ninguna" : observaciones)}\n", fontCuerpo);
                doc.Add(datosPie);

                // --- TABLA DE DETALLES DE MOVIMIENTOS ---
                doc.Add(new Paragraph("\nDetalle General de Movimientos:\n\n", fontHeader));

                PdfPTable tabla = new PdfPTable(5);
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 12f, 18f, 35f, 20f, 15f });

                tabla.AddCell(new PdfPCell(new Phrase("Folio", fontHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tabla.AddCell(new PdfPCell(new Phrase("Tipo", fontHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tabla.AddCell(new PdfPCell(new Phrase("Concepto", fontHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tabla.AddCell(new PdfPCell(new Phrase("Método Pago", fontHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });
                tabla.AddCell(new PdfPCell(new Phrase("Monto", fontHeader)) { BackgroundColor = BaseColor.LIGHT_GRAY });

                if (dtMovimientos != null && dtMovimientos.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMovimientos.Rows)
                    {
                        tabla.AddCell(new Phrase(row["Folio"].ToString(), fontCuerpo));

                        // Si es devolución o gasto pintamos en rojo, si no en negro normal
                        string tipo = row["Tipo"].ToString();
                        iTextFont rowFont = (tipo == "GASTO" || tipo == "DEVOLUCIÓN") ? FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.RED) : fontCuerpo;

                        tabla.AddCell(new Phrase(tipo, rowFont));
                        tabla.AddCell(new Phrase(row["Concepto"].ToString(), rowFont));

                        string tPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString() : "EFECTIVO";
                        tabla.AddCell(new Phrase(tPago, rowFont));

                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        tabla.AddCell(new Phrase(monto.ToString("$#,##0.00"), rowFont));
                    }
                }
                else
                {
                    PdfPCell celdaVacia = new PdfPCell(new Phrase("No hay movimientos registrados.", fontCuerpo));
                    celdaVacia.Colspan = 5;
                    celdaVacia.HorizontalAlignment = Element.ALIGN_CENTER;
                    tabla.AddCell(celdaVacia);
                }

                doc.Add(tabla);
                doc.Close();
                writer.Close();

                System.Diagnostics.Process.Start(nombreArchivo);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar PDF: " + ex.Message, "Error PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    public class MarcaDeAgua : PdfPageEventHelper
    {
        private string rutaImagen;

        public MarcaDeAgua(string ruta)
        {
            rutaImagen = ruta;
        }

        public override void OnEndPage(PdfWriter writer, iTextSharp.text.Document document)
        {
            if (System.IO.File.Exists(rutaImagen))
            {
                PdfContentByte cb = writer.DirectContentUnder;
                iTextImage img = iTextImage.GetInstance(rutaImagen);

                PdfGState state = new PdfGState();
                state.FillOpacity = 0.15f; // Opacidad
                cb.SetGState(state);

                img.ScaleToFit(400f, 400f);
                img.SetAbsolutePosition(
                    (document.PageSize.Width - img.ScaledWidth) / 2,
                    (document.PageSize.Height - img.ScaledHeight) / 2
                );

                cb.AddImage(img);
            }
        }
    }
}