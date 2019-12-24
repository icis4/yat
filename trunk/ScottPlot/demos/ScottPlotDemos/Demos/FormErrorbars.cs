﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScottPlotDemos
{
    public partial class FormErrorbars : Form
    {
        public FormErrorbars()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            formsPlot1.plt.Title("Demo - error lines");
            formsPlot1.plt.YLabel("Average");
            formsPlot1.Render();

            Button1_Click(null, null);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Random rand = new Random();
            int pointCount = 20;
            double[] xs = ScottPlot.DataGen.Consecutive(pointCount);
            double[] yMeans = ScottPlot.DataGen.RandomWalk(rand, pointCount, 100, rand.NextDouble()*5);
            double[] yErrs = ScottPlot.DataGen.Random(rand, pointCount, 20);
            double[] xErrs = ScottPlot.DataGen.Random(rand, pointCount, 1);

            formsPlot1.plt.Grid(false);
            double lineWidth = (cbConnect.Checked) ? 1 : 0;
            formsPlot1.plt.PlotScatter(xs, yMeans, lineWidth: lineWidth, errorY: yErrs, errorX: xErrs);
            formsPlot1.plt.AxisAuto();
            formsPlot1.Render();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            formsPlot1.plt.Clear();
            formsPlot1.Render();
        }
    }
}
