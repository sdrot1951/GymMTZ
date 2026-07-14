using GymApp.BLL;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;

// ALIAS PARA EVITAR CONFLICTOS CON WINDOWS FORMS
using iTextFont = iTextSharp.text.Font;
using iTextImage = iTextSharp.text.Image;

namespace GYMMTZ
{
    public partial class FrmCorteCaja : Form
    {
        private TextBox txtFondo, txtDeclarado, txtObservaciones;
        private Label lblMontoEsperado;

        public FrmCorteCaja()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(18, 18, 20); // El color oscuro de tu panel principal
            this.ForeColor = Color.White;
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Corte de Caja";

            // Borde sutil
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle,
                    Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            // Título
            Label lblTitulo = new Label { Text = "Arqueo de Caja", Font = new System.Drawing.Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            // Monto Esperado (Automático)
            Label lblInfo = new Label { Text = "Monto esperado por sistema:", Location = new Point(20, 70), AutoSize = true };
            lblMontoEsperado = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.Cyan, Location = new Point(20, 95), AutoSize = true };

            // Fondo Inicial
            Label lblFondo = new Label { Text = "Fondo Inicial:", Location = new Point(20, 140), AutoSize = true };
            txtFondo = new TextBox { Location = new Point(20, 160), Width = 340, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White };
            txtFondo.Text = "0.00";

            // Monto Declarado
            Label lblDeclarado = new Label { Text = "Efectivo Real en Caja:", Location = new Point(20, 200), AutoSize = true };
            txtDeclarado = new TextBox { Location = new Point(20, 220), Width = 340, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold) };

            // Observaciones
            Label lblObs = new Label { Text = "Observaciones:", Location = new Point(20, 260), AutoSize = true };
            txtObservaciones = new TextBox { Location = new Point(20, 280), Width = 340, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White };

            // Botón Guardar
            Button btnGuardar = new Button
            {
                Text = "✅ Realizar Corte",
                Location = new Point(20, 360),
                Width = 160, // Ajustamos ancho
                Height = 40,
                BackColor = Color.FromArgb(255, 69, 0),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnGuardar.Click += btnRealizarCorte_Click;

            // Botón Cancelar
            Button btnCancelar = new Button
            {
                Text = "❌ Cancelar",
                Location = new Point(200, 360), // Posición a la derecha del otro
                Width = 160,
                Height = 40,
                BackColor = Color.FromArgb(60, 60, 65), // Gris oscuro
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnCancelar.Click += (s, e) => this.Close(); // Cierra el formulario sin hacer nada

            // Agregamos ambos al formulario
            this.Controls.AddRange(new Control[] {
                lblTitulo, lblInfo, lblMontoEsperado, lblFondo, txtFondo,
                lblDeclarado, txtDeclarado, lblObs, txtObservaciones,
                btnGuardar, btnCancelar // <--- Asegúrate de incluirlo aquí
            });

            // Botón Cancelar/Cerrar Opcional
            Button btnCerrar = new Button { Text = "✖", Location = new Point(360, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitulo, lblInfo, lblMontoEsperado, lblFondo, txtFondo, lblDeclarado, txtDeclarado, lblObs, txtObservaciones, btnGuardar, btnCerrar });

            // Cargar monto esperado al abrir
            int empleadoTurno = GymApp.Core.SesionGlobal.IdEmpleado; // Usamos el ID global de la RAM
            CargarMontoEsperado(empleadoTurno);
        }

        private void CargarMontoEsperado(int empleadoTurno)
        {
            try
            {
                // 1. Instanciamos tu capa de negocio
                var bll = new GymApp.BLL.CajaBLL();

                // 2. Traemos el cálculo real desde SQL Server
                decimal esperado = bll.ObtenerTotalEsperadoDia(empleadoTurno);

                // 3. Lo formateamos a moneda para la pantalla
                lblMontoEsperado.Text = esperado.ToString("$#,##0.00");
            }
            catch (Exception ex)
            {
                // Si algo falla (ej. se cae la red), lo dejamos en cero y avisamos
                lblMontoEsperado.Text = "$0.00";
                MessageBox.Show("No se pudo cargar el monto esperado. Verifique su conexión.\nDetalle: " + ex.Message,
                                "Aviso del Sistema",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
            }
        }

        private void FrmCorteCaja_Load(object sender, EventArgs e)
        {

        }

        private void btnRealizarCorte_Click(object sender, EventArgs e)
        {
            try
            {
                decimal fondoInicial = Convert.ToDecimal(txtFondo.Text);
                decimal declarado = Convert.ToDecimal(txtDeclarado.Text);
                string observaciones = txtObservaciones.Text;
                //int idEmpleadoLogueado = 1; // Tu variable de sesión

                int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado; // Usamos el ID global de la RAM

                string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto; // Usamos el ID global de la RAM

                //string nombreEmpleado = idEmpleadoLogueado

                var bll = new CajaBLL();

                // 1. Guardamos el corte en SQL
                int nuevoIdCorte = bll.ProcesarCorteCaja(idEmpleadoLogueado, fondoInicial, declarado, observaciones);

                // 2. Traemos los movimientos reales (ejemplo: todo lo de hoy)
                DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, idEmpleadoLogueado);

                MessageBox.Show("Corte de caja registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3. Le pasamos la tabla real al PDF
                GenerarReportePDF(nuevoIdCorte, fondoInicial, declarado, nombreEmpleado, observaciones, dtMovimientos);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al realizar el corte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // CORRECCIÓN: Método compatible con iTextSharp v5
     
        public void GenerarReportePDF(int idCorte, decimal fondoInicial, decimal declarado, string empleado, string observaciones, DataTable dtMovimientos)
        {
            try
            {
                string nombreArchivo = $"Corte_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                Document doc = new Document(PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(nombreArchivo, FileMode.Create));

                // -- MARCA DE AGUA (Tu código del Toro) --
             

                string rutaLogo = Path.Combine(Application.StartupPath, "logo.jpg");
                writer.PageEvent = new MarcaDeAgua(rutaLogo);

                doc.Open();

                iTextFont fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextFont fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                iTextFont fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                iTextFont fontCuerpo = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Título central
                Paragraph titulo = new Paragraph("CORTE DE CAJA - GYM MTZ DEWS\n", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                Paragraph subtitulo = new Paragraph($"Folio de Corte: #{idCorte}\nFecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n", fontSubtitulo);
                subtitulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(subtitulo);

                // ==========================================
                // LÓGICA MATEMÁTICA PARA EL RESUMEN
                // ==========================================
                decimal ingresos = 0;
                decimal salidas = 0;

                // Diccionario para sumarizar cuánto entró por cada tipo de pago
                Dictionary<string, decimal> ingresosPorPago = new Dictionary<string, decimal>();

                if (dtMovimientos != null && dtMovimientos.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMovimientos.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        string tipoMov = row["Tipo"].ToString().ToUpper();

                        // Obtenemos el tipo de pago de SQL (Si no existe la columna por error, usamos Efectivo)
                        string tipoPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString().ToUpper() : "EFECTIVO";

                        if (tipoMov == "GASTO")
                        {
                            salidas += monto;
                        }
                        else
                        {
                            ingresos += monto; // Es Venta, Abono o Ingreso Extra

                            // Agrupamos el monto en su tipo de pago (Tarjeta, Efectivo, etc)
                            if (ingresosPorPago.ContainsKey(tipoPago))
                                ingresosPorPago[tipoPago] += monto;
                            else
                                ingresosPorPago.Add(tipoPago, monto);
                        }
                    }
                }
                decimal esperado = fondoInicial + ingresos - salidas;
                decimal diferencia = declarado - esperado;

                // ==========================================
                // TABLA DE RESUMEN AMPLIADA
                // ==========================================
                PdfPTable tableResumen = new PdfPTable(2);
                tableResumen.WidthPercentage = 75;
                tableResumen.HorizontalAlignment = Element.ALIGN_LEFT;
                tableResumen.SetWidths(new float[] { 40f, 60f });

                tableResumen.AddCell(new Phrase("Empleado en turno:", fontHeader));
                tableResumen.AddCell(new Phrase(empleado, fontCuerpo));

                tableResumen.AddCell(new Phrase("Fondo Inicial Caja:", fontHeader));
                tableResumen.AddCell(new Phrase(fondoInicial.ToString("$#,##0.00"), fontCuerpo));

                tableResumen.AddCell(new Phrase("Total de Ingresos:", fontHeader));
                tableResumen.AddCell(new Phrase(ingresos.ToString("$#,##0.00"), fontCuerpo));

                // ---> INYECTAMOS EL DESGLOSE DE MÉTODOS DE PAGO <---
                foreach (var item in ingresosPorPago)
                {
                    // Lo ponemos con un bullet point y cursiva visualmente para indicar que es un sub-dato
                    tableResumen.AddCell(new Phrase($"   • En {item.Key}:", fontCuerpo));
                    tableResumen.AddCell(new Phrase(item.Value.ToString("$#,##0.00"), fontCuerpo));
                }

                tableResumen.AddCell(new Phrase("Total de Salidas (Gastos):", fontHeader));
                tableResumen.AddCell(new Phrase(salidas.ToString("$#,##0.00"), fontCuerpo));

                tableResumen.AddCell(new Phrase("Monto Esperado en Sistema:", fontHeader));
                tableResumen.AddCell(new Phrase(esperado.ToString("$#,##0.00"), fontCuerpo));

                tableResumen.AddCell(new Phrase("Ingresos totales(Entregados):", fontHeader));
                tableResumen.AddCell(new Phrase(declarado.ToString("$#,##0.00"), fontCuerpo));

                iTextFont fontDif = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, diferencia < 0 ? BaseColor.RED : BaseColor.BLACK);
                tableResumen.AddCell(new Phrase("Diferencia:", fontHeader));
                tableResumen.AddCell(new Phrase(diferencia.ToString("$#,##0.00"), fontDif));

                tableResumen.AddCell(new Phrase("Observaciones:", fontHeader));
                tableResumen.AddCell(new Phrase(string.IsNullOrWhiteSpace(observaciones) ? "Ninguna" : observaciones, fontCuerpo));

                doc.Add(tableResumen);

                // ==========================================
                // TABLA DE DETALLES
                // ==========================================
                doc.Add(new Paragraph("\nDetalle de Movimientos:\n\n", fontHeader));

                PdfPTable tabla = new PdfPTable(5); // AHORA SON 5 COLUMNAS
                tabla.WidthPercentage = 100;
                tabla.SetWidths(new float[] { 12f, 18f, 35f, 20f, 15f }); // Ajustamos los anchos

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

                        // Inyectamos el método de pago
                        string tPago = dtMovimientos.Columns.Contains("TipoPago") ? row["TipoPago"].ToString() : "EFECTIVO";
                        tabla.AddCell(new Phrase(tPago, fontCuerpo));

                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        tabla.AddCell(new Phrase(monto.ToString("$#,##0.00"), fontCuerpo));
                    }
                }
                else
                {
                    PdfPCell celdaVacia = new PdfPCell(new Phrase("No hay movimientos registrados en este turno.", fontCuerpo));
                    celdaVacia.Colspan = 5; // AHORA SON 5
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
        // Pon esto fuera de la clase FrmCorteCaja, pero dentro del namespace
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
                    // DirectContentUnder nos permite dibujar "por debajo" del texto
                    PdfContentByte cb = writer.DirectContentUnder;
                    iTextImage img = iTextImage.GetInstance(rutaImagen);

                    // Configurar la transparencia (20% visible)
                    PdfGState state = new PdfGState();
                    state.FillOpacity = 0.15f;
                    cb.SetGState(state);

                    // Hacer la imagen grande y centrarla en la hoja
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