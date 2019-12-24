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
    public partial class FormFinancial : Form
    {
        public FormFinancial()
        {
            InitializeComponent();
            Button1_Click(null, null);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void GenerateNewData()
        {
            Random rand = new Random();

            int pointCount = 100;
            ScottPlot.OHLC[] ohlcs = ScottPlot.DataGen.RandomStockPrices(rand, pointCount);
            double[] timestamps = ScottPlot.DataGen.Consecutive(pointCount);
            double[] volumes = ScottPlot.DataGen.Random(rand, pointCount, 500, 1000);

            formsPlot1.plt.Clear();
            formsPlot1.plt.YLabel("Share Price", fontSize: 10);
            formsPlot1.plt.Title("ScottPlot Candlestick Demo");
            if (rbCandle.Checked)
                formsPlot1.plt.PlotCandlestick(ohlcs);
            else
                formsPlot1.plt.PlotOHLC(ohlcs);
            formsPlot1.plt.AxisAuto();

            formsPlot2.plt.Clear();
            formsPlot2.plt.YLabel("Volume", fontSize: 10);
            formsPlot2.plt.PlotBar(timestamps, volumes, barWidth: .5);
            formsPlot2.plt.AxisAuto(.01, .1);
            formsPlot2.plt.Axis(null, null, 0, null);

            formsPlot1.plt.MatchPadding(formsPlot2.plt, horizontal: true, vertical: false);
            formsPlot1.Render();
            formsPlot2.Render();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            GenerateNewData();
        }

        private void RbCandle_CheckedChanged(object sender, EventArgs e)
        {
            GenerateNewData();
        }

        private void RbOHLC_CheckedChanged(object sender, EventArgs e)
        {
            GenerateNewData();
        }

        private void ScottPlotUC1_MouseDragged(object sender, EventArgs e)
        {
            formsPlot2.plt.MatchAxis(formsPlot1.plt, horizontal: true, vertical: false);
            formsPlot2.Render();
        }

        private void ScottPlotUC2_MouseDragged(object sender, EventArgs e)
        {
            formsPlot1.plt.MatchAxis(formsPlot2.plt, horizontal: true, vertical: false);
            formsPlot1.Render();
        }
    }
}
