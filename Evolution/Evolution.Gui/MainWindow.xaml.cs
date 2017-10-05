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

namespace Evolution.Gui
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

            gl.Color(255, 0, 0);
            this.drawOval(gl, 0, 0, 30, 30, 60);

            gl.End();

            gl.Flush();
        }

        private void DrawCircle(OpenGL gl)
        {
            var sides = 60;
            var radius = 300;

            gl.LoadIdentity();
            gl.Begin(OpenGL.GL_LINE_LOOP);

            for (int a = 0; a < 360; a += 360 / sides)
            {
                var radians = this.DegreesToRadians(a);
                gl.Vertex(Math.Cos(radians) * radius, Math.Sin(radians) * radius, 0);
            }

            gl.End();
        }

        private void drawOval(OpenGL gl, double x_center, double y_center, double w, double h, int n)
        {
            gl.LoadIdentity();

            if (n <= 0)
                n = 1;
            var angle_increment = Math.PI * 2 / n;
            gl.PushMatrix();

            gl.Translate(x_center, y_center, 0);
            gl.Begin(OpenGL.GL_TRIANGLE_FAN);

            gl.Vertex(0, 0);

            for (var theta = 0.0; theta - Math.PI * 2 < 0.001; theta += angle_increment)
            {
                var x = w / 2 * Math.Cos(theta);
                var y = h / 2 * Math.Sin(theta);

                //  Color the vertices!
                gl.Color(100, 0, 0);
                gl.Vertex(x, y);
            }

            gl.End();
            gl.PopMatrix();
        }

        private double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180) * degrees;
        }

        private void OpenGLControl_OpenGLInitialized(object sender, SharpGL.SceneGraph.OpenGLEventArgs args)
        {

        }
    }
}
