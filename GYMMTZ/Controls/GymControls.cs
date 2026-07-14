using GYMMTZ.Theme;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;


namespace GYMMTZ.Controls
{
    // ─── BOTÓN REDONDEADO CON GRADIENTE ───────────────────────────────────────
    public class GymButton : Button
    {
        private Color _baseColor = Color.Transparent;
        private bool _isHovered;
        private bool _isPressed;
        public enum ButtonStyle { Primary, Secondary, Danger, Success, Ghost }
        private ButtonStyle _style;

        public ButtonStyle Style
        {
            get => _style;
            set { _style = value; UpdateColors(); Invalidate(); }
        }

        public GymButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            Cursor = Cursors.Hand;
            Font = GymTheme.FontBold;
            ForeColor = GymTheme.TextPrimary;
            Height = 40;
            UpdateColors();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void UpdateColors()
        {
            switch (_style)
            {
                case ButtonStyle.Primary: _baseColor = GymTheme.Accent; break;
                case ButtonStyle.Secondary: _baseColor = GymTheme.SurfaceElevated; break;
                case ButtonStyle.Danger: _baseColor = GymTheme.Danger; break;
                case ButtonStyle.Success: _baseColor = GymTheme.Success; break;
                case ButtonStyle.Ghost: _baseColor = Color.Transparent; break;
            }
        }

        protected override void OnMouseEnter(EventArgs e) { _isHovered = true; Invalidate(); base.OnMouseEnter(e); }
        protected override void OnMouseLeave(EventArgs e) { _isHovered = false; Invalidate(); base.OnMouseLeave(e); }
        protected override void OnMouseDown(MouseEventArgs e) { _isPressed = true; Invalidate(); base.OnMouseDown(e); }
        protected override void OnMouseUp(MouseEventArgs e) { _isPressed = false; Invalidate(); base.OnMouseUp(e); }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            var rect = new Rectangle(0, 0, Width - 1, Height - 1);
            var path = RoundedRect(rect, 8);

            Color c = _baseColor;
            if (_isPressed) c = ControlPaint.Dark(c, 0.2f);
            else if (_isHovered) c = ControlPaint.Light(c, 0.1f);

            if (_style == ButtonStyle.Ghost)
            {
                // CORRECCIÓN APLICADA AQUÍ: Se agregaron paréntesis y llaves
                using (var pen = new Pen(GymTheme.Border, 1.5f))
                {
                    g.DrawPath(pen, path);
                }
            }
            else
            {
                // CORRECCIÓN APLICADA AQUÍ: Se agregaron paréntesis y llaves
                using (var brush = new LinearGradientBrush(rect,
                    _isPressed ? c : ControlPaint.Light(c, 0.08f), c, 90f))
                {
                    g.FillPath(brush, path);
                }

                // Brillo superior
                if (!_isPressed)
                {
                    var shimRect = new Rectangle(2, 1, Width - 4, Height / 2);
                    var shimPath = RoundedRect(shimRect, 6);

                    // CORRECCIÓN APLICADA AQUÍ: Se agregaron paréntesis y llaves
                    using (var shimBrush = new LinearGradientBrush(shimRect,
                        Color.FromArgb(40, 255, 255, 255), Color.Transparent, 90f))
                    {
                        g.FillPath(shimBrush, shimPath);
                    }
                }
            }

            // Texto
            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            // CORRECCIÓN APLICADA AQUÍ: Envolvimos el SolidBrush en un using tradicional para evitar fugas de memoria
            using (var textBrush = new SolidBrush(ForeColor))
            {
                g.DrawString(Text, Font, textBrush, rect, sf);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            var path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    // ─── PANEL REDONDEADO CON BORDE ──────────────────────────────────────────
    public class GymPanel : Panel
    {
        public int CornerRadius { get; set; } = 10;
        public Color BorderColor { get; set; } = GymTheme.Border;
        public bool ShowAccentBorder { get; set; } = false;

        public GymPanel()
        {
            BackColor = GymTheme.SurfaceElevated;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(1, 1, Width - 2, Height - 2);
            var path = GetRoundedPath(rect, CornerRadius);

            // CORRECCIÓN APLICADA AQUÍ
            using (var brush = new SolidBrush(BackColor))
            {
                g.FillPath(brush, path);
            }

            if (ShowAccentBorder)
            {
                // Línea naranja izquierda
                // CORRECCIÓN APLICADA AQUÍ
                using (var accentPen = new Pen(GymTheme.Accent, 3f))
                {
                    g.DrawLine(accentPen, 1, CornerRadius, 1, Height - CornerRadius);
                }
            }

            // CORRECCIÓN APLICADA AQUÍ
            using (var borderPen = new Pen(ShowAccentBorder ? GymTheme.Border : BorderColor, 1f))
            {
                g.DrawPath(borderPen, path);
            }

            base.OnPaint(e);
        }

        private GraphicsPath GetRoundedPath(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, rad * 2, rad * 2, 180, 90);
            p.AddArc(r.Right - rad * 2, r.Y, rad * 2, rad * 2, 270, 90);
            p.AddArc(r.Right - rad * 2, r.Bottom - rad * 2, rad * 2, rad * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - rad * 2, rad * 2, rad * 2, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    // ─── TEXTBOX ESTILIZADO ──────────────────────────────────────────────────
    public class GymTextBox : UserControl
    {
        private TextBox _inner;
        private Label _lblLabel;
        private bool _isFocused;
        public string LabelText { get => _lblLabel.Text; set => _lblLabel.Text = value; }
        public char PasswordChar
        {
            get { return _inner.PasswordChar; }
            set { _inner.PasswordChar = value; }
        }
        public new string Text { get => _inner.Text; set => _inner.Text = value; }
        public bool UsePasswordChar { get => _inner.UseSystemPasswordChar; set => _inner.UseSystemPasswordChar = value; }
        public new event EventHandler TextChanged { add => _inner.TextChanged += value; remove => _inner.TextChanged -= value; }

        
        public GymTextBox()
        {
            Height = 64;
            BackColor = Color.Transparent;
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint | ControlStyles.ResizeRedraw, true);

            _lblLabel = new Label
            {
                Font = GymTheme.FontSmall,
                ForeColor = GymTheme.TextSecondary,
                AutoSize = true,
                Location = new Point(12, 6),
                BackColor = Color.Transparent
            };

            _inner = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = GymTheme.SurfaceElevated,
                ForeColor = GymTheme.TextPrimary,
                Font = GymTheme.FontBody,
                Location = new Point(12, 32),
                Height = 22
            };

            _inner.Enter += (s, e) => { _isFocused = true; Invalidate(); };
            _inner.Leave += (s, e) => { _isFocused = false; Invalidate(); };

            Controls.Add(_lblLabel);
            Controls.Add(_inner);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);

            // Agregamos esta validación de seguridad
            if (_inner != null)
            {
                _inner.Width = Width - 24;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var rect = new Rectangle(0, 18, Width - 1, Height - 20);
            var path = RoundedRect(rect, 8);

            // CORRECCIÓN APLICADA AQUÍ
            using (var bg = new SolidBrush(GymTheme.SurfaceElevated))
            {
                g.FillPath(bg, path);
            }

            var borderColor = _isFocused ? GymTheme.Accent : GymTheme.Border;

            // CORRECCIÓN APLICADA AQUÍ
            using (var pen = new Pen(borderColor, _isFocused ? 2f : 1f))
            {
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath RoundedRect(Rectangle r, int rad)
        {
            var p = new GraphicsPath();
            p.AddArc(r.X, r.Y, rad * 2, rad * 2, 180, 90);
            p.AddArc(r.Right - rad * 2, r.Y, rad * 2, rad * 2, 270, 90);
            p.AddArc(r.Right - rad * 2, r.Bottom - rad * 2, rad * 2, rad * 2, 0, 90);
            p.AddArc(r.X, r.Bottom - rad * 2, rad * 2, rad * 2, 90, 90);
            p.CloseFigure();
            return p;
        }
    }

    // ─── DATAGRIDVIEW ESTILIZADO ─────────────────────────────────────────────
    public static class GymGrid
    {
        public static void ApplyStyle(DataGridView grid)
        {
            grid.BackgroundColor = GymTheme.Surface;
            grid.GridColor = GymTheme.Border;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.Font = GymTheme.FontBody;
            grid.ForeColor = GymTheme.TextPrimary;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Columnas header
            grid.ColumnHeadersDefaultCellStyle.BackColor = GymTheme.Background;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = GymTheme.TextSecondary;
            grid.ColumnHeadersDefaultCellStyle.Font = GymTheme.FontBold;
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = GymTheme.Background;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 0, 0);
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 40;
            grid.EnableHeadersVisualStyles = false;

            // Filas alternas
            grid.DefaultCellStyle.BackColor = GymTheme.Surface;
            grid.DefaultCellStyle.ForeColor = GymTheme.TextPrimary;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 69, 0, 40);
            grid.DefaultCellStyle.SelectionForeColor = GymTheme.TextPrimary;
            grid.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);
            grid.DefaultCellStyle.WrapMode = DataGridViewTriState.False;

            grid.AlternatingRowsDefaultCellStyle.BackColor = GymTheme.SurfaceElevated;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = GymTheme.TextPrimary;
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(255, 69, 0, 40);

            grid.RowTemplate.Height = 38;
        }
    }
}