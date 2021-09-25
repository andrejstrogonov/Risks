namespace Graphic
{
    partial class Graphic
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chrtGraphic = new System.Windows.Forms.DataVisualization.Charting.Chart();
            ((System.ComponentModel.ISupportInitialize)(this.chrtGraphic)).BeginInit();
            this.SuspendLayout();
            // 
            // chrtGraphic
            // 
            chartArea1.Name = "ChartArea1";
            this.chrtGraphic.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.chrtGraphic.Legends.Add(legend1);
            this.chrtGraphic.Location = new System.Drawing.Point(2, 2);
            this.chrtGraphic.Name = "chrtGraphic";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chrtGraphic.Series.Add(series1);
            this.chrtGraphic.Size = new System.Drawing.Size(926, 582);
            this.chrtGraphic.TabIndex = 0;
            // 
            // Graphic
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1156, 583);
            this.Controls.Add(this.chrtGraphic);
            this.Name = "Graphic";
            this.Text = "График";
            ((System.ComponentModel.ISupportInitialize)(this.chrtGraphic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataVisualization.Charting.Chart chrtGraphic;
    }
}

