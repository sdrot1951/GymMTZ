using GymApp.BLL;
// ====== ALIAS PARA EVITAR CONFLICTOS CON WINDOWS FORMS ======
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
        private decimal _entradas = 0;
        private decimal _salidas = 0;
        private decimal _esperado = 0;

        private Label lblEntradas, lblSalidas, lblEsperado, lblDiferencia;
        private TextBox txtFondoInicial, txtDeclarado, txtObservaciones;

        public FrmCorteCajaDia()
        {
            InitializeComponent();
            ConfigurarUI();
            CargarDatosPrevios();
        }

        private void FrmCorteCajaDia_Load(object sender, EventArgs e)
        {
        }

        private void ConfigurarUI()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(20, 20, 20);
            this.ForeColor = Color.White;
            this.Size = new Size(420, 520); // Un poco más alto para que quepa todo
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Arqueo y Corte de Caja Diario";

            // Borde sutil
            this.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
            };

            Label title = new Label { Text = "🔒 Cierre de Caja General", Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            // Botón Cerrar
            Button btnCerrar = new Button { Text = "✖", Location = new Point(380, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat };
            btnCerrar.FlatAppearance.BorderSize = 0;
            btnCerrar.Click += (s, e) => this.Close();

            // Fila 1: Fondo Inicial
            Label l1 = new Label { Text = "Fondo Inicial Caja:", Location = new Point(20, 80), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 10) };
            txtFondoInicial = new TextBox { Location = new Point(200, 75), Width = 150, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, Font = new System.Drawing.Font("Segoe UI", 12), Text = "0.00" };
            txtFondoInicial.TextChanged += Calcular;

            // Fila 2: Entradas
            Label l2 = new Label { Text = "Total Ventas/Abonos (+):", Location = new Point(20, 120), AutoSize = true };
            lblEntradas = new Label { Text = "$0.00", Location = new Point(200, 120), ForeColor = Color.LimeGreen, Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };

            // Fila 3: Salidas
            Label l3 = new Label { Text = "Total Gastos (-):", Location = new Point(20, 160), AutoSize = true };
            lblSalidas = new Label { Text = "$0.00", Location = new Point(200, 160), ForeColor = Color.FromArgb(255, 69, 0), Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold), AutoSize = true };

            // Línea separadora
            Panel sep = new Panel { BackColor = Color.DarkGray, Height = 1, Width = 360, Location = new Point(20, 195) };

            // Fila 4: Total Esperado
            Label l4 = new Label { Text = "TOTAL ESPERADO:", Location = new Point(20, 215), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold) };
            lblEsperado = new Label { Text = "$0.00", Location = new Point(200, 215), ForeColor = Color.Cyan, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };

            // Fila 5: Lo que cuenta el Cajero
            Label l5 = new Label { Text = "Efectivo Físico Contado:", Location = new Point(20, 265), AutoSize = true };
            txtDeclarado = new TextBox { Location = new Point(200, 260), Width = 150, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold), Text = "0.00" };
            txtDeclarado.TextChanged += Calcular;

            // Fila 6: Diferencia
            Label l6 = new Label { Text = "DIFERENCIA:", Location = new Point(20, 310), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold) };
            lblDiferencia = new Label { Text = "$0.00", Location = new Point(200, 310), ForeColor = Color.White, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };

            // Observaciones
            Label l7 = new Label { Text = "Observaciones (Si hay diferencia):", Location = new Point(20, 350), AutoSize = true };
            txtObservaciones = new TextBox { Location = new Point(20, 370), Width = 360, Height = 40, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White };

            // Botón Guardar
            Button btnCorte = new Button { Text = "✅ GENERAR CORTE", Location = new Point(20, 430), Width = 180, Height = 40, BackColor = Color.LimeGreen, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold) };
            btnCorte.Click += BtnCorte_Click;

            // Botón Cancelar
            Button btnCancelar = new Button { Text = "❌ CANCELAR", Location = new Point(210, 430), Width = 170, Height = 40, BackColor = Color.FromArgb(60, 60, 65), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new System.Drawing.Font("Segoe UI", 10, FontStyle.Bold) };
            btnCancelar.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { btnCerrar, title, l1, txtFondoInicial, l2, lblEntradas, l3, lblSalidas, sep, l4, lblEsperado, l5, txtDeclarado, l6, lblDiferencia, l7, txtObservaciones, btnCorte, btnCancelar });
        }

        private void CargarDatosPrevios()
        {
            try
            {
                var bll = new CajaBLL();

                // 1. Obtenemos los movimientos ESTRICTAMENTE DE HOY
                DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, 0);

                _entradas = 0;
                _salidas = 0;

                // 2. Sumamos nosotros mismos
                if (dtMovimientos != null && dtMovimientos.Rows.Count > 0)
                {
                    foreach (DataRow row in dtMovimientos.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        string tipoMov = row["Tipo"].ToString().ToUpper();

                        if (tipoMov == "GASTO")
                            _salidas += monto;
                        else
                            _entradas += monto; // Ventas, Abonos, Ingresos
                    }
                }

                // 3. Pintamos en pantalla
                lblEntradas.Text = _entradas.ToString("$#,##0.00");
                lblSalidas.Text = _salidas.ToString("$#,##0.00");
                Calcular(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar datos de hoy: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Calcular(object sender, EventArgs e)
        {
            decimal fondo = 0, declarado = 0;
            decimal.TryParse(txtFondoInicial.Text, out fondo);
            decimal.TryParse(txtDeclarado.Text, out declarado);

            _esperado = fondo + _entradas - _salidas;
            lblEsperado.Text = _esperado.ToString("$#,##0.00");

            decimal diferencia = declarado - _esperado;
            lblDiferencia.Text = diferencia.ToString("$#,##0.00");

            // Semáforo de diferencia
            if (diferencia < 0) lblDiferencia.ForeColor = Color.FromArgb(255, 69, 0); // Rojo faltante
            else if (diferencia > 0) lblDiferencia.ForeColor = Color.Gold; // Amarillo sobrante
            else lblDiferencia.ForeColor = Color.LimeGreen; // Verde exacto
        }

        private void BtnCorte_Click(object sender, EventArgs e)
        {
            decimal fondo, declarado;
            if (!decimal.TryParse(txtFondoInicial.Text, out fondo)) fondo = 0;
            if (!decimal.TryParse(txtDeclarado.Text, out declarado)) declarado = 0;

            decimal diferencia = declarado - _esperado;

            if (diferencia != 0 && string.IsNullOrWhiteSpace(txtObservaciones.Text))
            {
                MessageBox.Show("Existe una diferencia de " + diferencia.ToString("C2") + ". Es OBLIGATORIO ingresar una observación explicando el motivo.", "Auditoría", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtObservaciones.Focus();
                return;
            }

            if (MessageBox.Show("¿Estás seguro de cerrar la caja general de HOY? Ya no se podrán modificar ni eliminar los movimientos.", "Confirmar Corte", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
                    string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto ?? "Usuario Admin";

                    var bll = new CajaBLL();
                    DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, 0);

                    // ========================================================
                    // SOLUCIÓN FOLIO: Usamos RegistrarCorte para obtener el ID
                    // ========================================================
                    int idCorteGenerado = bll.GenerarCorte(idEmpleadoLogueado, fondo, declarado, txtObservaciones.Text);

                    if (idCorteGenerado > 0)
                    {
                        MessageBox.Show("Corte de caja registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (MessageBox.Show("¿Desea generar y guardar el reporte en PDF?", "Generar Reporte", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            // AHORA SÍ LE PASAMOS EL ID REAL EN LUGAR DE UN 0
                            GenerarReportePDF(idCorteGenerado, fondo, declarado, nombreEmpleado, txtObservaciones.Text, dtMovimientos);
                        }

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // =========================================================================
        // MÉTODOS PARA GENERACIÓN DE PDF
        // =========================================================================
        public void GenerarReportePDF(int idCorte, decimal fondoInicial, decimal declarado, string empleado, string observaciones, DataTable dtMovimientos)
        {
            try
            {
                // SOLUCIÓN ARCHIVO BLOQUEADO: Agregamos "ss" (segundos) al nombre
                string nombreArchivo = $"Corte_Diario_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

                // USO EXPLÍCITO DE ITEXTSHARP PARA EVITAR ERROR CS0103/CS0246
                iTextSharp.text.Document doc = new iTextSharp.text.Document(iTextSharp.text.PageSize.A4, 25, 25, 30, 30);
                PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(nombreArchivo, FileMode.Create));

                // -- MARCA DE AGUA --
                string rutaLogo = Path.Combine(Application.StartupPath, "logo.jpg");
                writer.PageEvent = new MarcaDeAgua(rutaLogo);

                doc.Open();

                // USO DE ALIAS PARA FUENTES
                iTextFont fontTitulo = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18);
                iTextFont fontSubtitulo = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                iTextFont fontHeader = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                iTextFont fontCuerpo = FontFactory.GetFont(FontFactory.HELVETICA, 10);

                // Título central
                Paragraph titulo = new Paragraph("CORTE DE CAJA DIARIO - GYM MTZ DEWS\n", fontTitulo);
                titulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(titulo);

                Paragraph subtitulo = new Paragraph($"Folio de Corte: #{(idCorte > 0 ? idCorte.ToString() : "N/A")}\nFecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n", fontSubtitulo);
                subtitulo.Alignment = Element.ALIGN_CENTER;
                doc.Add(subtitulo);

                // ==========================================
                // LÓGICA MATEMÁTICA PARA EL RESUMEN
                // ==========================================
                //decimal ingresos = _entradas; // Reutilizamos lo calculado en pantalla
                //decimal salidas = _salidas;
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
                decimal esperado = _esperado;
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

                tableResumen.AddCell(new Phrase("Efectivo Real (Declarado):", fontHeader));
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
    }

    // ==========================================
    // CLASE PARA LA MARCA DE AGUA (FUERA DE LA CLASE DEL FORM)
    // ==========================================
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