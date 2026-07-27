using GymApp.BLL;
using GymApp.Entities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
// ALIAS PARA EVITAR CONFLICTOS CON WINDOWS FORMS
using iTextFont = iTextSharp.text.Font;
using iTextImage = iTextSharp.text.Image;

namespace GYMMTZ
{
    public partial class FrmCorteCaja : Form
    {
        private Label lblTotalEfectivo, lblTotalTransferencia, lblTotalGastos, lblTotalDevoluciones;
        private TextBox txtObservaciones;
        private DateTimePicker dtpFechaCorte;

        // Variables para guardar los cálculos del sistema
        private decimal sysEfectivo = 0;
        private decimal sysTransferencias = 0;
        private decimal sysGastos = 0;

        // Variables para devoluciones
        private decimal sysDevoluciones = 0;
        private int sysConteoDevoluciones = 0;

        public FrmCorteCaja()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(18, 18, 20);
            this.ForeColor = Color.White;
            // Hacemos el form un poco más alto para que quepan las devoluciones
            this.Size = new Size(400, 560);
            this.StartPosition = FormStartPosition.CenterParent;

            // Borde sutil
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            Label lblTitulo = new Label { Text = "Resumen de Turno", Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.FromArgb(255, 69, 0) };

            Label lblFechaSelector = new Label { Text = "Fecha a procesar:", Location = new Point(20, 60), AutoSize = true, ForeColor = Color.DarkGray };
            dtpFechaCorte = new DateTimePicker
            {
                Location = new Point(140, 56),
                Width = 120,
                Format = DateTimePickerFormat.Short, // Formato dd/MM/yyyy
                BackColor = Color.FromArgb(35, 35, 40),
                ForeColor = Color.White
            };
            dtpFechaCorte.ValueChanged += (s, e) => CargarResumenTurno(GymApp.Core.SesionGlobal.IdEmpleado);
            this.Controls.Add(lblFechaSelector);
            this.Controls.Add(dtpFechaCorte);

            // --- BLOQUE DE EFECTIVO ---
            Label lblEf = new Label { Text = "Total en Efectivo:", Location = new Point(20, 80), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
            lblTotalEfectivo = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.LimeGreen, Location = new Point(20, 105), AutoSize = true };

            // --- BLOQUE DE TRANSFERENCIAS ---
            Label lblTr = new Label { Text = "Total Transferencias:", Location = new Point(20, 150), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
            lblTotalTransferencia = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.DeepSkyBlue, Location = new Point(20, 175), AutoSize = true };

            // --- BLOQUE DE GASTOS ---
            Label lblGa = new Label { Text = "Total Gastos (Salidas):", Location = new Point(20, 220), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
            lblTotalGastos = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Crimson, Location = new Point(20, 245), AutoSize = true };

            // --- BLOQUE DE DEVOLUCIONES ---
            Label lblDev = new Label { Text = "Devoluciones (Cant. y Monto):", Location = new Point(20, 290), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
            lblTotalDevoluciones = new Label { Text = "0 ($0.00)", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Orange, Location = new Point(20, 315), AutoSize = true };

            // Observaciones
            Label lblObs = new Label { Text = "Observaciones del turno (Opcional):", Location = new Point(20, 370), AutoSize = true };
            txtObservaciones = new TextBox { Location = new Point(20, 390), Width = 360, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Botón Guardar
            Button btnGuardar = new Button
            {
                Text = "✅ Generar Corte",
                Location = new Point(20, 480),
                Width = 180,
                Height = 45,
                BackColor = Color.FromArgb(255, 69, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.Click += btnRealizarCorte_Click;

            // Botón Cancelar
            Button btnCancelar = new Button
            {
                Text = "❌ Cancelar",
                Location = new Point(210, 480),
                Width = 170,
                Height = 45,
                BackColor = Color.FromArgb(60, 60, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += (s, e) => this.Close();

            // Botón Cerrar (X arriba)
            Button btnCerrar = new Button { Text = "✖", Location = new Point(360, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] {
                lblTitulo,
                lblEf, lblTotalEfectivo,
                lblTr, lblTotalTransferencia,
                lblGa, lblTotalGastos,
                lblDev, lblTotalDevoluciones, // Agregado el visual de devoluciones
                lblObs, txtObservaciones,
                btnGuardar, btnCancelar, btnCerrar
            });

            // Disparamos el cálculo automático usando el ID de la sesión actual
            CargarResumenTurno(GymApp.Core.SesionGlobal.IdEmpleado);
        }

        private void CargarResumenTurno(int idEmpleado)
        {
            try
            {
                var bll = new GymApp.BLL.CajaBLL();

                DateTime fechaInicio = dtpFechaCorte.Value.Date;
                DateTime fechaFin = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                // Traemos los movimientos de ESA fecha
                DataTable dt = bll.ObtenerDetalleMovimientos(fechaInicio, fechaFin, idEmpleado);

                sysEfectivo = 0;
                sysTransferencias = 0;
                sysGastos = 0;
                sysDevoluciones = 0;
                sysConteoDevoluciones = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Math.Abs(Convert.ToDecimal(row["Monto"]));
                        string tipoMov = row["Tipo"].ToString().ToUpper().Trim();
                        string concepto = dt.Columns.Contains("Concepto") ? row["Concepto"].ToString().ToUpper().Trim() : "";
                        string tipoPago = dt.Columns.Contains("TipoPago") ? row["TipoPago"].ToString().ToUpper().Trim() : "EFECTIVO";

                        if (tipoMov.Contains("DEVOL") || concepto.Contains("DEVOL"))
                        {
                            sysDevoluciones += monto;
                            sysConteoDevoluciones++;
                        }
                        else if (tipoMov.Contains("GASTO") || tipoMov.Contains("SALIDA"))
                        {
                            sysGastos += monto;
                        }
                        else // Venta, Ingreso, etc.
                        {
                            if (tipoPago.Contains("TRANSFERENCIA"))
                                sysTransferencias += monto;
                            else
                                sysEfectivo += monto;
                        }
                    }
                }

                // Descontamos los gastos y devoluciones del total en efectivo físico que debe haber en caja
                decimal efectivoNeto = sysEfectivo - sysGastos - sysDevoluciones;

                // Pintamos la interfaz
                lblTotalEfectivo.Text = efectivoNeto.ToString("$#,##0.00");
                lblTotalTransferencia.Text = sysTransferencias.ToString("$#,##0.00");
                lblTotalGastos.Text = sysGastos.ToString("$#,##0.00");
                lblTotalDevoluciones.Text = $"{sysConteoDevoluciones} ({sysDevoluciones.ToString("$#,##0.00")})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el resumen del turno: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FrmCorteCaja_Load(object sender, EventArgs e)
        {
        }

        private void btnRealizarCorte_Click(object sender, EventArgs e)
        {
            try
            {
                decimal fondoInicial = 0;
                // Aplicamos la resta también al generar el corte
                decimal declaradoEfectivo = sysEfectivo - sysGastos - sysDevoluciones;
                decimal declaradoTransferencia = sysTransferencias;

                decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;

                decimal montoEsperado = declaradoTotal;

                string observaciones = txtObservaciones.Text;
                int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
                string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto;

                var bll = new CajaBLL();

                // Guardamos el corte en SQL 
                int nuevoIdCorte = bll.ProcesarCorteCaja(idEmpleadoLogueado, fondoInicial, montoEsperado, declaradoTotal, observaciones, dtpFechaCorte.Value.Date);

                DateTime fechaInicioPDF = dtpFechaCorte.Value.Date;
                DateTime fechaFinPDF = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(fechaInicioPDF, fechaFinPDF, idEmpleadoLogueado);

                MessageBox.Show("Corte de caja generado y guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Imprimimos el PDF con los valores ya restados
                GenerarReportePDF(nuevoIdCorte, fondoInicial, declaradoEfectivo, declaradoTransferencia, nombreEmpleado, observaciones, dtMovimientos);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el corte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void GenerarReportePDF(int idCorte, decimal fondoInicial, decimal declaradoEfectivo, decimal declaradoTransferencia, string empleado, string observaciones, DataTable dtMovimientos)
        {
            try
            {
                string nombreArchivo = $"Corte_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(nombreArchivo, FileMode.Create));

                string rutaLogo = Path.Combine(Application.StartupPath, "logo.jpg");
                writer.PageEvent = new MarcaDeAgua(rutaLogo);

                doc.Open();

                iTextFont fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextFont fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                iTextFont fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                iTextFont fontCuerpo = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                Paragraph titulo = new Paragraph("CORTE DE CAJA TURNO - GYM MTZ DEWS\n", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                Paragraph subtitulo = new Paragraph($"Fecha del Corte: {dtpFechaCorte.Value:dd/MM/yyyy}\n\n", fontSubtitulo);
                subtitulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(subtitulo);

                // ==========================================
                // LÓGICA MATEMÁTICA PARA EL RESUMEN DEL PDF
                // ==========================================
                decimal ingresos = 0;
                decimal salidas = 0;
                decimal devoluciones = 0;
                int contadorDevoluciones = 0;

                Dictionary<string, decimal> ingresosPorPago = new Dictionary<string, decimal>();

                if (dtMovimientos != null && dtMovimientos.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMovimientos.Rows)
                    {
                        decimal monto = Math.Abs(Convert.ToDecimal(row["Monto"]));
                        string tipoMov = row["Tipo"].ToString().ToUpper().Trim();
                        string concepto = dtMovimientos.Columns.Contains("Concepto") ? row["Concepto"].ToString().ToUpper().Trim() : "";
                        string tipoPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString().ToUpper().Trim() : "EFECTIVO";

                        if (tipoMov.Contains("DEVOL") || concepto.Contains("DEVOL"))
                        {
                            devoluciones += monto;
                            contadorDevoluciones++;
                        }
                        else if (tipoMov.Contains("GASTO") || tipoMov.Contains("SALIDA"))
                        {
                            salidas += monto;
                        }
                        else
                        {
                            ingresos += monto;

                            if (ingresosPorPago.ContainsKey(tipoPago))
                                ingresosPorPago[tipoPago] += monto;
                            else
                                ingresosPorPago.Add(tipoPago, monto);
                        }
                    }
                }

                decimal mtoTransferencia = ingresosPorPago.ContainsKey("TRANSFERENCIA") ? ingresosPorPago["TRANSFERENCIA"] : 0;
                decimal mtoEfectivo = ingresosPorPago.ContainsKey("EFECTIVO") ? ingresosPorPago["EFECTIVO"] : 0;

                // Restamos gastos y devoluciones del efectivo bruto
                decimal totalEnEfectivo = mtoEfectivo - salidas - devoluciones;

                decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;
                decimal esperado = fondoInicial + ingresos - salidas - devoluciones;
                decimal diferencia = declaradoTotal - esperado;

                // ==========================================
                // TABLA DE RESUMEN
                // ==========================================
                PdfPTable tableResumen = new PdfPTable(2);
                tableResumen.WidthPercentage = 65;
                tableResumen.HorizontalAlignment = Element.ALIGN_LEFT;
                tableResumen.SetWidths(new float[] { 55f, 45f });

                tableResumen.AddCell(new PdfPCell(new Phrase("TOTAL DE INGRESOS BRUTOS:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(ingresos.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Transferencia:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(mtoTransferencia.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Total de salidas (Gastos) (-):", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(salidas.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Total Devoluciones (-):", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase($"{contadorDevoluciones} reg. ({devoluciones.ToString("$#,##0.00")})", fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                tableResumen.AddCell(new PdfPCell(new Phrase("Total físico en efectivo:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(totalEnEfectivo.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                iTextFont fontDif = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, diferencia < 0 ? BaseColor.RED : BaseColor.BLACK);
                tableResumen.AddCell(new PdfPCell(new Phrase("Diferencia:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(diferencia.ToString("$#,##0.00"), fontDif)) { Border = PdfPCell.NO_BORDER });

                doc.Add(tableResumen);

                Paragraph datosPie = new Paragraph($"\nEmpleado en turno: {empleado}\nObservaciones: {(string.IsNullOrWhiteSpace(observaciones) ? "Ninguna" : observaciones)}\n", fontCuerpo);
                doc.Add(datosPie);

                // ==========================================
                // TABLA DE DETALLES
                // ==========================================
                doc.Add(new Paragraph("\nDetalle de Movimientos:\n\n", fontHeader));

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
                        tabla.AddCell(new Phrase(row["Tipo"].ToString(), fontCuerpo));
                        tabla.AddCell(new Phrase(row["Concepto"].ToString(), fontCuerpo));

                        string tPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString() : "EFECTIVO";
                        tabla.AddCell(new Phrase(tPago, fontCuerpo));

                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        tabla.AddCell(new Phrase(monto.ToString("$#,##0.00"), fontCuerpo));
                    }
                }
                else
                {
                    PdfPCell celdaVacia = new PdfPCell(new Phrase("No hay movimientos registrados en este turno.", fontCuerpo));
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

        public class MarcaDeAgua : PdfPageEventHelper
        {
            private string rutaImagen;

            public MarcaDeAgua(string ruta)
            {
                rutaImagen = ruta;
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                if (System.IO.File.Exists(rutaImagen))
                {
                    PdfContentByte cb = writer.DirectContentUnder;
                    iTextImage img = iTextImage.GetInstance(rutaImagen);

                    PdfGState state = new PdfGState();
                    state.FillOpacity = 0.15f;
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
}