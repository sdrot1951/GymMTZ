using GymApp.BLL;
using GYMMTZ.Controls;
using GYMMTZ.Theme;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace GYMMTZ
{
    public partial class FrmMenuPrincipal : Form
    {



        // ─── MENÚ ITEMS ───────────────────────────────────────────────────────
        private class MenuItem
        {
            public string Icon;
            public string Title;
            public string Subtitle;
            public string Section;
            //public Action OnClick; // Se quitó el '?' para compatibilidad con C# 7.3
        }

        // Se declaró explícitamente el tipo de la lista en el 'new'
        private readonly List<(string section, List<MenuItem> items)> _menu = new List<(string section, List<MenuItem> items)>()
        {
            ("PRINCIPAL", new List<MenuItem>
            {
                new MenuItem { Icon = "⚡", Title = "Dashboard",    Subtitle = "Resumen general", Section = "PRINCIPAL" },
            }),
            ("OPERACIONES", new List<MenuItem>
            {
                new MenuItem { Icon = "🛒", Title = "Ventas",        Subtitle = "Nueva venta / historial", Section = "OPERACIONES" },
                new MenuItem { Icon = "💰", Title = "Caja",          Subtitle = "Movimientos de caja", Section = "OPERACIONES" },
                new MenuItem { Icon = "✂️", Title = "Corte de Caja", Subtitle = "Arqueo de turno", Section = "OPERACIONES" },
                new MenuItem { Icon = "🏷️", Title = "Descuentos",   Subtitle = "Gestión de descuentos", Section = "OPERACIONES" },
                new MenuItem { Icon = "💳", Title = "Membresías", Subtitle = "Vigencias y estados", Section = "CATÁLOGOS" },
                new MenuItem { Icon = "💵", Title = "Cobranza", Subtitle = "Cuentas por cobrar", Section = "OPERACIONES" },
                new MenuItem { Icon = "↩️", Title = "Cancelaciones", Subtitle = "Devoluciones de ventas", Section = "OPERACIONES" }, // <-- NUEVA LÍNEA

            }),
            ("CATÁLOGOS", new List<MenuItem>
            {
                new MenuItem { Icon = "👤", Title = "Clientes",      Subtitle = "Registro de miembros", Section = "CATÁLOGOS" },
                new MenuItem { Icon = "👥", Title = "Empleados",     Subtitle = "Personal del gimnasio", Section = "CATÁLOGOS" },
                new MenuItem { Icon = "📦", Title = "Productos",     Subtitle = "Inventario y rubros", Section = "CATÁLOGOS" },
            }),
            ("REPORTES", new List<MenuItem>
            {
                new MenuItem { Icon = "📊", Title = "Gastos",        Subtitle = "Control de gastos", Section = "REPORTES" },
                new MenuItem { Icon = "📊", Title = "Gastos Mensuales", Subtitle = "Control de gastos mensuales", Section = "REPORTES" },
                new MenuItem { Icon = "📋", Title = "Inventario",    Subtitle = "Stock de productos", Section = "REPORTES" },
                new MenuItem { Icon = "📦", Title = "Compras",    Subtitle = "Compras Actualiza Inventario", Section = "REPORTES" },
            }),
            ("SISTEMA", new List<MenuItem>
            {
                new MenuItem { Icon = "⚙️", Title = "Categorías",   Subtitle = "Rubros, pagos, gastos",    Section = "SISTEMA" },
                new MenuItem { Icon = "🔐", Title = "Asistencias",       Subtitle = "Historial de entradas",       Section = "SISTEMA" },
                new MenuItem { Icon = "🔐", Title = "Acceso",       Subtitle = "Accesos de miembros",       Section = "SISTEMA" },
            }),

        };

        private Panel _sidebar;
        private Panel _header;
        private Panel _content;
        private Panel _selectedItem;
        private string _currentPage = "Dashboard";
        private bool _menuExpanded = true;
        private Label _lblPageTitle;
        private Label _lblPageSub;
        private Label _lblUser;
        private Label _lblPuesto;
        private DataGridView _currentGrid;
        private Timer _timerBusqueda = new Timer(); // <-- AGREGA ESTO
        private TextBox _currentSearchBox; // <-- Agrega esta línea

        private DateTimePicker _dtpDesde;  // <-- Agrega esta
        private DateTimePicker _dtpHasta;  // <-- Agrega esta
        public FrmMenuPrincipal()
        {
            InitializeComponent();
            InitializeCustomConfig();
            BuildUI();
            SetupMenuActions();
            NavigateTo("Dashboard");
            // Configura el timer
            _timerBusqueda.Interval = 400;
            _timerBusqueda.Tick += (s, e) => {
                _timerBusqueda.Stop();
                if (_currentSearchBox != null)
                {
                    EjecutarBusqueda(_currentSearchBox.Text); // <-- Ahora sí le enviamos el texto
                }
            };

            SetupMenuActions();
            NavigateTo("Dashboard");




        }

        private void InitializeCustomConfig()
        {
            SuspendLayout();
            // 
            // MainForm
            // 
            BackColor = Color.FromArgb(18, 18, 20);
            ClientSize = new Size(1264, 761);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
            ForeColor = Color.FromArgb(240, 240, 245);
            MinimumSize = new Size(1024, 680);
            Name = "FrmMenuPrincipal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "GYM MTZ DEWS Fitness — Sistema de Gestión";
            Load += FrmMenuPrincipal_Load;
            ResumeLayout(false);
        }

        private void BuildUI()
        {
            // ── SIDEBAR ────────────────────────────────────────────────────
            _sidebar = new Panel
            {
                Width = GymTheme.MenuWidth,
                Dock = DockStyle.Left,
                BackColor = GymTheme.MenuBackground,
                Padding = new Padding(0)
            };

            // Logo / Brand
            var brandPanel = new Panel
            {
                Height = GymTheme.HeaderHeight,
                Dock = DockStyle.Top,
                BackColor = GymTheme.Background,
            };

            var lblLogo = new Label
            {
                Text = "🏋️ GYM MTZ DEWS",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = GymTheme.Accent,
                TextAlign = ContentAlignment.MiddleLeft,
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 0, 0, 0)
            };

            // Botón colapsar menú
            var btnCollapse = new Button
            {
                Text = "◀",
                Width = 32,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = GymTheme.TextSecondary,
                Font = new Font("Segoe UI", 9f),
                Cursor = Cursors.Hand,
                Dock = DockStyle.Right,
                Margin = new Padding(0, 16, 8, 16),
            };
            btnCollapse.FlatAppearance.BorderSize = 0;
            btnCollapse.Click += (s, e) => ToggleMenu(btnCollapse);

            brandPanel.Controls.Add(lblLogo);
            brandPanel.Controls.Add(btnCollapse);

            // Separador
            var sep = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = GymTheme.Border };

            // ScrollPanel para los items de menú
            var menuScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 8, 8, 8)
            };
            menuScroll.HorizontalScroll.Enabled = false;

            // Construir items de menú
            int yOffset = 8;
            foreach (var (section, items) in _menu)
            {
                // Label de sección
                var lblSection = new Label
                {
                    Text = section,
                    Font = new Font("Segoe UI", 7.5f, FontStyle.Bold),
                    ForeColor = GymTheme.TextMuted,
                    AutoSize = false,
                    Height = 28,
                    Width = GymTheme.MenuWidth - 32,
                    Location = new Point(8, yOffset),
                    TextAlign = ContentAlignment.BottomLeft,
                    Padding = new Padding(6, 0, 0, 2),
                    Tag = "section"
                };
                menuScroll.Controls.Add(lblSection);
                yOffset += 30;

                foreach (var item in items)
                {
                    var itemPanel = CreateMenuItemPanel(item, yOffset);
                    menuScroll.Controls.Add(itemPanel);
                    yOffset += 54;
                }

                yOffset += 6;
            }

            // Footer del sidebar — usuario logueado
            var sideFooter = new Panel
            {
                Height = 64,
                Dock = DockStyle.Bottom,
                BackColor = GymTheme.Background,
                Padding = new Padding(12, 0, 12, 0)
            };


            _lblUser = new Label
            {
                Text = "👤  " + (GymApp.Core.SesionGlobal.NombreCompleto ?? "Usuario"),
                Font = GymTheme.FontBold,
                ForeColor = GymTheme.TextPrimary,
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblPuesto = new Label
            {
                Text = GymApp.Core.SesionGlobal.NombrePuesto ?? "Administrador",
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextSecondary,
                AutoSize = false,
                Height = 18,
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleLeft
            };


            sideFooter.Controls.Add(_lblUser);
            sideFooter.Controls.Add(_lblPuesto);

            var sepFooter = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = GymTheme.Border };
            sideFooter.Controls.Add(sepFooter);

            _sidebar.Controls.Add(menuScroll);
            _sidebar.Controls.Add(sideFooter);
            _sidebar.Controls.Add(sep);
            _sidebar.Controls.Add(brandPanel);

            // ── ÁREA DERECHA ───────────────────────────────────────────────
            var rightPanel = new Panel { Dock = DockStyle.Fill, BackColor = GymTheme.Background };

            // Header superior
            _header = new Panel
            {
                Height = GymTheme.HeaderHeight,
                Dock = DockStyle.Top,
                BackColor = GymTheme.HeaderBackground,
                Padding = new Padding(24, 0, 24, 0)
            };

            _lblPageTitle = new Label
            {
                Text = "Dashboard",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = GymTheme.TextPrimary,
                AutoSize = false,
                Width = 400,
                Height = 36,
                Location = new Point(24, 10),
                TextAlign = ContentAlignment.MiddleLeft
            };

            _lblPageSub = new Label
            {
                Text = "Resumen general del sistema",
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextSecondary,
                AutoSize = false,
                Width = 400,
                Height = 20,
                Location = new Point(25, 40),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Fecha/hora en el header
            var lblDate = new Label
            {
                Text = DateTime.Now.ToString("dd MMM yyyy"),
                Font = GymTheme.FontBody,
                ForeColor = GymTheme.TextSecondary,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleRight
            };
            lblDate.Location = new Point(_header.Width - lblDate.PreferredWidth - 120, 22);
            lblDate.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Botón de notificaciones
            var btnNotif = new Button
            {
                Text = "🔔",
                Width = 40,
                Height = 40,
                FlatStyle = FlatStyle.Flat,
                BackColor = GymTheme.SurfaceElevated,
                ForeColor = GymTheme.TextPrimary,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 14f),
                TextAlign = ContentAlignment.MiddleCenter,
            };
            btnNotif.FlatAppearance.BorderColor = GymTheme.Border;
            btnNotif.FlatAppearance.BorderSize = 1;
            btnNotif.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNotif.Location = new Point(0, 12);

            var headerRight = new Panel
            {
                Width = 100,
                Dock = DockStyle.Right,
                BackColor = Color.Transparent
            };
            headerRight.Controls.Add(btnNotif);
            btnNotif.Location = new Point(30, 12);

            var headerSep = new Panel { Height = 1, Dock = DockStyle.Bottom, BackColor = GymTheme.Border };

            _header.Controls.Add(_lblPageTitle);
            _header.Controls.Add(_lblPageSub);
            _header.Controls.Add(headerRight);
            _header.Controls.Add(headerSep);

            // Área de contenido
            _content = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = GymTheme.Background,
                Padding = new Padding(GymTheme.Padding),
                AutoScroll = true
            };

            rightPanel.Controls.Add(_content);
            rightPanel.Controls.Add(_header);

            Controls.Add(rightPanel);
            Controls.Add(_sidebar);
        }


        private Panel CreateMenuItemPanel(MenuItem item, int yPos)
        {
            var panel = new Panel
            {
                Width = GymTheme.MenuWidth - 32,
                Height = 48,
                Location = new Point(4, yPos),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = item.Title
            };

            var lblIcon = new Label
            {
                Text = item.Icon,
                Font = new Font("Segoe UI", 16f),
                ForeColor = GymTheme.TextSecondary,
                Size = new Size(44, 48),
                Location = new Point(8, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };

            var lblTitle = new Label
            {
                Text = item.Title,
                Font = GymTheme.FontBold,
                ForeColor = GymTheme.TextPrimary,
                AutoSize = false,
                Location = new Point(54, 8),
                Size = new Size(140, 18),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblSub = new Label
            {
                Text = item.Subtitle,
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextMuted,
                AutoSize = false,
                Location = new Point(54, 26),
                Size = new Size(140, 16),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Borde izquierdo indicador (oculto inicialmente)
            var indicator = new Panel
            {
                Width = 3,
                Location = new Point(0, 6),
                Height = 36,
                BackColor = GymTheme.Accent,
                Visible = false
            };

            panel.Controls.Add(lblIcon);
            panel.Controls.Add(lblTitle);
            panel.Controls.Add(lblSub);
            panel.Controls.Add(indicator);

            // Eventos hover
            void SetHover(bool on)
            {
                if (panel == _selectedItem) return;
                panel.BackColor = on ? Color.FromArgb(30, 255, 69, 0) : Color.Transparent;
                lblIcon.ForeColor = on ? GymTheme.Accent : GymTheme.TextSecondary;
            }



            panel.MouseEnter += (s, e) => SetHover(true);
            panel.MouseLeave += (s, e) => SetHover(false);
            foreach (Control c in panel.Controls)
            {
                c.MouseEnter += (s, e) => SetHover(true);
                c.MouseLeave += (s, e) => SetHover(false);
            }

            panel.Click += (s, e) =>
            {
                SelectMenuItem(panel, lblIcon, lblTitle, lblSub, indicator);
                NavigateTo(item.Title);
            };
            foreach (Control c in panel.Controls)
                c.Click += (s, e) =>
                {
                    SelectMenuItem(panel, lblIcon, lblTitle, lblSub, indicator);
                    NavigateTo(item.Title);
                };

            return panel;
        }

        private void SelectMenuItem(Panel panel, Label icon, Label title, Label sub, Panel indicator)
        {
            // Deseleccionar anterior
            if (_selectedItem != null)
            {
                _selectedItem.BackColor = Color.Transparent;
                foreach (Control c in _selectedItem.Controls)
                {
                    if (c is Label l && l.Font.Size == 16) l.ForeColor = GymTheme.TextSecondary;
                    if (c is Label lt && lt.Name != "sub") lt.ForeColor = GymTheme.TextPrimary;
                    if (c is Panel ind) ind.Visible = false;
                }
            }
            _selectedItem = panel;
            panel.BackColor = Color.FromArgb(40, 255, 69, 0);
            icon.ForeColor = GymTheme.Accent;
            title.ForeColor = GymTheme.Accent;
            indicator.Visible = true;
        }

        private void ToggleMenu(Button btn)
        {
            _menuExpanded = !_menuExpanded;
            _sidebar.Width = _menuExpanded ? GymTheme.MenuWidth : 64;
            btn.Text = _menuExpanded ? "◀" : "▶";

            // Mostrar/ocultar textos
            foreach (Control c in _sidebar.Controls)
                ShowHideMenuTexts(c, _menuExpanded);
        }

        private void ShowHideMenuTexts(Control parent, bool show)
        {
            foreach (Control c in parent.Controls)
            {
                if (c.Tag is string t && t == "section") c.Visible = show;
                if (c is Panel p && !(c.Tag is string))
                {
                    foreach (Control cc in p.Controls)
                    {
                        if (cc is Label lbl && lbl.Location.X > 44)
                            lbl.Visible = show;
                    }
                    ShowHideMenuTexts(p, show);
                }
            }
        }

        private void SetupMenuActions() { /* Las acciones se configuran en CreateMenuItemPanel */ }

        // ─── NAVEGACIÓN ───────────────────────────────────────────────────────
        private void NavigateTo(string page)
        {
            _currentPage = page;
            _lblPageTitle.Text = page;
            //    _content.Controls.Clear();

            foreach (Control c in _content.Controls)
            {
                if (c is FrmChecador frmChecadorZombi)
                {
                    frmChecadorZombi.ApagarLector(); // Obligamos a que suelte el USB
                    frmChecadorZombi.Close();
                    frmChecadorZombi.Dispose();      // Lo borramos de la memoria RAM
                }
            }
            // ===============================================================

            _content.Controls.Clear();

            switch (page)
            {
                case "Dashboard": LoadDashboard(); break;
                case "Clientes": LoadClientes(); break;
                case "Empleados": LoadEmpleados(); break;
                case "Productos": LoadProductos(); break;
                case "Ventas": LoadVentas(); break;
                case "Caja": LoadCaja(); break;
                case "Corte de Caja": LoadCortesDeCaja(); break;
                case "Gastos": LoadGastos(); break;
                case "Cobranza": LoadCobranza(); break;
                case "Cancelaciones": LoadCancelaciones(); break; // <-- NUEVA LÍNEA
                case "Gastos Mensuales": LoadGastosMensuales(); break;
                case "Inventario": LoadInventario(); break;
                case "Compras": LoadCompras(); break;
                case "Descuentos": LoadDescuentos(); break;
                case "Asistencias": LoadAsistencias(); break;
                case "Acceso": AbrirChecadorEnPanel(); break;
                case "Categorías": LoadCategorias(); break;
                case "Membresías": LoadMembresiasPantalla(); break;
                default: LoadComingSoon(page); break;
            }
        }

        // ─── DASHBOARD ────────────────────────────────────────────────────────
        private void LoadDashboard()
        {
            _lblPageSub.Text = "Resumen general del sistema";
            try
            {
                // 1. Obtenemos toda la información de un solo golpe
                var bll = new GymApp.BLL.DashboardBLL();
                DataSet ds = bll.ObtenerDatosDashboard();

                DataRow kpis = ds.Tables[0].Rows[0];
                DataTable dtUltimasVentas = ds.Tables[1];

                // 2. Extraemos y formateamos los valores para las tarjetas (Cards)
                string totalClientes = kpis["ClientesActivos"].ToString();
                string ventasDia = Convert.ToDecimal(kpis["VentasDia"]).ToString("$#,##0.00");
                string totalProd = kpis["TotalProductos"].ToString();
                string prodBajoStock = kpis["ProductosBajoStock"].ToString();
                string gastosMes = Convert.ToDecimal(kpis["GastosMes"]).ToString("$#,##0.00");


                // KPI Cards
                var kpiData = new[]
                          {
                    ("👤", "Clientes Activos", totalClientes, "En el sistema", GymTheme.Accent),
                    ("💰", "Ventas del Día", ventasDia, "Hoy", GymTheme.Success),
                    ("📦", "Productos Stock", totalProd, $"{prodBajoStock} por agot.", GymTheme.Warning),
                    ("📊", "Gastos del Mes", gastosMes, "Mes actual", GymTheme.Danger),
                };



                int xPos = 0;
                int cardW = (_content.Width - GymTheme.Padding * 2 - 60) / 4;

                // 1. Damos más altura al contenedor principal (de 120 a 140)
                var kpiRow = new Panel
                {
                    Location = new Point(0, 0),
                    Width = _content.Width - GymTheme.Padding * 2,
                    Height = 140,
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                foreach (var (icon, label, value, sub, color) in kpiData)
                {
                    // 2. Damos más altura a cada tarjeta individual (de 110 a 130)
                    var card = new GymPanel
                    {
                        Width = cardW,
                        Height = 130,
                        Location = new Point(xPos, 0),
                        ShowAccentBorder = false
                    };

                    var pBar = new Panel { Height = 4, Dock = DockStyle.Top, BackColor = color };

                    // 3. Recalculamos las posiciones (Y) para que no choquen
                    var lblIcon = new Label { Text = icon, Font = new Font("Segoe UI", 22f), Location = new Point(14, 12), AutoSize = true, BackColor = Color.Transparent };
                    var lblVal = new Label { Text = value, Font = new Font("Segoe UI", 18f, FontStyle.Bold), ForeColor = GymTheme.TextPrimary, Location = new Point(14, 52), AutoSize = true, BackColor = Color.Transparent };
                    var lblLab = new Label { Text = label, Font = GymTheme.FontSmall, ForeColor = GymTheme.TextSecondary, Location = new Point(14, 86), AutoSize = true, BackColor = Color.Transparent };
                    var lblSub = new Label { Text = sub, Font = GymTheme.FontSmall, ForeColor = color, Location = new Point(14, 104), AutoSize = true, BackColor = Color.Transparent };

                    card.Controls.Add(pBar); card.Controls.Add(lblIcon); card.Controls.Add(lblVal); card.Controls.Add(lblLab); card.Controls.Add(lblSub);
                    kpiRow.Controls.Add(card);
                    xPos += cardW + 16;
                }

                _content.Controls.Add(kpiRow);

                // 4. Bajamos la tabla de ventas un poco (de Y=130 a Y=150) para que no se encime con las tarjetas nuevas
                var panelVentas = new GymPanel
                {
                    Location = new Point(0, 150),
                    Width = (_content.Width - GymTheme.Padding * 2 - 20) * 2 / 3,
                    Height = 320,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                var lblVTitle = new Label { Text = "Últimas Ventas", Font = GymTheme.FontSubtitle, ForeColor = GymTheme.TextPrimary, Location = new Point(16, 12), AutoSize = true, BackColor = Color.Transparent };

                var grid = new DataGridView
                {
                    Location = new Point(10, 44),
                    Size = new Size(panelVentas.Width - 20, panelVentas.Height - 60),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                };


                GymGrid.ApplyStyle(grid);
                grid.Columns.Add("Folio", "# Folio");
                grid.Columns.Add("Cliente", "Cliente");
                grid.Columns.Add("Monto", "Monto");
                grid.Columns.Add("Fecha", "Fecha");
                grid.Columns.Add("Estado", "Empleado");



                foreach (DataRow row in dtUltimasVentas.Rows)
                {
                    grid.Rows.Add(
                        "#" + row["Folio"].ToString(),
                        row["Cliente"].ToString(),
                        Convert.ToDecimal(row["Monto"]).ToString("$#,##0.00"),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                        row["Cliente"].ToString()
                    //row["Estado"].ToString()
                    );
                }

                panelVentas.Controls.Add(lblVTitle);
                panelVentas.Controls.Add(grid);
                _content.Controls.Add(panelVentas);

                // Panel lateral - accesos rápidos
                var panelQuick = new GymPanel
                {
                    // AQUÍ ESTÁ EL TRUCO: Asegúrate de que el segundo número sea 150
                    Location = new Point(panelVentas.Right + 16, 150),
                    Width = _content.Width - GymTheme.Padding * 2 - panelVentas.Width - 20,
                    Height = 320,
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };

                var lblQTitle = new Label
                {
                    Text = "Acceso Rápido",
                    Font = GymTheme.FontSubtitle,
                    ForeColor = GymTheme.TextPrimary,
                    Location = new Point(16, 12),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                panelQuick.Controls.Add(lblQTitle);

                var quickBtns = new[]
                {
                    ("🛒 Nueva Venta", GymButton.ButtonStyle.Primary, new Action(() => {
                        var frm = new FrmVenta();
                        if (frm.ShowDialog() == DialogResult.OK) { LoadDashboard(); } // Recarga al cerrar la venta
                    })),
                    ("👤 Nuevo Cliente", GymButton.ButtonStyle.Secondary, new Action(() => { new FrmClientes().ShowDialog(); })),
                    ("💸 Nuevo Gasto", GymButton.ButtonStyle.Secondary, new Action(() => {
                        var frm = new FrmGastos();
                        if (frm.ShowDialog() == DialogResult.OK) { LoadDashboard(); } // Recarga al guardar gasto
                    })),
                    ("📦 Ver Inventario", GymButton.ButtonStyle.Secondary, new Action(() => { NavigateTo("Inventario"); }))
                };

                int qY = 50;
                foreach (var (txt, style, action) in quickBtns)
                {
                    var btn = new GymButton { Text = txt, Style = style, Width = panelQuick.Width - 32, Location = new Point(16, qY) };
                    btn.Click += (s, e) => action.Invoke();
                    panelQuick.Controls.Add(btn);
                    qY += 52;
                }


                // ... (aquí termina tu código anterior del panelQuick) ...
                _content.Controls.Add(panelQuick);

                // ===================================================================
                // 5. NUEVO PANEL: Gráfica de Rendimiento Semanal (Últimos 7 Días)
                // ===================================================================
                // Lo posicionamos a Y=490 para que quede debajo de las ventas y botones
                var panelGrafica = new GymPanel
                {
                    Location = new Point(0, 490),
                    Width = _content.Width - GymTheme.Padding * 2,
                    Height = 280,
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };

                var lblGTitle = new Label
                {
                    Text = "📊 Balance Financiero (Últimos 7 Días)",
                    Font = GymTheme.FontSubtitle,
                    ForeColor = GymTheme.TextPrimary,
                    Location = new Point(16, 12),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };
                panelGrafica.Controls.Add(lblGTitle);

                // Configuración general de la Gráfica
                Chart chartDashboard = new Chart
                {
                    Location = new Point(16, 45),
                    Size = new Size(panelGrafica.Width - 32, panelGrafica.Height - 65),
                    BackColor = Color.Transparent,
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
                };

                // Configuración del fondo y las líneas cuadriculadas (Estilo Oscuro)
                ChartArea area = new ChartArea("MainArea") { BackColor = Color.Transparent };
                area.AxisX.LabelStyle.ForeColor = Color.DarkGray;
                area.AxisY.LabelStyle.ForeColor = Color.DarkGray;
                area.AxisX.LineColor = Color.FromArgb(50, 50, 55);
                area.AxisY.LineColor = Color.FromArgb(50, 50, 55);
                //area.AxisX.MajorGrid.LineColor = Color.FromArgb(35, 35, 40);
                area.AxisX.MajorGrid.Enabled = false;

                area.AxisY.MajorGrid.LineColor = Color.FromArgb(35, 35, 40);



                chartDashboard.ChartAreas.Add(area);

                // Leyenda indicadora (Ventas vs Gastos)
                Legend leyenda = new Legend("Leyenda")
                {
                    BackColor = Color.Transparent,
                    ForeColor = Color.LightGray,
                    Docking = Docking.Top,
                    Alignment = StringAlignment.Center
                };
                chartDashboard.Legends.Add(leyenda);

                // Serie 1: Ventas (Barras Verdes)
                Series serieVentas = new Series("Ventas ")
                {
                    ChartType = SeriesChartType.Column,
                    Color = GymTheme.Success, // Verde
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Double,

                    // ====== NUEVAS LÍNEAS: Mostrar montos en las barras ======
                    IsValueShownAsLabel = true,
                    LabelFormat = "$#,##0", // Formato moneda sin decimales para ahorrar espacio
                    LabelForeColor = Color.White,
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold)
                };

                // Serie 2: Gastos (Barras Naranjas)
                Series serieGastos = new Series("Gastos ")
                {
                    ChartType = SeriesChartType.Column,
                    Color = Color.FromArgb(255, 69, 0), // Naranja/Rojo
                    XValueType = ChartValueType.String,
                    YValueType = ChartValueType.Double,

                    // ====== NUEVAS LÍNEAS: Mostrar montos en las barras ======
                    IsValueShownAsLabel = true,
                    LabelFormat = "$#,##0",
                    LabelForeColor = Color.White,
                    Font = new Font("Segoe UI", 8f, FontStyle.Bold)
                };

                chartDashboard.Series.Add(serieVentas);
                chartDashboard.Series.Add(serieGastos);

                // Llenado de datos desde el tercer Result Set del Stored Procedure
                if (ds.Tables.Count > 2)
                {
                    DataTable dtGrafica = ds.Tables[2];
                    foreach (DataRow row in dtGrafica.Rows)
                    {
                        string dia = row["Dia"].ToString();
                        double ventas = Convert.ToDouble(row["Ventas"]);
                        double gastos = Convert.ToDouble(row["Gastos"]);

                        serieVentas.Points.AddXY(dia, ventas);
                        serieGastos.Points.AddXY(dia, gastos);
                    }
                }

                // Agregamos la gráfica al panel, y el panel a la pantalla principal
                panelGrafica.Controls.Add(chartDashboard);
                _content.Controls.Add(panelGrafica);

            } // <-- AQUÍ DEBE ESTAR LA LLAVE QUE CIERRA TU BLOQUE TRY
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Dashboard: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }




        }

        private void LoadGenericCrud(string title, string subtitle, string[] columns, object[][] sampleData, EventHandler eventoNuevo = null, EventHandler eventoEditar = null, EventHandler eventoEliminar = null, EventHandler eventoAbonar = null) // <-- NUEVO PARÁMETRO AL FINAL
        {
            _content.Controls.Clear();
            _lblPageSub.Text = subtitle;

            // Barra de herramientas
            var toolbar = new Panel
            {
                Location = new Point(0, 0),
                Width = _content.Width - GymTheme.Padding * 2,
                Height = 48,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            var btnNuevo = new GymButton { Text = "➕  Nuevo", Style = GymButton.ButtonStyle.Primary, Width = 120, Location = new Point(0, 4) };
            if (eventoNuevo != null) btnNuevo.Click += eventoNuevo;
            else btnNuevo.Visible = false;

            var btnEditar = new GymButton { Text = "✏️  Editar", Style = GymButton.ButtonStyle.Secondary, Width = 110, Location = new Point(128, 4) };
            if (eventoEditar != null) btnEditar.Click += eventoEditar;
            else btnEditar.Visible = false;

            var btnEliminar = new GymButton { Text = "🗑️  Eliminar", Style = GymButton.ButtonStyle.Danger, Width = 120, Location = new Point(246, 4) };
            if (eventoEliminar != null) btnEliminar.Click += eventoEliminar;
            else btnEliminar.Visible = false;

            // ====== NUEVO: BOTÓN EXCLUSIVO PARA ABONAR ======
            var btnAbonar = new GymButton { Text = "💵  Abonar", Style = GymButton.ButtonStyle.Primary, Width = 120, Location = new Point(540, 4) }; // <--- X cambiado a 540
            btnAbonar.BackColor = Color.LimeGreen;
            btnAbonar.ForeColor = Color.Black;

            // Permitimos que el botón se muestre tanto en Cobranza como en Huellas
            // Busca esta línea y agrega || _currentPage == "Clientes"
            if ((_currentPage == "Cobranza" || _currentPage == "Clientes") && eventoAbonar != null)
            {
                btnAbonar.Click += eventoAbonar;
                btnAbonar.Visible = true;

                if (_currentPage == "Clientes")
                {
                    // ====== MAGIA VISUAL: AHORA DICE VER PERFIL ======
                    btnAbonar.Text = "👤 Ver Perfil";
                    btnAbonar.BackColor = Color.DodgerBlue;
                    btnAbonar.ForeColor = Color.White;
                    btnAbonar.Width = 150;
                }
                else // Si es Cobranza
                {
                    //btnAbonar.Visible = false;
                    btnAbonar.Text = "💵 Abonar";
                    btnAbonar.BackColor = Color.LimeGreen;
                    btnAbonar.ForeColor = Color.Black;
                    btnAbonar.Width = 120;
                }
            }
            else
            {
                btnAbonar.Visible = false;
            }


            if (_currentPage == "Ventas" || _currentPage == "Caja" || _currentPage == "Descuentos" || _currentPage == "Gastos" || _currentPage == "Gastos Mensuales" || _currentPage == "Cobranza" || _currentPage == "Compras" || _currentPage == "Asistencias" || _currentPage == "Inventario")
            {
                Label lblDesde = new Label { Text = "Desde:", ForeColor = Color.DarkGray, AutoSize = true, Location = new Point(130, 12), Font = new Font("Segoe UI", 9f) };
                _dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Location = new Point(180, 10), Font = new Font("Segoe UI", 9f) };

                if (_currentPage == "Caja") _dtpDesde.Value = DateTime.Now.AddDays(0);

                Label lblHasta = new Label { Text = "Hasta:", ForeColor = Color.DarkGray, AutoSize = true, Location = new Point(290, 12), Font = new Font("Segoe UI", 9f) };
                _dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 100, Location = new Point(335, 10), Font = new Font("Segoe UI", 9f) };

                Button btnBuscarFechas = new Button
                {
                    Text = "📅 Filtrar",
                    Location = new Point(445, 8),
                    Size = new Size(80, 27),
                    BackColor = Color.FromArgb(45, 45, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
                btnBuscarFechas.FlatAppearance.BorderSize = 0;

                btnBuscarFechas.Click += (s, e) => EjecutarBusqueda(_currentSearchBox.Text);

                toolbar.Controls.Add(lblDesde);
                toolbar.Controls.Add(_dtpDesde);
                toolbar.Controls.Add(lblHasta);
                toolbar.Controls.Add(_dtpHasta);
                toolbar.Controls.Add(btnBuscarFechas);
            }

            var searchBox = new Panel
            {
                Width = 260,
                Height = 36,
                BackColor = GymTheme.SurfaceElevated,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Padding = new Padding(10, 8, 10, 8),
            };
            searchBox.Location = new Point(toolbar.Width - 266, 6);

            _currentSearchBox = new TextBox
            {
                Text = "🔍  Buscar...",
                BorderStyle = BorderStyle.None,
                BackColor = GymTheme.SurfaceElevated,
                ForeColor = GymTheme.TextPrimary,
                Font = GymTheme.FontBody,
                Dock = DockStyle.Fill,
                Margin = new Padding(8)
            };

            _currentSearchBox.Enter += (s, e) => {
                if (_currentSearchBox.Text == "🔍  Buscar...")
                {
                    _currentSearchBox.Text = "";
                    _currentSearchBox.ForeColor = Color.White;
                }
            };

            _currentSearchBox.Leave += (s, e) => {
                if (string.IsNullOrWhiteSpace(_currentSearchBox.Text))
                {
                    _currentSearchBox.Text = "🔍  Buscar...";
                    _currentSearchBox.ForeColor = GymTheme.TextSecondary;
                }
            };

            _currentSearchBox.TextChanged += (s, e) => {
                if (_currentSearchBox.Text == "🔍  Buscar...") return;

                _timerBusqueda.Stop();
                _timerBusqueda.Start();
            };

            searchBox.Click += (s, e) => _currentSearchBox.Focus();
            searchBox.Controls.Add(_currentSearchBox);

            // Agregamos los botones a la barra (incluyendo el de abonar)
            toolbar.Controls.Add(btnNuevo);
            toolbar.Controls.Add(btnEditar);
            toolbar.Controls.Add(btnEliminar);
            toolbar.Controls.Add(btnAbonar); // <--- Nuestro nuevo botón
            toolbar.Controls.Add(searchBox);

            var gridPanel = new GymPanel
            {
                Location = new Point(0, 56),
                Width = _content.Width - GymTheme.Padding * 2,
                Height = _content.Height - 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            _currentGrid = new DataGridView
            {
                Location = new Point(8, 8),
                Size = new Size(gridPanel.Width - 16, gridPanel.Height - 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            GymGrid.ApplyStyle(_currentGrid);

            foreach (var col in columns)
                _currentGrid.Columns.Add(col.Replace(" ", "_"), col);

            foreach (var row in sampleData)
                _currentGrid.Rows.Add(row);

            gridPanel.Controls.Add(_currentGrid);
            _content.Controls.Add(toolbar);
            _content.Controls.Add(gridPanel);
        }


        private void LoadAsistencias()
        {
            try
            {
                var bll = new GymApp.BLL.AccesoBLL();

                DateTime desde = _dtpDesde?.Value ?? DateTime.Now.Date;
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;
                string filtro = _currentSearchBox != null && _currentSearchBox.Text != "🔍  Buscar..." ? _currentSearchBox.Text.Trim() : "";

                DataTable dt = bll.ConsultarAsistencias(filtro, desde, hasta);

                var filasGrid = new List<object[]>();
                int totalAsistencias = dt.Rows.Count;

                foreach (DataRow row in dt.Rows)
                {
                    filasGrid.Add(new object[]
                    {
                        row["ClienteID"].ToString(),
                        row["NombreCliente"].ToString(),
                        Convert.ToDateTime(row["FechaHora"]).ToString("dd/MM/yyyy HH:mm:ss"),
                        row["EstatusAcceso"].ToString()
                    });
                }

                string subtitulo = $"Asistencias del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | TOTAL: {totalAsistencias} accesos";

                LoadGenericCrud("Historial de Asistencias", subtitulo,
                    new[] { "ID Cliente", "Nombre del Cliente", "Fecha y Hora", "Estatus de Acceso" },
                    filasGrid.ToArray(),
                    null, null, null, null // Sin botones de acción (CRUD no permitido en auditoría)
                );

                // Colorear el estatus visualmente en el Grid principal
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    string estatus = r.Cells["Estatus_de_Acceso"].Value.ToString();
                    if (estatus.Contains("PermitidoOK") || estatus.Contains("Activo"))
                        r.Cells["Estatus_de_Acceso"].Style.ForeColor = Color.LimeGreen;
                    else if (estatus.Contains("Vencer") || estatus.Contains("PorVencer"))
                        r.Cells["Estatus_de_Acceso"].Style.ForeColor = Color.Gold;
                    else
                        r.Cells["Estatus_de_Acceso"].Style.ForeColor = Color.Crimson;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar asistencias: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void EjecutarBusqueda(string texto)
        {
            try
            {
                // Solo buscamos si estamos en la pantalla de empleados (para no romper otras pantallas a futuro)
                if (_currentPage == "Empleados")
                {
                    var bll = new GymApp.BLL.EmpleadoBLL();
                    var resultados = bll.Buscar(texto);

                    _currentGrid.SuspendLayout(); // Pausa el dibujado visual para evitar parpadeos
                    _currentGrid.Rows.Clear();    // Limpia la tabla actual

                    foreach (var emp in resultados)
                    {
                        // Agregamos las filas mapeando exactamente igual que en LoadEmpleados
                        _currentGrid.Rows.Add(
                            emp.fiEmpleado,
                            emp.fcNombre,
                            emp.fcApePat,
                            emp.fcApeMat,
                            emp.fcNombrePuesto,
                            emp.fiTelefono == 0 ? "N/A" : emp.fiTelefono.ToString(),
                            emp.fcEmail,
                            emp.fdFechaNac.ToString("dd/MM/yyyy")
                        );
                    }
                    _currentGrid.ResumeLayout(); // Vuelve a dibujar la tabla ya con los datos filtrados
                }
                else if (_currentPage == "Clientes")
                {
                    var bll = new GymApp.BLL.ClienteBLL();

                    var resultados = string.IsNullOrWhiteSpace(texto) ? bll.ObtenerTodos() : bll.Buscar(texto);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    foreach (var cli in resultados)
                    {
                        // Asegúrate de que el orden mapeado aquí sea idéntico al de tus columnas en LoadClientes
                        _currentGrid.Rows.Add(
                            cli.fiCliente,
                            cli.fcNombre,
                            cli.fcApeMat,
                            cli.fcApePat,
                            cli.fiTelefono == 0 ? "N/A" : cli.fiTelefono.ToString(),
                            cli.fcEmail,
                            cli.fcEmergencia,
                            cli.fdFechaNac.ToString("dd/MM/yyyy"),
                            cli.fcObservaciones
                        );
                    }
                    _currentGrid.ResumeLayout();
                }
                else if (_currentPage == "Productos")
                {
                    var bll = new GymApp.BLL.ProductoBLL();

                    // Si el usuario borra el buscador traemos el catálogo completo, si escribe filtramos con el SP
                    var resultados = string.IsNullOrWhiteSpace(texto) ? bll.ObtenerTodos() : bll.Buscar(texto);

                    _currentGrid.SuspendLayout(); // Detiene el parpadeo visual
                    _currentGrid.Rows.Clear();

                    foreach (var prod in resultados)
                    {
                        // Mapeamos exactamente en el mismo orden que tu método LoadProductos
                        _currentGrid.Rows.Add(
                            prod.fiProducto,
                            prod.fcDescripcion,
                            "$" + prod.fiPrecio.ToString("0.00"), // Formato de moneda local
                            "$" + prod.fiCosto.ToString("0.00"),
                            prod.fcNombreRubro,
                            prod.fiCantidad.ToString()
                        );
                    }
                    _currentGrid.ResumeLayout(); // Dibuja los resultados filtrados
                }
                else if (_currentPage == "Ventas")
                {
                    var bll = new GymApp.BLL.VentaBLL();

                    // Validamos si el texto es la marca de agua del buscador
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // Tomamos las fechas de los controles
                    DateTime? desde = _dtpDesde?.Value;
                    DateTime? hasta = _dtpHasta?.Value;

                    var dtVentas = bll.ConsultarVentas(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    foreach (DataRow row in dtVentas.Rows)
                    {
                        _currentGrid.Rows.Add(
                            //"#" + row["Folio"].ToString(),
                            row["Cliente"].ToString(),
                            row["Articulos"].ToString(), // <--- 1. Aquí inyectamos el texto con el SKU
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                            Convert.ToDecimal(row["Total"]).ToString("$#,##0.00"),
                            Convert.ToDecimal(row["Deuda"]).ToString("$#,##0.00"),
                            Convert.ToDecimal(row["Saldo"]).ToString("$#,##0.00"),
                            row["Estado"].ToString(),
                            row["Vendedor"].ToString()





                        );
                    }
                    _currentGrid.ResumeLayout();
                }

                else if (_currentPage == "Caja")
                {
                    var bll = new GymApp.BLL.CajaBLL();
                    DataTable dt;

                    // Filtramos la marca de agua
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // ====== LÓGICA DE ENRUTAMIENTO LIMPIA ======
                    if (string.IsNullOrEmpty(filtroBusqueda))
                    {
                        // 1. Si NO hay texto (buscador vacío), consultamos estrictamente por las FECHAS del calendario
                        DateTime desde = _dtpDesde?.Value ?? DateTime.Now;
                        DateTime hasta = _dtpHasta?.Value ?? DateTime.Now;

                        dt = bll.ConsultarCaja(desde, hasta);
                    }
                    else
                    {
                        // 2. Si HAY texto, ignoramos el calendario y buscamos en todo el histórico
                        dt = bll.BuscarCajaTexto(filtroBusqueda);
                    }

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal saldoActual = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        saldoActual += monto;

                        int rowIndex = _currentGrid.Rows.Add(
                            // "#" + row["Folio"].ToString(),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                            row["Tipo"].ToString(),
                            monto.ToString("$#,##0.00"),
                            row["Pago"].ToString(),
                            row["Concepto"].ToString(),
                            row["Usuario"].ToString()
                        );

                        if (row["Tipo"].ToString() == "VENTA")
                            _currentGrid.Rows[rowIndex].Cells["Monto"].Style.ForeColor = Color.LimeGreen;
                        else
                            _currentGrid.Rows[rowIndex].Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                    _currentGrid.ResumeLayout();

                    // ====== ACTUALIZAR SUBTÍTULOS ======
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Movimientos del {_dtpDesde.Value:dd/MM/yyyy} al {_dtpHasta.Value:dd/MM/yyyy} | SALDO: {saldoActual.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados históricos para '{filtroBusqueda}' | SALDO: {saldoActual.ToString("$#,##0.00")}";
                }

                else if (_currentPage == "Membresías")
                {
                    var bll = new GymApp.BLL.MembresiaBLL();

                    // Limpiamos el texto del buscador
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    DataTable dt = bll.ConsultarMembresiasClientes(filtroBusqueda);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        int rowIndex = _currentGrid.Rows.Add(
                            row["fiCliente"].ToString(),
                            row["Cliente"].ToString(),
                            row["TipoMembresia"].ToString(),
                            Convert.ToDateTime(row["FechaInicio"]).ToString("dd/MM/yyyy"),
                            Convert.ToDateTime(row["FechaVencimiento"]).ToString("dd/MM/yyyy"),
                            row["DiasRestantes"].ToString(),
                            row["Estatus"].ToString()
                        );

                        // Reaplicamos los colores en la búsqueda
                        int dias = Convert.ToInt32(row["DiasRestantes"]);
                        if (dias <= 0 || row["Estatus"].ToString() == "Vencida")
                        {
                            _currentGrid.Rows[rowIndex].Cells["Días_Restantes"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                            _currentGrid.Rows[rowIndex].Cells["Estado"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                        }
                        else if (dias <= 5)
                        {
                            _currentGrid.Rows[rowIndex].Cells["Días_Restantes"].Style.ForeColor = Color.Yellow;
                        }
                        else
                        {
                            _currentGrid.Rows[rowIndex].Cells["Estado"].Style.ForeColor = Color.LimeGreen;
                        }
                    }
                    _currentGrid.ResumeLayout();
                }
                else if (_currentPage == "Descuentos")
                {
                    var bll = new GymApp.BLL.DescuentoBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // Obtenemos las fechas directamente de los controles del formulario
                    DateTime? desde = _dtpDesde?.Value;
                    DateTime? hasta = _dtpHasta?.Value;

                    DataTable dt = bll.ConsultarDescuentos(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal totalDescuentos = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["MontoDescuento"]);
                        totalDescuentos += monto;

                        _currentGrid.Rows.Add(

                            row["Descripcion"].ToString(),
                            monto.ToString("$#,##0.00"),
                            row["Venta"].ToString(),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                            row["Autoriza"].ToString()
                        );
                    }
                    _currentGrid.ResumeLayout();

                    // ====== ACTUALIZAR SUBTÍTULOS DINÁMICAMENTE ======
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Descuentos del {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy} | TOTAL DESCONTADO: {totalDescuentos.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados para '{filtroBusqueda}' | TOTAL DESCONTADO: {totalDescuentos.ToString("$#,##0.00")}";
                }

                // ... (tu código anterior de Descuentos) ...

                if (_currentPage == "Gastos")
                {
                    var bll = new GymApp.BLL.GastosBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // Leemos el calendario superior
                    DateTime? desde = _dtpDesde?.Value;
                    DateTime? hasta = _dtpHasta?.Value;

                    DataTable dt = bll.ConsultarGastos(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal totalGastos = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        totalGastos += monto;

                        int rowIndex = _currentGrid.Rows.Add(
                            //row["ID"].ToString(),
                            row["Descripcion"].ToString(),
                            row["TipoGasto"].ToString(),
                            monto.ToString("$#,##0.00"),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                             row["Empleado"].ToString()
                        );

                        // Mantenemos el estilo de color naranja/rojo al buscar
                        _currentGrid.Rows[rowIndex].Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                    _currentGrid.ResumeLayout();

                    // ====== ACTUALIZAMOS EL SUBTÍTULO CON LOS NUEVOS TOTALES ======
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Gastos del {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy} | TOTAL: {totalGastos.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados para '{filtroBusqueda}' | TOTAL: {totalGastos.ToString("$#,##0.00")}";
                }



                else if (_currentPage == "Gastos Mensuales")
                {
                    var bll = new GymApp.BLL.GastosBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // Leemos el calendario superior
                    DateTime? desde = _dtpDesde?.Value;
                    DateTime? hasta = _dtpHasta?.Value;

                    DataTable dt = bll.ConsultarGastosMensuales(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal totalGastos = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Monto"]);
                        totalGastos += monto;

                        int rowIndex = _currentGrid.Rows.Add(
                            //row["ID"].ToString(),
                            row["Descripcion"].ToString(),
                            row["TipoGasto"].ToString(),
                            monto.ToString("$#,##0.00"),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                             row["TipoGasto"].ToString()
                        );

                        // Mantenemos el estilo de color naranja/rojo al buscar
                        _currentGrid.Rows[rowIndex].Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                    _currentGrid.ResumeLayout();

                    // ====== ACTUALIZAMOS EL SUBTÍTULO CON LOS NUEVOS TOTALES ======
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Gastos del {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy} | TOTAL: {totalGastos.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados para '{filtroBusqueda}' | TOTAL: {totalGastos.ToString("$#,##0.00")}";
                }


                else if (_currentPage == "Inventario")
                {
                    var bll = new GymApp.BLL.InventarioBLL();
                    string filtroBusqueda = texto.Contains("Buscar...") ? "" : texto.Trim();

                    DateTime desde = _dtpDesde?.Value ?? DateTime.Now.AddDays(-30).Date;
                    DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;

                    DataTable dt = bll.ConsultarKardex(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        int rowIndex = _currentGrid.Rows.Add(
                            row["Folio"].ToString(),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                            row["Tipo"].ToString(),
                            row["Producto"].ToString(),
                            row["Cantidad"].ToString(),
                            row["Referencia"].ToString(),
                            row["Usuario"].ToString()
                        );

                        // Reaplicamos el color al buscar
                        string tipo = row["Tipo"].ToString().ToUpper();
                        if (tipo.Contains("ENTRADA"))
                        {
                            _currentGrid.Rows[rowIndex].Cells["Tipo"].Style.ForeColor = Color.LimeGreen;
                            _currentGrid.Rows[rowIndex].Cells["Cantidad"].Style.ForeColor = Color.LimeGreen;
                        }
                        else
                        {
                            _currentGrid.Rows[rowIndex].Cells["Tipo"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                            _currentGrid.Rows[rowIndex].Cells["Cantidad"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                        }
                    }
                    _currentGrid.ResumeLayout();

                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Historial de movimientos del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}";
                    else
                        _lblPageSub.Text = $"Arrastre para '{filtroBusqueda}' | {dt.Rows.Count} movimientos encontrados";

                }
                else if (_currentPage == "Cobranza")
                {
                    var bll = new GymApp.BLL.AbonosBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    DateTime? desde = null;
                    DateTime? hasta = null;

                    // Si la barra de búsqueda está vacía, hacemos caso a los calendarios
                    if (string.IsNullOrEmpty(filtroBusqueda))
                    {
                        desde = _dtpDesde?.Value;
                        hasta = _dtpHasta?.Value;
                    }

                    DataTable dt = bll.ConsultarDeudas(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal totalCarteraVencida = 0;

                    foreach (DataRow row in dt.Rows)
                    {
                        decimal restante = Convert.ToDecimal(row["SaldoPendiente"]);
                        totalCarteraVencida += restante;

                        _currentGrid.Rows.Add(
                            row["IDDeuda"],
                            row["FolioTicket"],
                            row["Cliente"],
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                            Convert.ToDecimal(row["TotalDeuda"]).ToString("$#,##0.00"),
                            Convert.ToDecimal(row["Abonado"]).ToString("$#,##0.00"),
                            restante.ToString("$#,##0.00")
                        );
                    }
                    _currentGrid.ResumeLayout();

                    // Actualizamos el subtítulo según lo que buscó el usuario
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Cartera Vencida del {desde.Value:dd/MM/yyyy} al {hasta.Value:dd/MM/yyyy} | TOTAL PENDIENTE: {totalCarteraVencida.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados para '{filtroBusqueda}' | TOTAL PENDIENTE: {totalCarteraVencida.ToString("$#,##0.00")}";
                }
                else if (_currentPage == "Compras")
                {
                    var bll = new GymApp.BLL.InventarioBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();

                    // 1. Extraemos las fechas de los calendarios
                    DateTime? desde = _dtpDesde?.Value;
                    DateTime? hasta = _dtpHasta?.Value;

                    // Si por algún motivo están nulos, usamos el mes actual
                    DateTime fechaDesde = desde ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    DateTime fechaHasta = hasta ?? DateTime.Now.Date;

                    // 2. Traemos los datos filtrados de SQL
                    DataTable dtCompras = bll.ConsultarCompras(filtroBusqueda, fechaDesde, fechaHasta);

                    // 3. Pausamos el dibujado visual para evitar parpadeos
                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    decimal totalCompras = 0;

                    foreach (DataRow row in dtCompras.Rows)
                    {
                        decimal monto = Convert.ToDecimal(row["Total"]);
                        totalCompras += monto;

                        int rowIndex = _currentGrid.Rows.Add(
                            "#" + row["Folio"].ToString(),
                            Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                            monto.ToString("$#,##0.00"),
                            row["Empleado"].ToString()
                        );

                        // Mantenemos el color naranja/rojo para salidas de dinero
                        _currentGrid.Rows[rowIndex].Cells["Monto_Total"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                    _currentGrid.ResumeLayout(); // Volvemos a dibujar

                    // 4. Actualizamos el Subtítulo dinámicamente con los totales
                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Compras del {fechaDesde:dd/MM/yyyy} al {fechaHasta:dd/MM/yyyy} | GASTO TOTAL: {totalCompras.ToString("$#,##0.00")}";
                    else
                        _lblPageSub.Text = $"Resultados para factura '{filtroBusqueda}' | GASTO TOTAL: {totalCompras.ToString("$#,##0.00")}";
                }
                else if (_currentPage == "Asistencias")
                {
                    var bll = new GymApp.BLL.AccesoBLL();
                    string filtroBusqueda = texto == "🔍  Buscar..." ? "" : texto.Trim();
                    DateTime desde = _dtpDesde?.Value ?? DateTime.Now.Date;
                    DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;

                    DataTable dt = bll.ConsultarAsistencias(filtroBusqueda, desde, hasta);

                    _currentGrid.SuspendLayout();
                    _currentGrid.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        int rowIndex = _currentGrid.Rows.Add(
                            row["ClienteID"].ToString(),
                            row["NombreCliente"].ToString(),
                            Convert.ToDateTime(row["FechaHora"]).ToString("dd/MM/yyyy HH:mm:ss"),
                            row["EstatusAcceso"].ToString()
                        );

                        // Colorear el estatus de la búsqueda
                        string estatus = row["EstatusAcceso"].ToString();
                        if (estatus.Contains("PermitidoOK") || estatus.Contains("Activo"))
                            _currentGrid.Rows[rowIndex].Cells["Estatus_de_Acceso"].Style.ForeColor = Color.LimeGreen;
                        else if (estatus.Contains("Vencer") || estatus.Contains("PorVencer"))
                            _currentGrid.Rows[rowIndex].Cells["Estatus_de_Acceso"].Style.ForeColor = Color.Gold;
                        else
                            _currentGrid.Rows[rowIndex].Cells["Estatus_de_Acceso"].Style.ForeColor = Color.Crimson;
                    }
                    _currentGrid.ResumeLayout();

                    if (string.IsNullOrEmpty(filtroBusqueda))
                        _lblPageSub.Text = $"Asistencias del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | TOTAL: {dt.Rows.Count} accesos";
                    else
                        _lblPageSub.Text = $"Resultados para '{filtroBusqueda}' | TOTAL: {dt.Rows.Count} accesos";
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al ejecutar la búsqueda: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // ─── PÁGINAS ──────────────────────────────────────────────────────────
        private void LoadClientes()
        {
            try
            {
                var bll = new GymApp.BLL.ClienteBLL();
                var listaClientes = bll.ObtenerTodos();
                var filasGrid = new List<object[]>();

                foreach (var cli in listaClientes)
                {
                    filasGrid.Add(new object[]
                    {
                cli.fiCliente,
                cli.fcNombre,
                cli.fcApePat,
                cli.fcApeMat,
                cli.fiTelefono == 0 ? "N/A" : cli.fiTelefono.ToString(),
                cli.fcEmail,
                cli.fcEmergencia,
                cli.fdFechaNac.ToString("dd/MM/yyyy"),
                cli.fcObservaciones
                //cli.flActivo ? "✅" : "❌"
                    });
                }

                LoadGenericCrud(
                    "Clientes",
                    "Registro y gestión de miembros del gimnasio",
                    new[] { "ID", "Nombre", "Ap. Paterno", "Ap. Materno", "Teléfono", "Email", "Telefono Emergencias", "Fecha Nacimiento", "Observaciones" },
                    filasGrid.ToArray(),

                    // Evento Nuevo
                    (sender, e) =>
                    {
                        FrmClientes frm = new FrmClientes(); // <-- Tienes que crear este formulario
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadClientes();
                        }
                    },

                    // Evento Editar (Lo dejamos vacío por ahora hasta armar el método editar)
                    // Evento Ver Perfil (Último parámetro de LoadGenericCrud)
                    (sender, e) =>
                    {
                        if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                        {
                            int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);

                            // Abrimos el nuevo Dashboard del Cliente
                            FrmClientes frmPerfil = new FrmClientes(idSeleccionado);
                            frmPerfil.ShowDialog();

                            // Recargamos la tabla al cerrar el perfil por si el cliente pagó una deuda o se actualizó algo
                            LoadClientes();
                        }
                        else
                        {
                            MessageBox.Show("Por favor, selecciona un cliente de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    },

                    // Evento Eliminar (Baja lógica)
                    // Evento Eliminar (Baja lógica)
                    (sender, e) =>
                    {
                        if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                        {
                            // Extraemos el ID y el Nombre de la fila seleccionada
                            int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                            string nombreCliente = _currentGrid.SelectedRows[0].Cells[1].Value.ToString();

                            // Lanzamos advertencia
                            DialogResult dialogo = MessageBox.Show(
                                $"¿Está seguro de que desea dar de baja al cliente: {nombreCliente}?\n\nEl cliente ya no aparecerá en las listas activas.",
                                "Confirmar Baja",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2);

                            if (dialogo == DialogResult.Yes)
                            {
                                try
                                {
                                    var bllEliminar = new GymApp.BLL.ClienteBLL();
                                    if (bllEliminar.Eliminar(idSeleccionado))
                                    {
                                        MessageBox.Show("Cliente dado de baja correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadClientes(); // Recargamos la tabla automáticamente
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor, selecciona toda la fila de un cliente dando clic en el margen izquierdo de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    },
                 // Este va a ser el ÚLTIMO parámetro de tu LoadGenericCrud en LoadClientes()
                 (sender, e) =>
                 {
                     if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                     {
                         int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);

                         FrmPerfilCliente frmPerfil = new FrmPerfilCliente(idSeleccionado);
                         frmPerfil.ShowDialog();

                         LoadClientes(); // Recargamos el Grid por si abonó deuda o cambió algo
                     }
                     else
                     {
                         MessageBox.Show("Por favor, selecciona un cliente de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                 }


                );


            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error al cargar clientes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadEmpleados()
        {
            try
            {
                // 1. Instanciamos la capa de negocio para consultar los datos reales
                var bll = new GymApp.BLL.EmpleadoBLL();
                var listaEmpleados = bll.ObtenerTodos();

                // 2. Creamos una lista dinámica para construir la matriz del Grid
                var filasGrid = new List<object[]>();

                // 3. Recorremos los empleados devueltos por el SP y los mapeamos a las columnas
                foreach (var emp in listaEmpleados)
                {
                    filasGrid.Add(new object[]
                    {
                emp.fiEmpleado,
                emp.fcNombre,
                emp.fcApeMat,
                emp.fcApePat,
                emp.fcNombrePuesto, // Muestra la descripción del puesto (ej: "Administrador") gracias al INNER JOIN
                emp.fiTelefono == 0 ? "N/A" : emp.fiTelefono.ToString(), // Validación rápida por si no tiene teléfono
                emp.fcEmail,
                emp.fdFechaNac.ToString("dd/MM/yyyy"), // Formato de fecha limpio para el usuario
                //emp.flActivo ? "✅" : "❌"
                    });
                }

                // 4. Enviamos la información real al método genérico de tu Dashboard
                LoadGenericCrud(
                    "Empleados",
                    "Personal del gimnasio",
                    new[] { "ID", "Nombre", "Ap. Materno", "Ap. Paterno", "Puesto", "Teléfono", "Email", "Fecha Nacimiento" },
                    filasGrid.ToArray(),

                    // 1. Evento del botón "➕ Nuevo"
                    (sender, e) =>
                    {
                        FrmNuevoEmpleado frm = new FrmNuevoEmpleado();
                        var resultado = frm.ShowDialog(); // Abre la ventana en modo modal

                        // Si el formulario regresó DialogResult.OK (significa que guardó con éxito)
                        if (resultado == DialogResult.OK)
                        {
                            LoadEmpleados(); // Recarga la tabla automáticamente para mostrar al nuevo empleado
                        }
                    },

                    // 2. Lambda del botón "✏️ Editar" (NUEVO)
                    (sender, e) => {
                        if (_currentGrid.SelectedRows.Count == 0)
                        {
                            MessageBox.Show("Por favor, seleccione el empleado que desea editar haciendo clic en el margen izquierdo de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Extraemos el ID seleccionado de la fila
                        int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells["ID"].Value);

                        // Abrimos el formulario enviándole el ID por el constructor (Modo Editar)
                        FrmNuevoEmpleado frm = new FrmNuevoEmpleado(idSeleccionado);
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadEmpleados(); // Recarga la tabla con los cambios actualizados
                        }
                    },

                    (sender, e) =>
                    {
                        if (_currentGrid.SelectedRows.Count == 0) return;
                        //{
                        //    MessageBox.Show("Por favor, seleccione la fila completa del empleado dando clic en el margen izquierdo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //    return;
                        //}


                        DataGridViewRow filaSeleccionada = _currentGrid.SelectedRows[0];

                        // Leemos de _currentGrid usando el nombre de la columna que definiste arriba ("ID" y "Nombre")
                        int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells["ID"].Value);
                        string nombreEmpleado = _currentGrid.SelectedRows[0].Cells["Nombre"].Value.ToString();

                        DialogResult dialogo = MessageBox.Show(
                            $"¿Está seguro de que desea dar de baja al empleado: {nombreEmpleado}?",
                            "Confirmar Baja",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button2);

                        if (dialogo == DialogResult.Yes)
                        {
                            try
                            {
                                var blleliminar = new GymApp.BLL.EmpleadoBLL();
                                if (blleliminar.EliminarEmpleado(idSeleccionado))
                                {

                                    //MessageBox.Show("Producto removido del catálogo correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    MessageBox.Show("Empleado dado de baja correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                    LoadEmpleados();

                                    // 2. ✨ LA MAGIA: Borramos solo la fila del grid. 
                                    // Cero parpadeos, cero recargas de base de datos.

                                    this.BeginInvoke((Action)(() => NavigateTo("Empleados")));
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                );


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el listado de empleados: " + ex.Message, "Error de Conexión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProductos()
        {
            try
            {
                var bll = new GymApp.BLL.ProductoBLL();
                var listaProductos = bll.ObtenerTodos();
                var filasGrid = new List<object[]>();

                foreach (var prod in listaProductos)
                {
                    filasGrid.Add(new object[]
                    {
                        prod.fiProducto,
                        prod.fcDescripcion,
                        "$" + prod.fiPrecio.ToString("0.00"),
                        "$" + prod.fiCosto.ToString("0.00"),
                        prod.fcNombreRubro,
                        prod.fiCantidad.ToString()
                    });
                }

                LoadGenericCrud(
                    "Productos e Inventario",
                    "Catálogo de artículos y existencias en almacén",
                    new[] { "ID", "Descripción", "Precio Venta", "Costo", "Rubro", "Stock Actual" },
                    filasGrid.ToArray(),

                    // Evento Nuevo
                    (sender, e) =>
                    {
                        // Abrimos el formulario de entrada transaccional de stock
                        FrmProducto frm = new FrmProducto();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadProductos(); // Al regresar, la tabla principal mostrará el nuevo stock sumado
                        }
                    },
                    (sender, e) =>
                    {
                        if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                        {
                            // Obtenemos el ID del producto de la primera celda
                            int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);

                            // Abrimos el formulario pasándole el ID seleccionado por parámetro
                            FrmProducto frm = new FrmProducto(idSeleccionado);
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                LoadProductos(); // Recargamos el listado para ver el reflejo de los cambios
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor, selecciona toda la fila de un producto haciendo clic en el margen izquierdo de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    },

                    // Reemplaza el evento Eliminar dentro de tu LoadProductos por este:
                    (sender, e) =>
                    {
                        if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                        {
                            // Extraemos el ID y la descripción de la fila seleccionada en el DataGrid
                            int idSeleccionado = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                            string descripcionProd = _currentGrid.SelectedRows[0].Cells[1].Value.ToString();

                            // Lanzamos cuadro de diálogo de advertencia
                            DialogResult dialogo = MessageBox.Show(
                                $"¿Está seguro de que desea dar de baja el producto:\n\n\"{descripcionProd}\"?\n\nEl artículo ya no aparecerá en el catálogo ni se podrá vender.",
                                "Confirmar Baja de Producto",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning,
                                MessageBoxDefaultButton.Button2);

                            if (dialogo == DialogResult.Yes)
                            {
                                try
                                {
                                    var bllEliminar = new GymApp.BLL.ProductoBLL();
                                    if (bllEliminar.Eliminar(idSeleccionado))
                                    {
                                        MessageBox.Show("Producto removido del catálogo correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        LoadProductos(); // Recargamos tu Grid global para ver los cambios en caliente
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message, "Error al eliminar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor, selecciona toda la fila de un producto haciendo clic en el margen izquierdo de la tabla.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar productos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadVentas()
        {
            try
            {
                var bll = new GymApp.BLL.VentaBLL();

                var dtVentas = bll.ConsultarVentas("", DateTime.Now, DateTime.Now);
                var filasGrid = new List<object[]>();

                foreach (DataRow row in dtVentas.Rows)
                {
                    filasGrid.Add(new object[]
                    {
               // "#" + row["Folio"].ToString(),
                row["Cliente"].ToString(),
                row["Articulos"].ToString(), // <--- 1. Aquí inyectamos el texto con el SKU
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                Convert.ToDecimal(row["Total"]).ToString("$#,##0.00"),
                Convert.ToDecimal(row["Deuda"]).ToString("$#,##0.00"),
                Convert.ToDecimal(row["Saldo"]).ToString("$#,##0.00"),
                row["Estado"].ToString(),
                row["Vendedor"].ToString()
                    });
                }

                LoadGenericCrud("Ventas (Punto de Venta)", "Historial y registro de ventas de mostrador",
                    // <--- 2. Aquí agregamos la cabecera "Artículos" para que el Grid dibuje la columna
                    new[] { "Cliente", "Artículos", "Fecha", "Monto", "Deuda", "Saldo", "Estado", "Vendedor" },
                    filasGrid.ToArray(),

                    (sender, e) =>
                    {
                        FrmVenta frmCaja = new FrmVenta();
                        if (frmCaja.ShowDialog() == DialogResult.OK)
                        {
                            LoadVentas();
                        }
                    },
                    null,
                    null
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar ventas: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCaja()
        {
            try
            {
                var bll = new GymApp.BLL.CajaBLL();

                // Obtenemos los últimos 5 días inicialmente
                // DateTime desde = DateTime.Now.AddDays(-1);
                DateTime desde = DateTime.Today;
                DateTime hasta = DateTime.Now;

                DataTable dt = bll.ConsultarCaja(desde, hasta);
                var filasGrid = new List<object[]>();

                decimal saldoActual = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["Monto"]);
                    saldoActual += monto;

                    filasGrid.Add(new object[]
                    {
                //"#" + row["Folio"].ToString(),
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                row["Tipo"].ToString(),
                monto.ToString("$#,##0.00"),
                row["Pago"].ToString(),
                row["Concepto"].ToString(),
                row["Usuario"].ToString()
                    });
                }

                // Mostramos el Saldo Total directamente en el subtítulo
                string subtitulo = $"Movimientos del periodo | SALDO TOTAL: {saldoActual.ToString("$#,##0.00")}";

                LoadGenericCrud("Caja Registradora", subtitulo,
                    new[] { "Fecha", "Tipo", "Monto", "Tipo Pago", "Concepto", "Usuario" },
                    filasGrid.ToArray(),
                    null, // <--- Esto oculta el botón Nuevo
                    null,
                    null
                );

                // Opcional: Pintar de color verde las ventas y rojo los gastos en el Grid
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    if (r.Cells["Tipo"].Value.ToString() == "VENTA")
                        r.Cells["Monto"].Style.ForeColor = Color.LimeGreen;
                    else
                        r.Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0); // Naranja rojizo
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la caja: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCortesDeCaja()
        {
            _content.Controls.Clear();
            _lblPageSub.Text = "Gestión y auditoría de cortes de turno y cierres diarios";

            // 1. Barra de Herramientas (Botones)
            Panel toolbar = new Panel
            {
                Location = new Point(0, 0),
                Width = _content.Width - (GymTheme.Padding * 2),
                Height = 50,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            GymButton btnCorteTurno = new GymButton
            {
                Text = "✂️  Corte de Turno",
                Style = GymButton.ButtonStyle.Primary, // Naranja
                Width = 200,
                Location = new Point(0, 5)
            };
            btnCorteTurno.Click += (s, e) =>
            {
                // Aquí llamaremos al formulario de Turno más adelante
                FrmCorteCaja frm = new FrmCorteCaja();
                if (frm.ShowDialog() == DialogResult.OK) LoadCortesDeCaja();
                //MessageBox.Show("Módulo para Corte de Turno en desarrollo", "Aviso");
            };

            GymButton btnCorteDiario = new GymButton
            {
                Text = "📅  Corte del Día",
                Style = GymButton.ButtonStyle.Secondary, // Gris oscuro
                Width = 200,
                Location = new Point(210, 5)
            };
            btnCorteDiario.Click += (s, e) =>
            {
                // Aquí llamaremos al formulario Diario más adelante
                FrmCorteCajaDia frm = new FrmCorteCajaDia();
                if (frm.ShowDialog() == DialogResult.OK) LoadCortesDeCaja();
                // MessageBox.Show("Módulo para Corte Diario en desarrollo", "Aviso");
            };

            toolbar.Controls.Add(btnCorteTurno);
            toolbar.Controls.Add(btnCorteDiario);
            _content.Controls.Add(toolbar);

            // 2. Panel Superior: Historial de Cortes de Turno
            GymPanel pnlTurnos = new GymPanel
            {
                Location = new Point(0, 60),
                Width = _content.Width - (GymTheme.Padding * 2),
                Height = 250,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblTurnos = new Label { Text = "📋 Historial de Cortes de Turno", Font = GymTheme.FontSubtitle, ForeColor = GymTheme.TextPrimary, Location = new Point(16, 12), AutoSize = true, BackColor = Color.Transparent };

            DataGridView gridTurnos = new DataGridView
            {
                Location = new Point(10, 44),
                Size = new Size(pnlTurnos.Width - 20, pnlTurnos.Height - 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            GymGrid.ApplyStyle(gridTurnos);
            // gridTurnos.Columns.Add("ID", "Folio");
            gridTurnos.Columns.Add("Empleado", "Empleado");
            gridTurnos.Columns.Add("Fecha", "Fecha / Hora");
            gridTurnos.Columns.Add("Esperado", "Esperado");
            gridTurnos.Columns.Add("Declarado", "Declarado");
            gridTurnos.Columns.Add("Diferencia", "Diferencia");

            // Datos de prueba (MOCK) para ver el diseño
            // ===== LLENADO REAL: CORTES DE TURNO =====
            try
            {
                var bllCaja = new GymApp.BLL.CajaBLL();
                DateTime desde = _dtpDesde?.Value ?? DateTime.Now.Date;
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;

                DataTable dtTurnos = bllCaja.ConsultarCortesTurno(desde, hasta);
                gridTurnos.Rows.Clear();

                foreach (DataRow row in dtTurnos.Rows)
                {
                    decimal diferencia = Convert.ToDecimal(row["Diferencia"]);
                    int rowIndex = gridTurnos.Rows.Add(
                        //  "#" + row["Folio"].ToString(),
                        row["Empleado"].ToString(),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                        Convert.ToDecimal(row["Esperado"]).ToString("$#,##0.00"),
                        Convert.ToDecimal(row["Declarado"]).ToString("$#,##0.00"),
                        diferencia.ToString("$#,##0.00")
                    );

                    // Semáforo de auditoría
                    if (diferencia < 0) gridTurnos.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.FromArgb(255, 69, 0); // Faltante
                    else if (diferencia > 0) gridTurnos.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.Gold; // Sobrante
                    else gridTurnos.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.LimeGreen; // Exacto
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar cortes de turno: " + ex.Message); }

            pnlTurnos.Controls.Add(lblTurnos);
            pnlTurnos.Controls.Add(gridTurnos);
            _content.Controls.Add(pnlTurnos);

            // 3. Panel Inferior: Historial de Cortes Diarios Generales
            GymPanel pnlDiarios = new GymPanel
            {
                Location = new Point(0, 325),
                Width = _content.Width - (GymTheme.Padding * 2),
                Height = _content.Height - 345, // Calcula el espacio restante hacia abajo
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            Label lblDiarios = new Label { Text = "📊 Historial de Cierres Diarios Generales", Font = GymTheme.FontSubtitle, ForeColor = GymTheme.TextPrimary, Location = new Point(16, 12), AutoSize = true, BackColor = Color.Transparent };

            DataGridView gridDiarios = new DataGridView
            {
                Location = new Point(10, 44),
                Size = new Size(pnlDiarios.Width - 20, pnlDiarios.Height - 60),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            GymGrid.ApplyStyle(gridDiarios);
            // gridDiarios.Columns.Add("ID", "Folio");
            gridDiarios.Columns.Add("Fecha", "Fecha del Corte");
            gridDiarios.Columns.Add("Entradas", "Total Entradas");
            gridDiarios.Columns.Add("Salidas", "Total Gastos");
            gridDiarios.Columns.Add("Declarado", "Ingresos Totales");
            gridDiarios.Columns.Add("Diferencia", "Diferencia Global");

            // Datos de prueba (MOCK) para ver el diseño
            // ===== LLENADO REAL: CORTES DIARIOS =====
            try
            {
                var bllCaja = new GymApp.BLL.CajaBLL();
                DateTime desde = _dtpDesde?.Value ?? DateTime.Now.Date;
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;

                DataTable dtDiarios = bllCaja.ConsultarCortesDiarios(desde, hasta);
                gridDiarios.Rows.Clear();

                foreach (DataRow row in dtDiarios.Rows)
                {
                    decimal diferencia = Convert.ToDecimal(row["Diferencia"]);
                    int rowIndex = gridDiarios.Rows.Add(
                        // "#" + row["Folio"].ToString(),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                        Convert.ToDecimal(row["Entradas"]).ToString("$#,##0.00"),
                        Convert.ToDecimal(row["Salidas"]).ToString("$#,##0.00"),
                        Convert.ToDecimal(row["Declarado"]).ToString("$#,##0.00"),
                        diferencia.ToString("$#,##0.00")
                    );

                    // Semáforo de auditoría
                    if (diferencia < 0) gridDiarios.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    else if (diferencia > 0) gridDiarios.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.Gold;
                    else gridDiarios.Rows[rowIndex].Cells["Diferencia"].Style.ForeColor = Color.LimeGreen;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error al cargar cortes diarios: " + ex.Message); }

            pnlDiarios.Controls.Add(lblDiarios);
            pnlDiarios.Controls.Add(gridDiarios);
            _content.Controls.Add(pnlDiarios);

            pnlDiarios.Controls.Add(lblDiarios);
            pnlDiarios.Controls.Add(gridDiarios);
            _content.Controls.Add(pnlDiarios);
        }

        private void LoadGastos()
        {
            try
            {
                var bll = new GymApp.BLL.GastosBLL();

                // Por defecto, mostraremos los gastos del mes actual
                // DateTime desde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime desde = DateTime.Today;
                DateTime hasta = DateTime.Now.Date;

                DataTable dtGastos = bll.ConsultarGastos("", desde, hasta);

                var filasGrid = new List<object[]>();
                decimal totalGastos = 0;

                foreach (DataRow row in dtGastos.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["Monto"]);
                    totalGastos += monto;

                    filasGrid.Add(new object[]
                    {

                        row["Descripcion"].ToString(),
                        row["TipoGasto"].ToString(), // Alineado con tu captura
                        monto.ToString("$#,##0.00"),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                        row["Empleado"].ToString()   // El empleado que agregamos en el SP
                    });
                }

                string subtitulo = $"Gastos del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | TOTAL: {totalGastos.ToString("$#,##0.00")}";

                LoadGenericCrud(
                    "Historial de Gastos",
                    subtitulo,
                    new[] { "Descripción", "Tipo Gasto", "Monto", "Fecha", "Empleado" },
                    filasGrid.ToArray(),
                    (sender, e) =>
                    {
                        FrmGastos frm = new FrmGastos();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadGastos(); // Recarga la tabla de inmediato
                        }
                    },
                    null, // <-- MAGIA: Al pasar null, el botón [Editar] desaparece
                    null  // <-- MAGIA: Al pasar null, el botón [Eliminar] desaparece
                );

                // Pintamos los montos de color naranja/rojo para indicar salidas de efectivo
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    r.Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los gastos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGastosMensuales()
        {
            try
            {
                var bll = new GymApp.BLL.GastosBLL();

                // Por defecto, mostraremos los gastos del mes actual
                DateTime desde = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime hasta = DateTime.Now.Date;

                DataTable dtGastos = bll.ConsultarGastosMensuales("", desde, hasta);

                var filasGrid = new List<object[]>();
                decimal totalGastos = 0;

                foreach (DataRow row in dtGastos.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["Monto"]);
                    totalGastos += monto;

                    filasGrid.Add(new object[]
                    {

                        row["Descripcion"].ToString(),
                        row["TipoGasto"].ToString(), // Alineado con tu captura
                        monto.ToString("$#,##0.00"),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                        row["Empleado"].ToString()   // El empleado que agregamos en el SP
                    });
                }

                string subtitulo = $"Gastos del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | TOTAL: {totalGastos.ToString("$#,##0.00")}";

                LoadGenericCrud(
                    "Historial de Gastos",
                    subtitulo,
                    new[] { "Descripción", "Tipo Gasto", "Monto", "Fecha", "Empleado" },
                    filasGrid.ToArray(),
                    (sender, e) =>
                    {
                        FrmGastosMensuales frm = new FrmGastosMensuales();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadGastosMensuales(); // Recarga la tabla de inmediato
                        }
                    },
                    null, // <-- MAGIA: Al pasar null, el botón [Editar] desaparece
                    null  // <-- MAGIA: Al pasar null, el botón [Eliminar] desaparece
                );

                // Pintamos los montos de color naranja/rojo para indicar salidas de efectivo
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    r.Cells["Monto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar los gastos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadInventario()
        {
            try
            {
                var bll = new GymApp.BLL.InventarioBLL();

                DateTime desde = _dtpDesde?.Value ?? DateTime.Now.AddDays(-30).Date;
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;
                string filtro = _currentSearchBox != null && !_currentSearchBox.Text.Contains("Buscar...") ? _currentSearchBox.Text.Trim() : "";

                DataTable dtInventario = bll.ConsultarKardex(filtro, desde, hasta);

                var filasGrid = new List<object[]>();

                foreach (DataRow row in dtInventario.Rows)
                {
                    filasGrid.Add(new object[]
                    {
                        row["Folio"].ToString(),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                        row["Tipo"].ToString(),
                        row["Producto"].ToString(),
                        row["Cantidad"].ToString(),
                        row["Referencia"].ToString(),
                        row["Usuario"].ToString()
                    });
                }

                string subtitulo = $"Historial de movimientos y arrastre del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy}";

                LoadGenericCrud(
                    "Kardex de Inventario",
                    subtitulo,
                    new[] { "ID Movto", "Fecha", "Tipo", "Producto", "Cantidad", "Referencia", "Usuario" },
                    filasGrid.ToArray(),
                    null, null, null // Ocultamos los botones CRUD, el historial no se borra
                );

                // ====== LÓGICA DE COLORES DEL KARDEX ======
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    string tipo = r.Cells["Tipo"].Value.ToString().ToUpper();

                    // Entradas en Verde, Salidas en Naranja
                    if (tipo.Contains("ENTRADA"))
                    {
                        r.Cells["Tipo"].Style.ForeColor = Color.LimeGreen;
                        r.Cells["Cantidad"].Style.ForeColor = Color.LimeGreen;
                    }
                    else
                    {
                        r.Cells["Tipo"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                        r.Cells["Cantidad"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el Kardex de inventario: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void LoadDescuentos()
        {
            try
            {
                var bll = new GymApp.BLL.DescuentoBLL();

                // Por defecto cargamos el día de hoy
                DateTime desde = DateTime.Now.Date;
                DateTime hasta = DateTime.Now.Date;

                DataTable dtDescuentos = bll.ConsultarDescuentos("", desde, hasta);

                var filasGrid = new List<object[]>();
                decimal totalDescuentos = 0;

                foreach (DataRow row in dtDescuentos.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["MontoDescuento"]);
                    totalDescuentos += monto;

                    filasGrid.Add(new object[]
                    {
                        //row["ID"].ToString(),
                        row["Descripcion"].ToString(),
                        monto.ToString("$#,##0.00"),
                        row["Venta"].ToString(),
                        Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                        row["Autoriza"].ToString()
                    });
                }

                // Subtítulo con el total descontado
                string subtitulo = $"Descuentos del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | TOTAL DESCONTADO: {totalDescuentos.ToString("$#,##0.00")}";

                LoadGenericCrud(
                    "Historial de Descuentos",
                    subtitulo,
                    new[] { "Descripción", "Monto", "Venta", "Fecha", "Autoriza" },
                    filasGrid.ToArray(),
                    null, null, null // Ocultamos botones CRUD
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar el historial de descuentos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadHuellas()
        {
            _lblPageSub.Text = "Seleccione un cliente para registrar su huella";

            var bll = new GymApp.BLL.ClienteBLL();

            // ====== REEMPLAZO AQUÍ: Solo cargamos los pendientes ======
            var listaClientes = bll.ObtenerClientesSinHuella();
            // ==========================================================

            var filas = new List<object[]>();

            foreach (var cli in listaClientes)
            {
                filas.Add(new object[] { cli.fiCliente, cli.fcNombre, cli.fcApePat, cli.fcApeMat });
            }

            LoadGenericCrud(
                "Registro Biométrico",
                "Seleccione un cliente de la lista para capturar su huella",
                new[] { "ID", "Nombre", "Ap. Paterno", "Ap. Materno" },
                filas.ToArray(),
                null,
                null,
                null,

                (sender, e) =>
                {
                    if (_currentGrid.SelectedRows.Count > 0)
                    {
                        int idCliente = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);

                        FrmHuellas frm = new FrmHuellas(idCliente);

                        // Si la huella se guardó con éxito, recargamos la pantalla
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadHuellas(); // Al recargar, el cliente desaparecerá mágicamente de la lista porque ya tiene huella
                        }
                    }
                    else
                    {
                        MessageBox.Show("Por favor, seleccione toda la fila de un cliente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            );
        }

        private void LoadCategorias()
        {
            // 1. FUNDAMENTAL: Limpiar para que no se dibujen copias empalmadas al recargar
            _content.Controls.Clear();
            _lblPageSub.Text = "Configuración de catálogos, membresías y parámetros del sistema";

            int xPos = 0;
            int yPos = 0;

            // 2. ESTRUCTURA INTELIGENTE: Título, ID del BLL y Columnas visuales
            var cats = new[]
            {
                new { Titulo = "Rubros de Productos", Tipo = "Rubro", Cols = new[] { "ID", "Descripción" } },
                new { Titulo = "Categorías de Gastos", Tipo = "Gasto", Cols = new[] { "ID", "Descripción" } },
                new { Titulo = "Métodos de Pagos", Tipo = "Pago", Cols = new[] { "ID", "Descripción" } },
                new { Titulo = "Visitas y Pases", Tipo = "Visita", Cols = new[] { "ID", "Descripción", "Precio" } },
                new { Titulo = "Tipos de Membresía", Tipo = "Membresia", Cols = new[] { "ID", "Nombre", "Días", "Precio" } }
            };

            int pw = (_content.Width - GymTheme.Padding * 2 - 48) / 2;
            var bll = new GymApp.BLL.CatalogosBLL();

            // 3. CONSTRUCCIÓN EN CICLO
            foreach (var cat in cats)
            {
                var p = new GymPanel
                {
                    Width = pw,
                    Height = 220,
                    Location = new Point(xPos, yPos)
                };

                var lbl = new Label
                {
                    Text = cat.Titulo,
                    Font = GymTheme.FontBold,
                    ForeColor = GymTheme.Accent,
                    Location = new Point(14, 12),
                    AutoSize = true,
                    BackColor = Color.Transparent
                };

                var grid = new DataGridView
                {
                    Location = new Point(8, 40),
                    Size = new Size(p.Width - 16, 130),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    ReadOnly = true,
                    AllowUserToAddRows = false
                };
                GymGrid.ApplyStyle(grid);

                // ========================================================
                // MAGIA: Llenar el Grid con datos de SQL
                // ========================================================
                try
                {
                    DataTable dt = bll.ObtenerDatosCatalogo(cat.Tipo);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        grid.DataSource = dt; // Llenado automático con los datos reales
                    }
                    else
                    {
                        // Si está vacío, solo dibujamos la cabecera
                        foreach (var c in cat.Cols) grid.Columns.Add(c.Replace(" ", "_"), c);
                        grid.Rows.Add("---", "Sin registros...");
                    }
                }
                catch
                {
                    foreach (var c in cat.Cols) grid.Columns.Add(c, c);
                    grid.Rows.Add("ERR", "Error de conexión");
                }



                // ========================================================
                // ENRUTADOR DE CLICS (Abre el Formulario Correcto)
                // ========================================================
                var btnAdd = new GymButton { Text = "➕ Agregar", Style = GymButton.ButtonStyle.Primary, Width = 110, Height = 34, Location = new Point(14, 176) };

                // ====== NUEVOS BOTONES ======
                var btnEdit = new GymButton { Text = "✏️ Editar", Style = GymButton.ButtonStyle.Secondary, Width = 100, Height = 34, Location = new Point(130, 176) };
                var btnDelete = new GymButton { Text = "🗑️ Eliminar", Style = GymButton.ButtonStyle.Danger, Width = 110, Height = 34, Location = new Point(236, 176) };

                // ====== EVENTO AGREGAR ======
                btnAdd.Click += (s, e) =>
                {
                    DialogResult res = (cat.Tipo == "Visita" || cat.Tipo == "Membresia") ?
                                       new FrmCatMembresias(cat.Tipo).ShowDialog() :
                                       new FrmCatalogoGenerico(cat.Tipo).ShowDialog();
                    if (res == DialogResult.OK) LoadCategorias();
                };

                // ====== EVENTO EDITAR ======
                btnEdit.Click += (s, e) =>
                {
                    if (grid.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una fila primero.", "Aviso"); return; }

                    int id = Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value);
                    string desc = grid.SelectedRows[0].Cells[1].Value.ToString();
                    DialogResult res;

                    if (cat.Tipo == "Visita" || cat.Tipo == "Membresia")
                    {
                        // Limpiamos el formato de moneda ($) para que C# pueda leer el número
                        decimal precio = Convert.ToDecimal(grid.SelectedRows[0].Cells["Precio"].Value.ToString().Replace("$", ""));
                        int dias = cat.Tipo == "Membresia" ? Convert.ToInt32(grid.SelectedRows[0].Cells["Dias"].Value) : 0;
                        res = new FrmCatMembresias(cat.Tipo, id, desc, precio, dias).ShowDialog();
                    }
                    else
                    {
                        res = new FrmCatalogoGenerico(cat.Tipo, id, desc).ShowDialog();
                    }

                    if (res == DialogResult.OK) LoadCategorias();
                };

                // ====== EVENTO ELIMINAR ======
                btnDelete.Click += (s, e) =>
                {
                    if (grid.SelectedRows.Count == 0) { MessageBox.Show("Seleccione una fila primero.", "Aviso"); return; }

                    string nombre = grid.SelectedRows[0].Cells[1].Value.ToString();
                    if (MessageBox.Show($"¿Eliminar '{nombre}' de {cat.Titulo}?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            bll.EliminarCatalogo(cat.Tipo, Convert.ToInt32(grid.SelectedRows[0].Cells["ID"].Value));
                            LoadCategorias(); // Recargar visualmente
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message, "Error"); }
                    }
                };

                p.Controls.Add(lbl);
                p.Controls.Add(grid);
                p.Controls.Add(btnAdd);
                p.Controls.Add(btnEdit);   // <--- Agregamos al panel
                p.Controls.Add(btnDelete); // <--- Agregamos al panel
                _content.Controls.Add(p);

                // Lógica de Coordenadas: Acomoda en 2 columnas y luego baja de fila
                xPos += pw + 16;
                if (xPos >= (pw + 16) * 2)
                {
                    xPos = 0;       // Regresa al margen izquierdo
                    yPos += 236;    // Baja a la siguiente fila
                }
            }
        }
        private void LoadComingSoon(string page)
        {
            _lblPageSub.Text = "Módulo en construcción";
            var p = new GymPanel
            {
                Location = new Point((_content.Width - 400) / 2, 80),
                Size = new Size(400, 200)
            };
            var lbl = new Label
            {
                Text = $"🚧\n\n{page}\nMódulo próximamente",
                Font = GymTheme.FontSubtitle,
                ForeColor = GymTheme.TextSecondary,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            p.Controls.Add(lbl);
            _content.Controls.Add(p);
        }

        /* private void LoadMembresiasPantalla()
         {
             try
             {
                 // 1. Instanciamos la capa lógica y traemos el DataTable puro desde la base de datos
                 var bll = new GymApp.BLL.MembresiaBLL();
                 DataTable dtMembresias = bll.ConsultarMembresiasClientes(""); // Cadena vacía para traer todos al inicio

                 _lblPageSub.Text = "Monitoreo de membresías vigentes y estados de acceso";

                 // 2. Definimos las columnas visuales
                 string[] columnas = new[] { "Cliente ID", "Cliente", "Membresía", "Fecha Inicio", "Vencimiento", "Días Restantes", "Estado" };

                 // 3. Mapeamos las filas del DataTable a la lista de objetos de tu generador de Grid
                 var filasGrid = new List<object[]>();

                 foreach (DataRow row in dtMembresias.Rows)
                 {
                     filasGrid.Add(new object[]
                     {
                         row["fiCliente"].ToString(),
                         row["Cliente"].ToString(),
                         row["TipoMembresia"].ToString(),
                         Convert.ToDateTime(row["FechaInicio"]).ToString("dd/MM/yyyy"),
                         Convert.ToDateTime(row["FechaVencimiento"]).ToString("dd/MM/yyyy"),
                         row["DiasRestantes"].ToString(),
                         row["Estatus"].ToString()
                     });
                 }

                 // 4. Invocamos tu grandioso método genérico
                 LoadGenericCrud(
                     "Membresías",
                     "Monitoreo de membresías vigentes",
                     columnas,
                     filasGrid.ToArray(),
                     null,
                    // (sender, e) => { MessageBox.Show("Asignar nueva membresía manualmente en desarrollo"); },
                     //(sender, e) => { MessageBox.Show("Modificar vigencia en desarrollo"); },
                     //(sender, e) => { MessageBox.Show("Cancelar membresía en desarrollo"); }
                // );

                 (sender, e) =>
                 {
                     if (_currentGrid.SelectedRows.Count > 0)
                     {
                         int idCliente = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                         string nombreCliente = _currentGrid.SelectedRows[0].Cells[1].Value.ToString();

                         // Abrimos la caja registradora con el cliente precargado
                         FrmVenta frmVenta = new FrmVenta(idCliente, nombreCliente);
                         if (frmVenta.ShowDialog() == DialogResult.OK)
                         {
                             LoadMembresiasPantalla(); // Recarga y verás cómo el semáforo cambia a verde
                         }
                     }
                     else
                     {
                         MessageBox.Show("Por favor seleccione un cliente de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                     }
                 },

             null, // Ocultar botón Eliminar
             null  // Ocultar botón Abonar
         );

                 foreach (Control c in _content.Controls)
                 {
                     if (c is Panel toolbar) // La barra de herramientas
                     {
                         foreach (Control btn in toolbar.Controls)
                         {
                             // El botón de "Editar" es el que está en la posición 128, 4 según tu método LoadGenericCrud
                             if (btn is Button b && b.Location.X == 128)
                             {
                                 b.Text = "🔄  Renovar Membresía";
                                 b.BackColor = Color.LimeGreen; // ¡El verde que querías!
                                 b.ForeColor = Color.Black;     // Texto negro para que resalte en el verde
                                 b.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Bold);
                                 b.Width = 180;                 // Un poco más ancho
                                 b.FlatStyle = FlatStyle.Flat;
                                 b.FlatAppearance.BorderSize = 0;
                             }
                         }
                     }
                 }

                 // 5. Opcional visual: Pintar la columna de Días Restantes y Estatus según si ya venció
                 foreach (DataGridViewRow r in _currentGrid.Rows)
                 {
                     int dias = Convert.ToInt32(r.Cells["Días_Restantes"].Value);
                     if (dias <= 0 || r.Cells["Estado"].Value.ToString() == "Vencida")
                     {
                         r.Cells["Días_Restantes"].Style.ForeColor = Color.FromArgb(255, 69, 0); // Rojo/Naranja
                         r.Cells["Estado"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                     }
                     else if (dias <= 5)
                     {
                         r.Cells["Días_Restantes"].Style.ForeColor = Color.Yellow; // Alerta de por vencer
                     }
                     else
                     {
                         r.Cells["Estado"].Style.ForeColor = Color.LimeGreen; // Verde Activa
                     }
                 }
             }
             catch (Exception ex)
             {
                 MessageBox.Show("Error al cargar la pantalla de membresías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
             }
         }
         */

        private void LoadMembresiasPantalla()
        {
            try
            {
                var bll = new GymApp.BLL.MembresiaBLL();
                DataTable dtMembresias = bll.ConsultarMembresiasClientes("");

                _lblPageSub.Text = "Monitoreo de membresías vigentes y estados de acceso";

                string[] columnas = new[] { "Cliente ID", "Cliente", "Membresía", "Fecha Inicio", "Vencimiento", "Días Restantes", "Estado" };
                var filasGrid = new List<object[]>();

                foreach (DataRow row in dtMembresias.Rows)
                {
                    filasGrid.Add(new object[]
                    {
                row["fiCliente"].ToString(),
                row["Cliente"].ToString(),
                row["TipoMembresia"].ToString(),
                Convert.ToDateTime(row["FechaInicio"]).ToString("dd/MM/yyyy"),
                Convert.ToDateTime(row["FechaVencimiento"]).ToString("dd/MM/yyyy"),
                row["DiasRestantes"].ToString(),
                row["Estatus"].ToString()
                    });
                }

                LoadGenericCrud(
                    "Membresías",
                    "Monitoreo de membresías vigentes",
                    columnas,
                    filasGrid.ToArray(),

                    // 1. EVENTO NUEVO (Lo usaremos para EDITAR FECHA DE MEMBRESÍA)
                    (sender, e) =>
                    {
                        // Validación de Rol de Administrador
                        if (GymApp.Core.SesionGlobal.NombrePuesto != "Administrador")
                        {
                            MessageBox.Show("Acceso denegado. Solo los administradores pueden modificar fechas de membresía de forma manual.", "Seguridad", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (_currentGrid.SelectedRows.Count > 0)
                        {
                            int idCliente = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                            string nombreCliente = _currentGrid.SelectedRows[0].Cells[1].Value.ToString();
                            DateTime fechaActual = Convert.ToDateTime(_currentGrid.SelectedRows[0].Cells[4].Value);

                            FrmActualizaFechaMembresia frm = new FrmActualizaFechaMembresia(idCliente, nombreCliente, fechaActual);
                            if (frm.ShowDialog() == DialogResult.OK)
                            {
                                LoadMembresiasPantalla(); // Recargamos para ver la nueva fecha
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor seleccione un cliente de la lista dando clic en el margen izquierdo.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    },

                    // 2. EVENTO EDITAR (Lo usas para RENOVAR MEMBRESÍA)
                    (sender, e) =>
                    {
                        if (_currentGrid.SelectedRows.Count > 0)
                        {
                            int idCliente = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                            string nombreCliente = _currentGrid.SelectedRows[0].Cells[1].Value.ToString();

                            FrmVenta frmVenta = new FrmVenta(idCliente, nombreCliente);
                            if (frmVenta.ShowDialog() == DialogResult.OK)
                            {
                                LoadMembresiasPantalla();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Por favor seleccione un cliente de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    },

                    null, // Ocultar botón Eliminar
                    null  // Ocultar botón Abonar
                );

                // CONFIGURACIÓN VISUAL DE LOS BOTONES
                foreach (Control c in _content.Controls)
                {
                    if (c is Panel toolbar)
                    {
                        foreach (Control btn in toolbar.Controls)
                        {
                            if (btn is Button b)
                            {
                                if (b.Location.X == 128) // Botón Editar original (ahora Renovar)
                                {
                                    b.Text = "🔄  Renovar Membresía";
                                    b.BackColor = Color.LimeGreen;
                                    b.ForeColor = Color.Black;
                                    b.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Bold);
                                    b.Width = 180;
                                    b.FlatStyle = FlatStyle.Flat;
                                    b.FlatAppearance.BorderSize = 0;
                                }
                                else if (b.Location.X == 0) // Botón Nuevo original (ahora Editar Fecha)
                                {
                                    b.Text = "✏️  Editar Fecha";
                                    b.BackColor = Color.FromArgb(255, 193, 7); // Color ámbar/dorado de advertencia
                                    b.ForeColor = Color.Black;
                                    b.Font = new System.Drawing.Font("Segoe UI", 9f, FontStyle.Bold);
                                    b.Width = 125;
                                    b.FlatStyle = FlatStyle.Flat;
                                    b.FlatAppearance.BorderSize = 0;
                                    b.Visible = true; // Forzamos visibilidad
                                }
                            }
                        }
                    }
                }

                // COLORES DE DÍAS RESTANTES
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    int dias = Convert.ToInt32(r.Cells["Días_Restantes"].Value);
                    if (dias <= 0 || r.Cells["Estado"].Value.ToString() == "Vencida")
                    {
                        r.Cells["Días_Restantes"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                        r.Cells["Estado"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                    }
                    else if (dias <= 5)
                    {
                        r.Cells["Días_Restantes"].Style.ForeColor = Color.Yellow;
                    }
                    else
                    {
                        r.Cells["Estado"].Style.ForeColor = Color.LimeGreen;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la pantalla de membresías: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadCobranza()
        {
            try
            {
                var bll = new GymApp.BLL.AbonosBLL();
                DateTime desde = DateTime.Now.AddDays(-30); // Carga los últimos 30 días por defecto
                DateTime hasta = DateTime.Now;
                var dt = bll.ConsultarDeudas("", desde, hasta);
                var filasGrid = new List<object[]>();

                decimal totalCarteraVencida = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal restante = Convert.ToDecimal(row["SaldoPendiente"]);
                    totalCarteraVencida += restante;

                    filasGrid.Add(new object[]
                    {
                row["IDDeuda"],
                row["FolioTicket"],
                row["Cliente"],
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy"),
                Convert.ToDecimal(row["TotalDeuda"]).ToString("$#,##0.00"),
                Convert.ToDecimal(row["Abonado"]).ToString("$#,##0.00"),
                restante.ToString("$#,##0.00") // Esta celda podrías pintarla roja
                    });
                }

                string subtitulo = $"Cartera Vencida Total: {totalCarteraVencida:C2}";

                LoadGenericCrud("Cuentas por Cobrar", subtitulo,
                                new[] { "ID Deuda", "Ticket", "Cliente", "Fecha", "Deuda Original", "Abonado", "Saldo Pendiente" },
                                filasGrid.ToArray(),

                                    null, // 1. Botón NUEVO desactivado
                                    null, // 2. Botón EDITAR desactivado (¡Aquí faltaba este null!)
                                    null, // 3. Botón ELIMINAR desactivado

                                    // 4. EVENTO ABONAR (Ahora sí cae en el botón verde de la posición 540)
                                    (sender, e) =>
                                    {
                                        if (_currentGrid != null && _currentGrid.SelectedRows.Count > 0)
                                        {
                                            int idDeuda = Convert.ToInt32(_currentGrid.SelectedRows[0].Cells[0].Value);
                                            string nombreCliente = _currentGrid.SelectedRows[0].Cells[2].Value.ToString();

                                            // Limpiamos el formato de moneda para pasarlo como decimal
                                            string strRestante = _currentGrid.SelectedRows[0].Cells[6].Value.ToString().Replace("$", "").Replace(",", "");
                                            decimal pendiente = Convert.ToDecimal(strRestante);

                                            FrmAbonos frm = new FrmAbonos(idDeuda, nombreCliente, pendiente);
                                            if (frm.ShowDialog() == DialogResult.OK)
                                            {
                                                LoadCobranza(); // Recargamos para ver cómo baja la deuda
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Por favor, selecciona una deuda del listado.", "Aviso");
                                        }
                                    }
                                );

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar la cobranza: " + ex.Message);
            }
        }
        private void LoadCompras()
        {
            try
            {
                var bll = new GymApp.BLL.InventarioBLL();

                // Leemos las fechas de los DatePickers de tu menú superior
                DateTime desde = _dtpDesde?.Value ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;

                string filtroBusqueda = _currentSearchBox != null && _currentSearchBox.Text != "🔍  Buscar..." ? _currentSearchBox.Text.Trim() : "";

                // Llamamos a la base de datos (Crearemos este método en el Paso 4)
                DataTable dtCompras = bll.ConsultarCompras(filtroBusqueda, desde, hasta);

                var filasGrid = new List<object[]>();
                decimal totalCompras = 0;

                foreach (DataRow row in dtCompras.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["Total"]);
                    totalCompras += monto;

                    filasGrid.Add(new object[]
                    {
                "#" + row["Folio"].ToString(),
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                monto.ToString("$#,##0.00"),
                row["Empleado"].ToString()
                    });
                }

                string subtitulo = $"Compras del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | GASTO TOTAL: {totalCompras.ToString("$#,##0.00")}";

                LoadGenericCrud(
                    "Historial de Compras",
                    subtitulo,
                    new[] { "Folio / Factura", "Fecha", "Monto Total", "Registró" },
                    filasGrid.ToArray(),

                    // Evento "NUEVO" -> Abre tu formulario de compras
                    (sender, e) =>
                    {
                        FrmCompras frm = new FrmCompras();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadCompras(); // Recarga la tabla si la compra se guardó
                        }
                    },
                    null, // Ocultar botón Editar
                    null  // Ocultar botón Eliminar (Las compras no se borran por auditoría)
                );

                // Pintamos los montos de naranja para indicar que es dinero que salió del negocio
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    r.Cells["Monto_Total"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar las compras: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCancelaciones()
        {
            try
            {
                var bll = new GymApp.BLL.DevolucionesBLL();

                DateTime desde = _dtpDesde?.Value ?? DateTime.Now.AddDays(-30).Date;
                DateTime hasta = _dtpHasta?.Value ?? DateTime.Now.Date;
                string filtro = _currentSearchBox != null && _currentSearchBox.Text != "🔍  Buscar..." ? _currentSearchBox.Text.Trim() : "";

                DataTable dt = bll.ConsultarDevoluciones(filtro, desde, hasta);

                var filasGrid = new List<object[]>();
                decimal totalDevuelto = 0;

                foreach (DataRow row in dt.Rows)
                {
                    decimal monto = Convert.ToDecimal(row["Monto"]);
                    totalDevuelto += monto;

                    filasGrid.Add(new object[]
                    {
                "DEV-" + row["Folio"].ToString(),
                "TKT-" + row["VentaOrigen"].ToString(),
                Convert.ToDateTime(row["Fecha"]).ToString("dd/MM/yyyy HH:mm"),
                row["Autorizo"].ToString(),
                monto.ToString("$#,##0.00"),
                row["Motivo"].ToString()
                    });
                }

                string subtitulo = $"Devoluciones del {desde:dd/MM/yyyy} al {hasta:dd/MM/yyyy} | SALIDAS TOTALES: {totalDevuelto.ToString("$#,##0.00")}";

                LoadGenericCrud(
                    "Historial de Cancelaciones",
                    subtitulo,
                    new[] { "Folio Dev.", "Ticket Origen", "Fecha", "Autorizó", "Monto Devuelto", "Motivo" },
                    filasGrid.ToArray(),

                    (sender, e) => // Botón Nuevo (Manda llamar al FrmDevolucion)
                    {
                        FrmDevolucion frm = new FrmDevolucion();
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            LoadCancelaciones();
                        }
                    },
                    null, // Sin Editar
                    null  // Sin Eliminar (Historial auditable no se borra)
                );

                // Pintamos el monto de rojo/naranja porque es dinero que salió
                foreach (DataGridViewRow r in _currentGrid.Rows)
                {
                    r.Cells["Monto_Devuelto"].Style.ForeColor = Color.FromArgb(255, 69, 0);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar devoluciones: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AbrirChecadorEnPanel()
        {
            // 1. Limpiamos el panel central (_content es el nombre de tu panel de contenido)
            _content.Controls.Clear();

            // 2. Instanciamos el formulario
            FrmChecador frm = new FrmChecador();

            // 3. Lo configuramos para que sea un hijo del panel
            frm.TopLevel = false;
            frm.Dock = DockStyle.Fill;

            // 4. Lo agregamos al panel principal _content
            _content.Controls.Add(frm);
            frm.Show();
        }

        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            // Asignamos los valores de la sesión activa a las etiquetas de la pantalla



            // Al abrir el menú, mandamos llamar los datos de la RAM global
            /*       MessageBox.Show($"¡Sesión iniciada con éxito!\n\n" +
                                   $"Empleado: {GymApp.Core.SesionGlobal.NombreCompleto}\n" +
                                   $"Puesto: {GymApp.Core.SesionGlobal.NombrePuesto}\n" +
                                   $"ID: {GymApp.Core.SesionGlobal.IdEmpleado}",
                                   "Verificación de Memoria",
                                   MessageBoxButtons.OK,
                                   MessageBoxIcon.Information);*/

        }
    }
}




