using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VectorGraphicsEditor.Models;

namespace VectorGraphicsEditor.State
{
    public sealed class Scene
    {
        public List<IShape> Shapes { get; } = new List<IShape>();


        private IShape? _selectedShape;

        public IShape? SelectedShape
        {
            get => _selectedShape;
            set
            {
                if (ReferenceEquals(_selectedShape, value)) return;
                _selectedShape = value;
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler? SelectionChanged;



        private bool _antiAliasingEnabled;

        public bool AntiAliasingEnabled
        {
            get => _antiAliasingEnabled;
            set
            {
                if (_antiAliasingEnabled == value) return;
                _antiAliasingEnabled = value;
                RenderSettingsChanged?.Invoke(this, EventArgs.Empty);
            }
        }   

        public event EventHandler? RenderSettingsChanged;


        public void AddShape(IShape shape, bool select = true)
        {
            Shapes.Add(shape);
            if(select) SelectedShape = shape;
            ShapesChanged?.Invoke(this, EventArgs.Empty);
        }
        public void RemoveShape(IShape shape)
        {
            if (!Shapes.Remove(shape)) return;
            if (ReferenceEquals(SelectedShape, shape)) SelectedShape = null;
            ShapesChanged?.Invoke(this, System.EventArgs.Empty);
        }
        public void ClearAll()
        {
            Shapes.Clear();
            SelectedShape = null;
            ShapesChanged?.Invoke(this, System.EventArgs.Empty);
        }

        public void ReplaceAll(IEnumerable<IShape> shapes)
        {
            Shapes.Clear();
            Shapes.AddRange(shapes);
            SelectedShape = null;
            ShapesChanged?.Invoke(this, System.EventArgs.Empty);
        }

        public event EventHandler? ShapesChanged;

    }
}
