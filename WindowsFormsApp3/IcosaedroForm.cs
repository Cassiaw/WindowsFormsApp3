using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class IcosaedroForm : Form
    {
        // Parâmetros de transformação
        private float translateX = 0;
        private float translateY = 0;
        private int rotation = 0; // 0, 1, 2, 3 (multiplo de 90 graus)
        private float scale = 1.0f;     
        private Color corFigura = Color.DodgerBlue; // Inicial

        // NumericUpDown para Espessura
        private NumericUpDown numericEspessura;

        // Vértices da figura do enunciado (Hexágono + topo + base)
        private PointF[] vertices;

        // Triângulos conforme solicitado
        private int[][] triangles = new int[][]
        {
            new int[]{7, 8, 9},   // centro

            new int[]{0, 1, 7},   // topo-direita
            new int[]{0, 7, 5},   // topo-esquerda

            new int[]{1, 2, 8},   // lado direito
            new int[]{1, 7, 8},   // ligação topo-direita

            new int[]{5, 4, 9},   // lado esquerdo
            new int[]{5, 7, 9},   // ligação topo-esquerda

            new int[]{2, 3, 8},   // baixo-direita
            new int[]{3, 4, 9},   // baixo-esquerda
            new int[]{3, 8, 9}    // base central
        };

        // Arestas para desenho da figura exatamente igual à fornecida
        private int[][] edges = new int[][]
        {
            new int[]{0,1}, new int[]{1,2}, new int[]{2,3}, new int[]{3,4}, new int[]{4,5}, new int[]{5,0},
            new int[]{0,7}, new int[]{1,7}, new int[]{2,8}, new int[]{3,8}, new int[]{3,9}, new int[]{4,9}, new int[]{5,9}, new int[]{7,8}, new int[]{7,9}, new int[]{8,9}
        };

        public IcosaedroForm()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 700;
            this.Height = 740;
            this.Text = "Icosaedro 2D - Computação Gráfica";

            // TrackBars para translação, rotação, escala
            var trX = new TrackBar() { Minimum = -200, Maximum = 200, Value = 0, Left = 20, Top = 20, Width = 150, TickFrequency = 20 };
            var trY = new TrackBar() { Minimum = -200, Maximum = 200, Value = 0, Left = 190, Top = 20, Width = 150, TickFrequency = 20 };
            var trScale = new TrackBar() { Minimum = 40, Maximum = 200, Value = 100, Left = 360, Top = 20, Width = 120, TickFrequency = 20 };
            var trRot = new TrackBar() { Minimum = 0, Maximum = 3, Value = 0, Left = 500, Top = 20, Width = 120, TickFrequency = 1 };

            var lblX = new Label() { Left = 20, Top = 50, Text = "Translação X" };
            var lblY = new Label() { Left = 190, Top = 50, Text = "Translação Y" };
            var lblS = new Label() { Left = 360, Top = 50, Text = "Escala" };
            var lblR = new Label() { Left = 500, Top = 50, Text = "Rotação (90°)" };

            // NumericUpDown para espessura
            numericEspessura = new NumericUpDown() { Minimum = 1, Maximum = 5, Value = 2, Left = 630, Top = 20, Width = 50 };
            var lblEsp = new Label() { Left = 630, Top = 50, Text = "Espessura" };

            // Botão para escolher cor
            var btnCor = new Button() { Left = 550, Top = 80, Width = 100, Height = 30, Text = "Cor da figura" };
            btnCor.Click += BtnCor_Click;

            this.Controls.Add(trX); this.Controls.Add(lblX);
            this.Controls.Add(trY); this.Controls.Add(lblY);
            this.Controls.Add(trScale); this.Controls.Add(lblS);
            this.Controls.Add(trRot); this.Controls.Add(lblR);
            this.Controls.Add(numericEspessura); this.Controls.Add(lblEsp);
            this.Controls.Add(btnCor);

            trX.Scroll += (s, e) => { translateX = trX.Value; Invalidate(); };
            trY.Scroll += (s, e) => { translateY = trY.Value; Invalidate(); };
            trScale.Scroll += (s, e) => { scale = trScale.Value / 100f; Invalidate(); };
            trRot.Scroll += (s, e) => { rotation = trRot.Value; Invalidate(); };
            numericEspessura.ValueChanged += (s, e) => { Invalidate(); };

            DefinirVertices();
        }

        private void InitializeComponent()
        {
          
        }

        private void DefinirVertices()
        {
            // Centro da tela e escala para caber no formulário
            float raio = 250;
            float cx = this.ClientSize.Width / 2;
            float cy = this.ClientSize.Height / 2 + 40;

            vertices = new PointF[10];
            // Vértices principais
            vertices[0] = new PointF(cx + 0 * raio, cy - 1.0f * raio);         // V0 (0, 1.0)      topo
            vertices[1] = new PointF(cx + 0.866f * raio, cy - 0.5f * raio);    // V1 (0.866, 0.5)  topo-direita
            vertices[2] = new PointF(cx + 0.866f * raio, cy + 0.5f * raio);    // V2 (0.866,-0.5)  baixo-direita
            vertices[3] = new PointF(cx + 0 * raio, cy + 1.0f * raio);         // V3 (0,-1.0)      fundo
            vertices[4] = new PointF(cx - 0.866f * raio, cy + 0.5f * raio);    // V4 (-0.866,-0.5) baixo-esquerda
            vertices[5] = new PointF(cx - 0.866f * raio, cy - 0.5f * raio);    // V5 (-0.866,0.5)  topo-esquerda
            // Pontos internos
            vertices[7] = new PointF(cx + 0 * raio, cy - 0.5f * raio);         // T0 (0,0.5)       meio superior
            vertices[8] = new PointF(cx + 0.433f * raio, cy + 0.25f * raio);   // T1 (0.433,-0.25) interior direita
            vertices[9] = new PointF(cx - 0.433f * raio, cy + 0.25f * raio);   // T2 (-0.433,-0.25)interior esquerda
            // O índice 6 não é usado, mas pode ser inicializado para evitar erros
            vertices[6] = new PointF(cx, cy); // Não utilizado nos triângulos
        }

        private PointF Transform(PointF p)
        {
            float cx = this.ClientSize.Width / 2;
            float cy = this.ClientSize.Height / 2 + 40;
            float x = p.X - cx;
            float y = p.Y - cy;
            for (int i = 0; i < rotation; i++)
            {
                float tmp = x;
                x = -y;
                y = tmp;
            }
            x *= scale;
            y *= scale;
            x += cx + translateX;
            y += cy + translateY;
            return new PointF(x, y);
        }

        private void BtnCor_Click(object sender, EventArgs e)
        {
            using (ColorDialog dlg = new ColorDialog())
            {
                dlg.Color = corFigura;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    corFigura = dlg.Color;
                    Invalidate();
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            this.BackColor = Color.White;

            int esp = (int)numericEspessura.Value;

            // Use triangles no lugar de faces
            for (int i = 0; i < triangles.Length; i++)
            {
                PointF[] pts = new PointF[3];
                for (int j = 0; j < 3; j++)
                    pts[j] = Transform(vertices[triangles[i][j]]);
                g.FillPolygon(new SolidBrush(Color.FromArgb(120, corFigura)), pts);
            }

            using (Pen pen = new Pen(corFigura, esp))
            {
                foreach (var edge in edges)
                {
                    PointF p1 = Transform(vertices[edge[0]]);
                    PointF p2 = Transform(vertices[edge[1]]);
                    g.DrawLine(pen, p1, p2);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var frm = new IcosaedroForm();
            frm.Show();
        }
    }
}