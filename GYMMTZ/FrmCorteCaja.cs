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
        //private TextBox txtFondo, txtDeclarado, txtObservaciones;
        /*private TextBox txtFondo, txtDeclaradoEfectivo, txtDeclaradoTransferencia, txtObservaciones;
        private Label lblMontoEsperado;*/

        private Label lblTotalEfectivo, lblTotalTransferencia, lblTotalGastos;
        private TextBox txtObservaciones;
        private DateTimePicker dtpFechaCorte;


        // Variables para guardar los cálculos del sistema
        private decimal sysEfectivo = 0;
        private decimal sysTransferencias = 0;
        private decimal sysGastos = 0;



        /* public FrmCorteCaja()
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

              // Declarado Efectivo
              Label lblDeclaradoEf = new Label { Text = "Efectivo Entregado:", Location = new Point(20, 200), AutoSize = true };
             txtDeclaradoEfectivo = new TextBox { Location = new Point(20, 220), Width = 160, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.LimeGreen, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold) };
             txtDeclaradoEfectivo.Text = "0.00";

             // Declarado Transferencia
             Label lblDeclaradoTrans = new Label { Text = "Transferencias:", Location = new Point(200, 200), AutoSize = true };
             txtDeclaradoTransferencia = new TextBox { Location = new Point(200, 220), Width = 160, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.DeepSkyBlue, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold) };
             txtDeclaradoTransferencia.Text = "0.00";
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
                 lblDeclaradoEf, lblDeclaradoTrans, lblObs, txtObservaciones,
                 btnGuardar, btnCancelar // <--- Asegúrate de incluirlo aquí
             });

             // Botón Cancelar/Cerrar Opcional
             Button btnCerrar = new Button { Text = "✖", Location = new Point(360, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat };
             btnCerrar.FlatAppearance.BorderSize = 0;
             btnCerrar.Click += (s, e) => this.Close();

             this.Controls.AddRange(new Control[] {
                 lblTitulo, lblInfo, lblMontoEsperado, lblFondo, txtFondo,
                 lblDeclaradoEf, txtDeclaradoEfectivo,           // <--- AQUÍ YA ESTÁ LA CAJA DE EFECTIVO
                 lblDeclaradoTrans, txtDeclaradoTransferencia,   // <--- AQUÍ YA ESTÁ LA CAJA DE TRANSFERENCIAS
                 lblObs, txtObservaciones,
                 btnGuardar, btnCancelar, btnCerrar
             });

             // Cargar monto esperado al abrir
             int empleadoTurno = GymApp.Core.SesionGlobal.IdEmpleado; // Usamos el ID global de la RAM
             CargarMontoEsperado(empleadoTurno);
         }*/


        /*  public FrmCorteCaja()

              {
              this.FormBorderStyle = FormBorderStyle.None;
              this.BackColor = Color.FromArgb(20, 20, 20);
              this.ForeColor = Color.White;
              // 1. Hacemos la ventana más alta (de 480 a 530) para que quepa el calendario
              this.Size = new Size(420, 530); 
              this.StartPosition = FormStartPosition.CenterParent;
              this.Text = "Cierre de Caja General";

              // Borde sutil
              this.Paint += (s, e) => {
                  ControlPaint.DrawBorder(e.Graphics, this.ClientRectangle, Color.FromArgb(45, 45, 45), ButtonBorderStyle.Solid);
              };

              Label title = new Label { Text = "🔒 Cierre General del Día", Font = new System.Drawing.Font("Segoe UI", 16, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true, ForeColor = Color.Gold };

              // ====== NUEVO: SELECTOR DE FECHA ======
              Label lblFechaSelector = new Label { Text = "Fecha a procesar:", Location = new Point(20, 70), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12), ForeColor = Color.DarkGray };
              dtpFechaCorte = new DateTimePicker
              {
                  Location = new Point(160, 68),
                  Width = 140,
                  Format = DateTimePickerFormat.Short,
                  Font = new System.Drawing.Font("Segoe UI", 12)
              };
              // Magia pura: Recalculamos el dinero en automático si eligen otro día en el calendario
              dtpFechaCorte.ValueChanged += (s, e) => CargarResumenTurno(GymApp.Core.SesionGlobal.IdEmpleado); 
              // ======================================

              // --- BLOQUE DE EFECTIVO (Bajamos Y a 130) ---
              Label lblEf = new Label { Text = "Efectivo Total en Caja:", Location = new Point(20, 130), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
              lblTotalEfectivo = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.LimeGreen, Location = new Point(20, 155), AutoSize = true };

              // --- BLOQUE DE TRANSFERENCIAS (Bajamos Y a 200) ---
              Label lblTr = new Label { Text = "Transferencias Totales:", Location = new Point(20, 200), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
              lblTotalTransferencia = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.DeepSkyBlue, Location = new Point(20, 225), AutoSize = true };

              // --- BLOQUE DE GASTOS (Bajamos Y a 270) ---
              Label lblGa = new Label { Text = "Gastos Totales (Salidas):", Location = new Point(20, 270), AutoSize = true, Font = new System.Drawing.Font("Segoe UI", 12) };
              lblTotalGastos = new Label { Text = "$0.00", Font = new System.Drawing.Font("Segoe UI", 18, FontStyle.Bold), ForeColor = Color.Crimson, Location = new Point(20, 295), AutoSize = true };

              // Observaciones (Bajamos Y a 350)
              Label lblObs = new Label { Text = "Observaciones de Auditoría (Opcional):", Location = new Point(20, 350), AutoSize = true };
              txtObservaciones = new TextBox { Location = new Point(20, 370), Width = 380, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

              // Botón Guardar (Bajamos Y a 460)
              Button btnCorte = new Button
              {
                  Text = "✅ CERRAR DÍA",
                  Location = new Point(20, 460),
                  Width = 180,
                  Height = 45,
                  BackColor = Color.LimeGreen,
                  ForeColor = Color.Black,
                  FlatStyle = FlatStyle.Flat,
                  Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Bold),
                  Cursor = Cursors.Hand
              };
              btnCorte.FlatAppearance.BorderSize = 0;
              btnCorte.Click += btnRealizarCorte_Click;

              // Botón Cancelar (Bajamos Y a 460)
              Button btnCancelar = new Button
              {
                  Text = "❌ CANCELAR",
                  Location = new Point(220, 460),
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

              // Botón Cerrar Superior
              Button btnCerrar = new Button { Text = "✖", Location = new Point(380, 10), Width = 30, Height = 30, ForeColor = Color.DarkGray, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
              btnCerrar.FlatAppearance.BorderSize = 0;
              btnCerrar.Click += (s, e) => this.Close();

              // Asegúrate de inyectar el lblFechaSelector y el dtpFechaCorte al form aquí:
              this.Controls.AddRange(new Control[] {
                  title, 
                  lblFechaSelector, dtpFechaCorte, // <--- AQUÍ LOS AGREGAMOS VISUALMENTE
                  lblEf, lblTotalEfectivo, 
                  lblTr, lblTotalTransferencia, 
                  lblGa, lblTotalGastos, 
                  lblObs, txtObservaciones, 
                  btnCorte, btnCancelar, btnCerrar
              });
          }*/

        public FrmCorteCaja()

        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.FromArgb(18, 18, 20);
            this.ForeColor = Color.White;
            this.Size = new Size(400, 480);
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
            // Cuando el admin cambie la fecha, recalculamos la pantalla
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

            // Observaciones
            Label lblObs = new Label { Text = "Observaciones del turno (Opcional):", Location = new Point(20, 300), AutoSize = true };
            txtObservaciones = new TextBox { Location = new Point(20, 320), Width = 360, Height = 60, Multiline = true, BackColor = Color.FromArgb(35, 35, 40), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

            // Botón Guardar
            Button btnGuardar = new Button
            {
                Text = "✅ Generar Corte",
                Location = new Point(20, 410),
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
                Location = new Point(210, 410),
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
                lblObs, txtObservaciones,
                btnGuardar, btnCancelar, btnCerrar
            });

            // Disparamos el cálculo automático usando el ID de la sesión actual
            CargarResumenTurno(GymApp.Core.SesionGlobal.IdEmpleado);
        }




        /*  private void CargarMontoEsperado(int empleadoTurno)
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
          }*/



        private void CargarResumenTurno(int idEmpleado)
        {
            try
            {
                var bll = new GymApp.BLL.CajaBLL();

                // Traemos EXACTAMENTE los movimientos de HOY para EL EMPLEADO LOGUEADO
               // DataTable dt = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, idEmpleado);

                DateTime fechaInicio = dtpFechaCorte.Value.Date;
                DateTime fechaFin = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                // Traemos los movimientos de ESA fecha
                DataTable dt = bll.ObtenerDetalleMovimientos(fechaInicio, fechaFin, idEmpleado);

                sysEfectivo = 0;
                sysTransferencias = 0;
                sysGastos = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        string tipoMov = row["Tipo"].ToString().ToUpper();
                        string tipoPago = dt.Columns.Contains("TipoPago") ? row["TipoPago"].ToString().ToUpper() : "EFECTIVO";

                        if (tipoMov == "GASTO")
                        {
                            sysGastos += monto;
                        }
                        else // Venta, Ingreso, etc.
                        {
                            if (tipoPago == "TRANSFERENCIA")
                                sysTransferencias += monto;
                            else
                                sysEfectivo += monto;
                        }
                    }
                }

                // Descontamos los gastos directamente del total en efectivo físico que debe haber en caja
                decimal efectivoNeto = sysEfectivo - sysGastos;

                // Pintamos la interfaz
                lblTotalEfectivo.Text = efectivoNeto.ToString("$#,##0.00");
                lblTotalTransferencia.Text = sysTransferencias.ToString("$#,##0.00");
                lblTotalGastos.Text = sysGastos.ToString("$#,##0.00");
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
                decimal declaradoEfectivo = sysEfectivo - sysGastos;
                decimal declaradoTransferencia = sysTransferencias;

                decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;

                // --- LA NUEVA REGLA DE NEGOCIO ---
                // Como es un arqueo ciego, el sistema espera exactamente lo que el sistema declara
                decimal montoEsperado = declaradoTotal;

                string observaciones = txtObservaciones.Text;
                int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
                string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto;

                var bll = new CajaBLL();

                // 1. Guardamos el corte en SQL (AÑADIMOS montoEsperado A LA LLAMADA)
                int nuevoIdCorte = bll.ProcesarCorteCaja(idEmpleadoLogueado, fondoInicial, montoEsperado, declaradoTotal, observaciones, dtpFechaCorte.Value.Date);

                DateTime fechaInicioPDF = dtpFechaCorte.Value.Date;
                DateTime fechaFinPDF = dtpFechaCorte.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);

                DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(fechaInicioPDF, fechaFinPDF, idEmpleadoLogueado);
                // =========================================================================

                MessageBox.Show("Corte de caja generado y guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // 2. Traemos los movimientos reales del día
               // DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, idEmpleadoLogueado);

               // MessageBox.Show("Corte de caja generado y guardado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 3. Imprimimos el PDF
                GenerarReportePDF(nuevoIdCorte, fondoInicial, declaradoEfectivo, declaradoTransferencia, nombreEmpleado, observaciones, dtMovimientos);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al generar el corte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*  private void btnRealizarCorte_Click(object sender, EventArgs e)
          {
              try
              {
                  decimal fondoInicial = Convert.ToDecimal(txtFondo.Text);
                  decimal declaradoEfectivo = Convert.ToDecimal(txtDeclaradoEfectivo.Text);
                  decimal declaradoTransferencia = Convert.ToDecimal(txtDeclaradoTransferencia.Text);

                  // Sumamos ambos para obtener el total entregado
                  decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;

                  string observaciones = txtObservaciones.Text;
                  int idEmpleadoLogueado = GymApp.Core.SesionGlobal.IdEmpleado;
                  string nombreEmpleado = GymApp.Core.SesionGlobal.NombreCompleto;

                  var bll = new CajaBLL();

                  // 1. Guardamos el corte en SQL (seguimos pasando el total para no romper tu lógica actual)
                  int nuevoIdCorte = bll.ProcesarCorteCaja(idEmpleadoLogueado, fondoInicial, declaradoTotal, observaciones);

                  // 2. Traemos los movimientos reales
                  DataTable dtMovimientos = bll.ObtenerDetalleMovimientos(DateTime.Today, DateTime.Now, idEmpleadoLogueado);

                  MessageBox.Show("Corte de caja registrado exitosamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                  // 3. Le pasamos al PDF los desgloses (OJO: Actualizaremos la firma del método en el paso 4)
                  GenerarReportePDF(nuevoIdCorte, fondoInicial, declaradoEfectivo, declaradoTransferencia, nombreEmpleado, observaciones, dtMovimientos);

                  this.DialogResult = DialogResult.OK;
                  this.Close();
              }
              catch (Exception ex)
              {
                  MessageBox.Show("Error al realizar el corte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
              }
          }

          */
        // CORRECCIÓN: Método compatible con iTextSharp v5

        public void GenerarReportePDF(int idCorte, decimal fondoInicial, decimal declaradoEfectivo, decimal declaradoTransferencia, string empleado, string observaciones, DataTable dtMovimientos) 
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

                //Paragraph subtitulo = new Paragraph($"Folio de Corte: #{idCorte}\nFecha: {DateTime.Now:dd/MM/yyyy HH:mm}\n\n", fontSubtitulo);
                //Paragraph subtitulo = new Paragraph($"Folio de Corte: #{(idCorte > 0 ? idCorte.ToString() : "N/A")}\nFecha del Corte: {dtpFechaCorte.Value:dd/MM/yyyy}\n\n", fontSubtitulo);
                Paragraph subtitulo = new Paragraph($"Fecha del Corte: {dtpFechaCorte.Value:dd/MM/yyyy}\n\n", fontSubtitulo);
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
          /*      decimal esperado = fondoInicial + ingresos - salidas;
                decimal diferencia = declarado - esperado;*/

                decimal declaradoTotal = declaradoEfectivo + declaradoTransferencia;
                decimal esperado = fondoInicial + ingresos - salidas;
                decimal diferencia = declaradoTotal - esperado;

                // ==========================================
                // TABLA DE RESUMEN AMPLIADA
                // ==========================================
                /*                PdfPTable tableResumen = new PdfPTable(2);
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
                                tableResumen.AddCell(new Phrase(declaradoTotal.ToString("$#,##0.00"), fontCuerpo));

                                // ---> NUEVO: DESGLOSE DE LO ENTREGADO <---
                                tableResumen.AddCell(new Phrase("   • En TRANSFERENCIA:", fontCuerpo));
                                tableResumen.AddCell(new Phrase(declaradoTransferencia.ToString("$#,##0.00"), fontCuerpo));

                                tableResumen.AddCell(new Phrase("   • En EFECTIVO:", fontCuerpo));
                                tableResumen.AddCell(new Phrase(declaradoEfectivo.ToString("$#,##0.00"), fontCuerpo));
                                // -----------------------------------------

                                iTextFont fontDif = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, diferencia < 0 ? BaseColor.RED : BaseColor.BLACK);
                                tableResumen.AddCell(new Phrase("Diferencia:", fontHeader));
                                tableResumen.AddCell(new Phrase(diferencia.ToString("$#,##0.00"), fontDif));



                                tableResumen.AddCell(new Phrase("Observaciones:", fontHeader));
                                tableResumen.AddCell(new Phrase(string.IsNullOrWhiteSpace(observaciones) ? "Ninguna" : observaciones, fontCuerpo));

                                doc.Add(tableResumen);


                                // ==========================================
                                // TABLA DE RESUMEN SIMPLIFICADA
                                // ==========================================
                                PdfPTable tableResumen = new PdfPTable(2);
                                tableResumen.WidthPercentage = 65; // Un poco más compacta
                                tableResumen.HorizontalAlignment = Element.ALIGN_LEFT;
                                tableResumen.SetWidths(new float[] { 55f, 45f });

                                // Extraemos los montos del sistema de forma segura
                                decimal mtoTransferencia = ingresosPorPago.ContainsKey("TRANSFERENCIA") ? ingresosPorPago["TRANSFERENCIA"] : 0;
                                decimal mtoEfectivo = ingresosPorPago.ContainsKey("EFECTIVO") ? ingresosPorPago["EFECTIVO"] : 0;

                                // Calculamos el efectivo neto que debería haber (Ingresos Efectivo - Salidas)
                                decimal totalEnEfectivo = mtoEfectivo - salidas;

                                // ROW 1: TOTAL DE INGRESO
                                tableResumen.AddCell(new PdfPCell(new Phrase("TOTAL DE INGRESO:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                                tableResumen.AddCell(new PdfPCell(new Phrase(ingresos.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                                // ROW 2: Transferencia
                                tableResumen.AddCell(new PdfPCell(new Phrase("Transferencia:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                                tableResumen.AddCell(new PdfPCell(new Phrase(mtoTransferencia.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                                // ROW 3: Total de salidas
                                tableResumen.AddCell(new PdfPCell(new Phrase("Total de salidas:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                                tableResumen.AddCell(new PdfPCell(new Phrase(salidas.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                                // ROW 4: Total en efectivo
                                tableResumen.AddCell(new PdfPCell(new Phrase("Total en efectivo:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                                tableResumen.AddCell(new PdfPCell(new Phrase(totalEnEfectivo.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                                // ROW 5: Diferencia (Con color condicional si falta dinero)
                                iTextFont fontDif = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, diferencia < 0 ? BaseColor.RED : BaseColor.BLACK);
                                tableResumen.AddCell(new PdfPCell(new Phrase("Diferencia:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                                tableResumen.AddCell(new PdfPCell(new Phrase(diferencia.ToString("$#,##0.00"), fontDif)) { Border = PdfPCell.NO_BORDER });

                                doc.Add(tableResumen);

                                // Datos del empleado y observaciones en un formato de texto simple abajo de la tabla
                                Paragraph datosPie = new Paragraph($"\nEmpleado en turno: {empleado}\nObservaciones: {(string.IsNullOrWhiteSpace(observaciones) ? "ok" : observaciones)}\n", fontCuerpo);
                                doc.Add(datosPie);


                */
                // ==========================================
                // TABLA DE RESUMEN SIMPLIFICADA
                // ==========================================
                PdfPTable tableResumen = new PdfPTable(2);
                tableResumen.WidthPercentage = 65; // Un poco más compacta
                tableResumen.HorizontalAlignment = Element.ALIGN_LEFT;
                tableResumen.SetWidths(new float[] { 55f, 45f });

                // Extraemos los montos del sistema de forma segura
                decimal mtoTransferencia = ingresosPorPago.ContainsKey("TRANSFERENCIA") ? ingresosPorPago["TRANSFERENCIA"] : 0;
                decimal mtoEfectivo = ingresosPorPago.ContainsKey("EFECTIVO") ? ingresosPorPago["EFECTIVO"] : 0;

                // Calculamos el efectivo neto que debería haber (Ingresos Efectivo - Salidas)
                decimal totalEnEfectivo = mtoEfectivo - salidas;

                // ROW 1: TOTAL DE INGRESO
                tableResumen.AddCell(new PdfPCell(new Phrase("TOTAL DE INGRESO:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(ingresos.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                // ROW 2: Transferencia
                tableResumen.AddCell(new PdfPCell(new Phrase("Transferencia:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(mtoTransferencia.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                // ROW 3: Total de salidas
                tableResumen.AddCell(new PdfPCell(new Phrase("Total de salidas:", fontCuerpo)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(salidas.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                // ROW 4: Total en efectivo
                tableResumen.AddCell(new PdfPCell(new Phrase("Total en efectivo:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(totalEnEfectivo.ToString("$#,##0.00"), fontCuerpo)) { Border = PdfPCell.NO_BORDER });

                // ROW 5: Diferencia (Con color condicional si falta dinero)
                iTextFont fontDif = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, diferencia < 0 ? BaseColor.RED : BaseColor.BLACK);
                tableResumen.AddCell(new PdfPCell(new Phrase("Diferencia:", fontHeader)) { Border = PdfPCell.NO_BORDER });
                tableResumen.AddCell(new PdfPCell(new Phrase(diferencia.ToString("$#,##0.00"), fontDif)) { Border = PdfPCell.NO_BORDER });

                doc.Add(tableResumen);

                // Datos del empleado y observaciones en un formato de texto simple abajo de la tabla
                Paragraph datosPie = new Paragraph($"\nEmpleado en turno: {empleado}\nObservaciones: {(string.IsNullOrWhiteSpace(observaciones) ? "ok" : observaciones)}\n", fontCuerpo);
                doc.Add(datosPie);
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