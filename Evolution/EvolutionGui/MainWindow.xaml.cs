using SharpGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EvolutionGui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenGLControl_OpenGLDraw(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {
            var gl = args.OpenGL;
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            gl.Translate(-0.0f, 0.0f, -1.0f);

            //  Start drawing triangles.
            gl.Begin(OpenGL.GL_TRIANGLE_FAN);

            var n = 60;

            var angle_increment = Math.PI * 2 / n;

            for (var theta = 0.0; theta < Math.PI * 2; theta += angle_increment)
            {
                var x = (60 / 2 * Math.Cos(theta)) / 1000;
                var y = (60 / 2 * Math.Sin(theta)) / 1000;

                //  Color the vertices!
                gl.Color(1.0f, 0.0f, 0.0f);
                gl.Vertex(x, y);
            }

            gl.End();

            //  Flush OpenGL.
            gl.Flush();
        }
    }
}
