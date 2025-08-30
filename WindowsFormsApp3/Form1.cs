using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        // Espessura da linha (pode ser alterada conforme desejado)
        private float espessuraLinha = 2f;
        private float sx = 1f;  // escala em X
        private float sy = 1f;  // escala em Y

        // Cor escolhida pelo usuário (default = branco)
        private Color corSelecionada = Color.White;

        // Cores de cada face
        private Color[] coresFaces = new Color[10];

        public Form1()
        {
            InitializeComponent();
            this.Paint += Form1_Paint;

            // trackBars via código 
            trackBar1.Minimum = -360; // rotação
            trackBar1.Maximum = 360;
            trackBar1.Value = 0;

            trackBar2.Minimum = -700; // translação vertical
            trackBar2.Maximum = 700;
            trackBar2.Value = 0;

            trackBar3.Minimum = -700; // translação horizontal
            trackBar3.Maximum = 700;
            trackBar3.Value = 0;

            // adicionar opções no comboBox1
            comboBox1.Items.AddRange(new string[] {
                "Vermelho", "Verde", "Azul", "Amarelo", "Roxo",
                "Laranja", "Rosa", "Ciano", "Marrom", "Preto", "Branco"
            });
            comboBox1.SelectedIndex = 10; // Branco como padrão

            // preencher comboBox2 com as faces
            for (int i = 0; i < 10; i++)
            {
                comboBox2.Items.Add("Face " + (i + 1));
                coresFaces[i] = corSelecionada; // inicializa cada face com branco
            }
            comboBox2.SelectedIndex = 0;
        }

        private void trackBar1_Scroll(object sender, EventArgs e) => this.Invalidate();
        private void trackBar2_Scroll(object sender, EventArgs e) => this.Invalidate();
        private void trackBar3_Scroll(object sender, EventArgs e) => this.Invalidate();
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            sx = trackBar4.Value / 10f;
            if (sx <= 0) sx = 0.1f;
            sy = sx; // manter proporção X e Y igual
            this.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private void trackBar3_ValueChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }

        public Color iniciarCor(int r, int g, int b)
        {
            return Color.FromArgb(255, r, g, b);
        }

        public Pen criarCaneta(Color cor, float espessura)
        {
            return new Pen(cor, espessura);
        }

        public void desenharPoligono(Graphics g, int[] x, int[] y, Color corLinha, float espessuraLinha, Color corHachura, Color corFundo)
        {
            Point[] pontos = new Point[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                pontos[i] = new Point(x[i], y[i]);
            }

            using (SolidBrush brush = new SolidBrush(corFundo))
            {
                g.FillPolygon(brush, pontos);
            }
            using (Pen caneta = criarCaneta(corLinha, espessuraLinha))
            {
                g.DrawPolygon(caneta, pontos);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            // 1. Vértices do hexágono
            int[] vx = { 300, 400, 400, 300, 200, 200, 300 };
            int[] vy = { 100, 150, 250, 300, 250, 150, 100 };

            // 2. Vértices internos (T0,T1,T2)
            int[] tx = { 300, 350, 250 };
            int[] ty = { 150, 225, 225 };

            // 3. Triângulos (10 faces)
            int[][] triangulosX = new int[][]
            {
                new int[]{tx[0], tx[1], tx[2]},
                new int[]{vx[0], vx[1], tx[0]},
                new int[]{vx[0], tx[0], vx[5]},
                new int[]{vx[1], vx[2], tx[1]},
                new int[]{vx[1], tx[0], tx[1]},
                new int[]{vx[5], vx[4], tx[2]},
                new int[]{vx[5], tx[0], tx[2]},
                new int[]{vx[2], vx[3], tx[1]},
                new int[]{vx[3], vx[4], tx[2]},
                new int[]{vx[3], tx[1], tx[2]}
            };

            int[][] triangulosY = new int[][]
            {
                new int[]{ty[0], ty[1], ty[2]},
                new int[]{vy[0], vy[1], ty[0]},
                new int[]{vy[0], ty[0], vy[5]},
                new int[]{vy[1], vy[2], ty[1]},
                new int[]{vy[1], ty[0], ty[1]},
                new int[]{vy[5], vy[4], ty[2]},
                new int[]{vy[5], ty[0], ty[2]},
                new int[]{vy[2], vy[3], ty[1]},
                new int[]{vy[3], vy[4], ty[2]},
                new int[]{vy[3], ty[1], ty[2]}
            };

            
            // Rotação + Translação X e Y + Escala
          
            int angulo = (this.Controls.ContainsKey("trackBar1") && this.trackBar1 != null) ? trackBar1.Value : 0;
            double rad = angulo * Math.PI / 180.0;
            double s = Math.Sin(rad);
            double c = Math.Cos(rad);

            int deslocamentoY = (this.Controls.ContainsKey("trackBar2") && this.trackBar2 != null) ? trackBar2.Value : 0;
            int deslocamentoX = (this.Controls.ContainsKey("trackBar3") && this.trackBar3 != null) ? trackBar3.Value : 0;

            int cx = 300;
            int cy = 200;

            // Transformar vértices externos com escala
            int[] vx2 = new int[vx.Length];
            int[] vy2 = new int[vy.Length];
            for (int i = 0; i < vx.Length; i++)
            {
                double dx = vx[i] - cx;
                double dy = vy[i] - cy;
                int xr = (int)(cx + (dx * c - dy * s) * sx) + deslocamentoX;
                int yr = (int)(cy + (dx * s + dy * c) * sy) + deslocamentoY;
                vx2[i] = xr;
                vy2[i] = yr;
            }

            // Transformar vértices internos com escala
            int[] tx2 = new int[tx.Length];
            int[] ty2 = new int[ty.Length];
            for (int i = 0; i < tx.Length; i++)
            {
                double dx = tx[i] - cx;
                double dy = ty[i] - cy;
                int xr = (int)(cx + (dx * c - dy * s) * sx) + deslocamentoX;
                int yr = (int)(cy + (dx * s + dy * c) * sy) + deslocamentoY;
                tx2[i] = xr;
                ty2[i] = yr;
            }

            // atualizar triângulos
            int[][] triangulosX2 = new int[][]
            {
                new int[]{tx2[0], tx2[1], tx2[2]},
                new int[]{vx2[0], vx2[1], tx2[0]},
                new int[]{vx2[0], tx2[0], vx2[5]},
                new int[]{vx2[1], vx2[2], tx2[1]},
                new int[]{vx2[1], tx2[0], tx2[1]},
                new int[]{vx2[5], vx2[4], tx2[2]},
                new int[]{vx2[5], tx2[0], tx2[2]},
                new int[]{vx2[2], vx2[3], tx2[1]},
                new int[]{vx2[3], vx2[4], tx2[2]},
                new int[]{vx2[3], tx2[1], tx2[2]}
            };

            int[][] triangulosY2 = new int[][]
            {
                new int[]{ty2[0], ty2[1], ty2[2]},
                new int[]{vy2[0], vy2[1], ty2[0]},
                new int[]{vy2[0], ty2[0], vy2[5]},
                new int[]{vy2[1], vy2[2], ty2[1]},
                new int[]{vy2[1], ty2[0], ty2[1]},
                new int[]{vy2[5], vy2[4], ty2[2]},
                new int[]{vy2[5], ty2[0], ty2[2]},
                new int[]{vy2[2], vy2[3], ty2[1]},
                new int[]{vy2[3], vy2[4], ty2[2]},
                new int[]{vy2[3], ty2[1], ty2[2]}
            };

            vx = vx2; vy = vy2;
            tx = tx2; ty = ty2;
            triangulosX = triangulosX2;
            triangulosY = triangulosY2;

            // desenha triângulos
            Color corLinha = iniciarCor(0, 0, 0);
            Color corHachura = iniciarCor(0, 150, 255);

            for (int i = 0; i < triangulosX.Length; i++)
            {
                desenharPoligono(
                    g,
                    triangulosX[i],
                    triangulosY[i],
                    corLinha,
                    espessuraLinha,
                    corHachura,
                    coresFaces[i] // usa cor individual de cada face
                );
            }

            // desenhar contorno do hexágono
            Point[] hexPoints = new Point[vx.Length];
            for (int i = 0; i < vx.Length; i++)
                hexPoints[i] = new Point(vx[i], vy[i]);
            using (Pen penAresta = criarCaneta(corLinha, 2f))
            {
                g.DrawPolygon(penAresta, hexPoints);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "Vermelho": corSelecionada = iniciarCor(255, 0, 0); break;
                case "Verde": corSelecionada = iniciarCor(0, 255, 0); break;
                case "Azul": corSelecionada = iniciarCor(0, 0, 255); break;
                case "Amarelo": corSelecionada = iniciarCor(255, 255, 0); break;
                case "Roxo": corSelecionada = iniciarCor(128, 0, 128); break;
                case "Laranja": corSelecionada = iniciarCor(255, 165, 0); break;
                case "Rosa": corSelecionada = iniciarCor(255, 192, 203); break;
                case "Ciano": corSelecionada = iniciarCor(0, 255, 255); break;
                case "Marrom": corSelecionada = iniciarCor(139, 69, 19); break;
                case "Preto": corSelecionada = iniciarCor(0, 0, 0); break;
                case "Branco": corSelecionada = iniciarCor(255, 255, 255); break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == null) return;
            int faceIndex = comboBox2.SelectedIndex;
            coresFaces[faceIndex] = corSelecionada;
            this.Invalidate(); // redesenha com a nova cor
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e) { }
    }
}
