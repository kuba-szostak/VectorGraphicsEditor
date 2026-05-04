using System.Drawing;
using System.Windows.Forms;
using VectorGraphicsEditor.Models;
using VectorGraphicsEditor.State;
using RasterEditor.Rendering;

namespace VectorGraphicsEditor
{
    public partial class MainForm : Form
    {
        private readonly Scene _scene;
        private readonly Rasterizer _rasterizer;
        private readonly FileManager _fileManager;

        private Tool _currentTool = Tool.Select;
        private Point? _startPoint;
        private IShape? _tempShape;
        private PolygonShape? _activePolygon;

        private enum Tool
        {
            Select,
            Line,
            Circle,
            Polygon
        }

        public MainForm()
        {
            InitializeComponent();

            _scene = new Scene();
            _fileManager = new FileManager();
            _rasterizer = new Rasterizer(_scene, pictureBox1.Width, pictureBox1.Height);

            SetupEvents();
            UpdateToolSelection();
        }

        private void SetupEvents()
        {
            pictureBox1.MouseDown += OnMouseDown;
            pictureBox1.MouseMove += OnMouseMove;
            pictureBox1.MouseUp += OnMouseUp;
            pictureBox1.SizeChanged += (s, e) => {
                if (pictureBox1.Width > 0 && pictureBox1.Height > 0)
                {
                    _rasterizer.Resize(pictureBox1.Width, pictureBox1.Height);
                    Redraw();
                }
            };

            btnSelect.Click += (s, e) => SetTool(Tool.Select);
            btnLine.Click += (s, e) => SetTool(Tool.Line);
            btnCircle.Click += (s, e) => SetTool(Tool.Circle);
            btnPolygon.Click += (s, e) => SetTool(Tool.Polygon);

            btnColor.Click += OnColorClick;
            txtThickness.TextChanged += OnThicknessChanged;
            btnAntiAlias.CheckedChanged += (s, e) => {
                _scene.AntiAliasingEnabled = btnAntiAlias.Checked;
                Redraw();
            };

            loadMenuItem.Click += OnLoadClick;
            saveMenuItem.Click += OnSaveClick;
            clearMenuItem.Click += (s, e) => {
                _scene.ClearAll();
                _activePolygon = null;
                Redraw();
            };

            this.FormClosed += (s, e) => _rasterizer.DirectBitmap.Dispose();

            _scene.ShapesChanged += (s, e) => Redraw();
            _scene.SelectionChanged += (s, e) => Redraw();
            _scene.RenderSettingsChanged += (s, e) => Redraw();
        }

        private void SetTool(Tool tool)
        {
            _currentTool = tool;
            _activePolygon = null;
            UpdateToolSelection();
        }

        private void UpdateToolSelection()
        {
            btnSelect.Checked = _currentTool == Tool.Select;
            btnLine.Checked = _currentTool == Tool.Line;
            btnCircle.Checked = _currentTool == Tool.Circle;
            btnPolygon.Checked = _currentTool == Tool.Polygon;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (_currentTool == Tool.Select)
            {
                var hit = _scene.Shapes.LastOrDefault(s => s.HitTest(e.X, e.Y));
                _scene.SelectedShape = hit;
                return;
            }

            if (_currentTool == Tool.Polygon)
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (_activePolygon == null)
                    {
                        _activePolygon = new PolygonShape { 
                            BaseColor = btnColor.BackColor, 
                            Thickness = GetThickness() 
                        };
                        _activePolygon.Vertices.Add(e.Location);
                        _activePolygon.Vertices.Add(e.Location); // Placeholder for move
                        _scene.AddShape(_activePolygon, false);
                    }
                    else
                    {
                        _activePolygon.Vertices.Add(e.Location);
                    }
                }
                else if (e.Button == MouseButtons.Right && _activePolygon != null)
                {
                    if (_activePolygon.Vertices.Count > 2)
                    {
                        _activePolygon.Vertices.RemoveAt(_activePolygon.Vertices.Count - 1); // Remove placeholder
                        _activePolygon.IsClosed = true;
                    }
                    _activePolygon = null;
                    Redraw();
                }
                return;
            }

            _startPoint = e.Location;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_currentTool == Tool.Polygon && _activePolygon != null)
            {
                _activePolygon.Vertices[_activePolygon.Vertices.Count - 1] = e.Location;
                Redraw();
                return;
            }

            if (_startPoint == null) return;

            switch (_currentTool)
            {
                case Tool.Line:
                    _tempShape = new LineShape { 
                        StartPoint = _startPoint.Value, 
                        EndPoint = e.Location,
                        BaseColor = btnColor.BackColor,
                        Thickness = GetThickness()
                    };
                    break;
                case Tool.Circle:
                    int radius = (int)Math.Sqrt(Math.Pow(e.X - _startPoint.Value.X, 2) + Math.Pow(e.Y - _startPoint.Value.Y, 2));
                    _tempShape = new CircleShape { 
                        Center = _startPoint.Value, 
                        Radius = radius,
                        BaseColor = btnColor.BackColor,
                        Thickness = GetThickness()
                    };
                    break;
            }
            Redraw();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (_startPoint == null) return;

            if (_tempShape != null)
            {
                _scene.AddShape(_tempShape);
                _tempShape = null;
            }
            _startPoint = null;
            Redraw();
        }

        private void Redraw()
        {
            var bitmap = _rasterizer.Render();
            
            if (_tempShape != null)
            {
                switch (_tempShape)
                {
                    case LineShape line: _rasterizer.DrawLine(line); break;
                    case CircleShape circle: _rasterizer.DrawCircle(circle); break;
                    case PolygonShape poly: _rasterizer.DrawPolygon(poly); break;
                }
            }

            if (pictureBox1.Image != bitmap)
            {
                pictureBox1.Image = bitmap;
            }
            else
            {
                pictureBox1.Invalidate();
            }
        }

        private int GetThickness()
        {
            if (int.TryParse(txtThickness.Text, out int t)) return t;
            return 1;
        }

        private void OnColorClick(object sender, EventArgs e)
        {
            using (var cd = new ColorDialog())
            {
                cd.Color = btnColor.BackColor;
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    btnColor.BackColor = cd.Color;
                    btnColor.ForeColor = cd.Color.GetBrightness() > 0.5f ? Color.Black : Color.White;
                    if (_scene.SelectedShape != null)
                    {
                        _scene.SelectedShape.BaseColor = cd.Color;
                        Redraw();
                    }
                }
            }
        }

        private void OnThicknessChanged(object sender, EventArgs e)
        {
            if (_scene.SelectedShape != null)
            {
                _scene.SelectedShape.Thickness = GetThickness();
                Redraw();
            }
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "JSON files (*.json)|*.json";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    _fileManager.Save(_scene, sfd.FileName);
                }
            }
        }

        private void OnLoadClick(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "JSON files (*.json)|*.json";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _fileManager.Load(_scene, ofd.FileName);
                    Redraw();
                }
            }
        }
    }
}
