using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.Utils;
using DevExpress.XtraCharts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentChart : ComponentBase
    {
        private Series series;

        private string serieName = "";
        private List<string> serieList = new List<string>();
        private List<object> XList = new List<object>();
        private List<object> YList = new List<object>();
        private string SingleOrMultiInd = "";

        private ChartControl chartControl;
        private XYDiagram diagram
        { get { return chartControl.Diagram as XYDiagram; } }
        //XYDiagram xyDiagram { get; set; }
        //Diagram3D diagram3d { get; set; }
        //XYDiagram3D xyDiagram3d { get; set; }

        private new JChart definition;

        public ComponentChart() : base()
        {
        }

        public ComponentChart(Control control, object def) : base(control, def)
        {
            definition = def as JChart;

            if (!(control is ChartControl))
                return;

            chartControl = (control as ChartControl);
            Init();
        }

        private void Init()
        {
            try
            {
                if (!string.IsNullOrEmpty(definition.viewType))
                {
                    DataTable chartDT = GetData().Tables[0];
                    SingleOrMultiInd = chartDT.Columns.Count == 3 ? "multi" : "single"; // 2 kolone - single, 3 kolone - multi

                    setChartValueLists(serieList, XList, YList, chartDT);
                    createChart(chartDT);
                    setControlProperties();
                }
                else
                    form.Logger.Warn("ChartControl", "Nedefiniran atribut 'viewtype'");
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - Init()", ex.Message);
            }
        }

        private void setChartValueLists(List<string> serieList, List<object> XList, List<object> YList, DataTable chartDT)
        {
            try
            {
                foreach (DataRow row in chartDT.Rows)
                {
                    XList.Add(SingleOrMultiInd == "multi" ? row[1] : row[0]);
                    YList.Add(SingleOrMultiInd == "multi" ? row[2] : row[1]);
                    serieList.Add(SingleOrMultiInd == "multi" ? row[0].ToString() : "");
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - setChartValueLists()", ex.Message);
            }
        }

        private void createChart(DataTable chartDT)
        {
            try
            {
                foreach (var serie in serieList.Distinct())
                {
                    createViewType();

                    createSeriesPoints(chartDT, serie);

                    // labele na podacima iz serije (na barovima npr.)
                    setLabels();

                    showAreaMarkers(series);
                    chartControl.Series.Add(series);
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - createChart()", ex.Message);
            }
        }

        private void setDateTimeMeasureUnit()
        {
            if (string.IsNullOrEmpty(definition.MeasureUnit))
            {
                diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Month;
                diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Month;
                return;
            }

            switch (definition.MeasureUnit.ToLower())
            {
                case "year":
                    diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Year;
                    diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Year;
                    break;

                case "month":
                    diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Month;
                    diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Month;
                    break;

                case "day":
                    diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
                    diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Day;
                    break;

                default:
                    diagram.AxisX.DateTimeScaleOptions.MeasureUnit = DateTimeMeasureUnit.Day;
                    diagram.AxisX.DateTimeScaleOptions.GridAlignment = DateTimeGridAlignment.Day;
                    break;
            };
        }

        private void setControlProperties()
        {
            try
            {
                if (!string.IsNullOrEmpty(definition.LegendVisible) && definition.LegendVisible.ToLower() == "true")
                    chartControl.Legend.Visibility = DefaultBoolean.True;
                else
                    chartControl.Legend.Visibility = DefaultBoolean.False;

                //if (!string.IsNullOrEmpty(definition.LegendVisible) && definition.LegendVisible.ToLower() == "true"
                //    && !string.IsNullOrEmpty(definition.LegendCheckboxes) && definition.LegendCheckboxes.ToLower() == "true")
                //    chartControl.Legend.UseCheckBoxes = true;

                setAxisOptions();

                if (!string.IsNullOrEmpty(definition.Caption))
                {
                    ChartTitle chartCaption = new ChartTitle();
                    chartCaption.Text = definition.Caption;
                    chartCaption.WordWrap = true;
                    chartCaption.MaxLineCount = 2;

                    SetStyles(chartCaption);

                    //= Common.Helpers.Colors.HtmlToColor(this.definition.FontColor, baseEdit.Properties.Appearance.ForeColor);
                    //chartCaption.Indent = 10;

                    chartControl.Titles.Add(chartCaption);
                }

                //if (definition.viewType.ToLower() == "line")
                //{
                //    series.ArgumentScaleType = ScaleType.Numerical;
                //    ((SplineSeriesView)series.View).LineTensionPercent = 90;
                //}

                setValueAsPercent();
                // TODO: tooltips
                // TODO: labels - position
                // TODO: checkboxes in legend

                //if (!string.IsNullOrEmpty(definition.LegendCheckboxes) && definition.LegendCheckboxes.ToLower() == "true")
                //chartControl.Legend.UseCheckBoxes = true;

                //chartControl.Titles.Add(new ChartTitle());
                //chartControl.Titles[0].Text = !string.IsNullOrEmpty(definition.Caption) ? definition.Caption : "";
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - setControlProperties()", ex.Message);
            }
        }

        private void createViewType()
        {
            try
            {
                switch (definition.viewType.ToLower())
                {
                    case "bar":
                        series = new Series(serieName, ViewType.Bar);
                        break;

                    case "stackedbar":
                        series = new Series(serieName, ViewType.StackedBar);
                        break;

                    case "pie":
                        series = new Series(serieName, ViewType.Pie);
                        break;

                    case "doughnut":
                        series = new Series(serieName, ViewType.Doughnut);
                        break;

                    case "point":
                        series = new Series(serieName, ViewType.Point);
                        break;

                    case "line":
                        series = new Series(serieName, ViewType.Line);
                        break;

                    case "stepline":
                        series = new Series(serieName, ViewType.StepLine);
                        break;

                    case "spline":
                        series = new Series(serieName, ViewType.Spline);
                        break;

                    case "scatterline":
                        series = new Series(serieName, ViewType.ScatterLine);
                        break;

                    case "area":
                        series = new Series(serieName, ViewType.Area);
                        break;

                    case "steparea":
                        series = new Series(serieName, ViewType.StepArea);
                        break;

                    case "splinearea":
                        series = new Series(serieName, ViewType.SplineArea);
                        break;

                    case "stackedarea":
                        series = new Series(serieName, ViewType.StackedArea);
                        break;

                    case "stackedsplinearea":
                        series = new Series(serieName, ViewType.StackedSplineArea);
                        break;

                    case "bar3d":
                        series = new Series(serieName, ViewType.Bar3D);
                        break;

                    case "stackedbar3d":
                        series = new Series(serieName, ViewType.StackedBar3D);
                        break;

                    case "pie3d":
                        series = new Series(serieName, ViewType.Pie3D);
                        break;

                    case "doughnut3d":
                        series = new Series(serieName, ViewType.Doughnut3D);
                        break;

                    default:
                        break;
                };
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ComponentChart - createSeries()", ex.Message);
            }
        }

        //private void setSeriesTitles(Series series)
        //{
        //}

        private void createSeriesPoints(DataTable chartDT, string serie)
        {
            try
            {
                for (int i = 0; i < chartDT.Rows.Count; i++)
                {
                    if (SingleOrMultiInd == "multi")
                    {
                        if (chartDT.Rows[i][0].ToString() == serie)
                            series.Points.Add(new SeriesPoint(XList[i], YList[i]));
                    }
                    else
                        series.Points.Add(new SeriesPoint(XList[i], YList[i]));
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - createSeriesPoints()", ex.Message);
            }
        }

        private void setValueAsPercent()    // samo na pie i doughnut
        {
            try
            {
                if (definition.viewType.ToLower().Contains("pie") || definition.viewType.ToLower().Contains("doughnut"))
                {
                    if (!string.IsNullOrEmpty(definition.ValueAsPercent) && definition.ValueAsPercent.ToLower() == "true")
                    {
                        foreach (Series serie in chartControl.Series)
                        {
                            serie.Label.TextPattern = "{VP:P0}";
                        }
                    }
                    else
                    {
                        foreach (Series serie in chartControl.Series)
                        {
                            serie.Label.TextPattern = "{V:F2}";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - setValueAsPercent()", ex.Message);
            }
        }

        private void setLabels()
        {
            if (!string.IsNullOrEmpty(definition.LabelVisible) && definition.LabelVisible.ToLower() == "true")
                series.LabelsVisibility = DefaultBoolean.True;
            else
                series.LabelsVisibility = DefaultBoolean.False;
        }

        private void showAreaMarkers(Series serie)
        {
            try
            {
                if (definition.viewType.ToLower().Contains("area"))
                {
                    if (!string.IsNullOrEmpty(definition.ShowAreaMarkers) && definition.ShowAreaMarkers.ToLower() == "true")
                    {
                        switch (definition.viewType.ToLower())
                        {
                            case "area":
                                (serie.View as AreaSeriesView).MarkerVisibility = DefaultBoolean.True;
                                (serie.View as AreaSeriesView).MarkerOptions.Kind = MarkerKind.Circle;
                                (serie.View as AreaSeriesView).MarkerOptions.Size = 8;
                                break;

                            case "steparea":
                                (serie.View as StepAreaSeriesView).MarkerVisibility = DefaultBoolean.True;
                                (serie.View as StepAreaSeriesView).MarkerOptions.Kind = MarkerKind.Circle;
                                (serie.View as StepAreaSeriesView).MarkerOptions.Size = 8;
                                break;

                            case "splinearea":
                                (serie.View as SplineAreaSeriesView).MarkerVisibility = DefaultBoolean.True;
                                (serie.View as SplineAreaSeriesView).MarkerOptions.Kind = MarkerKind.Circle;
                                (serie.View as SplineAreaSeriesView).MarkerOptions.Size = 8;
                                break;

                            default:
                                break;
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - setControlProperties()", ex.Message);
            }
        }

        private void SetStyles(ChartTitle chartCaption)
        {
            try
            {
                //chartCaption.Font. = new System.Drawing.Font("Tahoma", 14, System.Drawing.FontStyle.Bold);

                var checkIfResult = true;

                if (definition.Styles != null && definition.Styles.StyleList.Count > 0)
                {
                    foreach (JStyle style in definition.Styles.StyleList)
                    {
                        if (Conditions.CheckCondition(ref checkIfResult, style, form, null, false) == false)
                            continue;

                        switch (style.Name.ToLower())
                        {
                            case "captioncolor":
                                chartCaption.TextColor = Common.Helpers.Colors.HtmlToColor(style.Value, chartCaption.TextColor);
                                break;

                            case "captionposition":
                                {
                                    switch (style.Value.ToLower())
                                    {
                                        case "left":
                                            chartCaption.Dock = ChartTitleDockStyle.Left;
                                            break;

                                        case "right":
                                            chartCaption.Dock = ChartTitleDockStyle.Right;
                                            break;

                                        case "top":
                                            chartCaption.Dock = ChartTitleDockStyle.Top;
                                            break;

                                        case "bottom":
                                            chartCaption.Dock = ChartTitleDockStyle.Bottom;
                                            break;

                                        default:
                                            break;
                                    };
                                }
                                break;

                            default:
                                break;
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ComponentChart() - SetStyles()", ex.Message);
            }
        }

        private void setAxisOptions()
        {
            try
            {
                if (!definition.viewType.Contains("pie") && !definition.viewType.Contains("doughnut"))
                {
                    if (!definition.viewType.Contains("3d"))
                    {
                        setAxisLabelFormats();
                        EnableScrollingAndZooming();
                    }

                    if (XList[0].GetType() == typeof(DateTime))
                    {
                        setAxisXdatetimeProperties();
                        diagram.AxisX.DateTimeScaleOptions.GridSpacing = definition.GridSpacing;
                    }
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ComponentChart() - setAxisOptions()", ex.Message);
            }
        }

        private void setAxisLabelFormats()
        {
            try
            {
                if (!string.IsNullOrEmpty(definition.Xformat))
                {
                    if (XList[0].GetType() == typeof(DateTime))
                        diagram.AxisX.Label.TextPattern = "{A:" + definition.Xformat + "}";
                    else if (XList[0].GetType() == typeof(int) || XList[0].GetType() == typeof(double) || XList[0].GetType() == typeof(decimal)
                        || XList[0].GetType() == typeof(float) || XList[0].GetType() == typeof(float))
                        diagram.AxisX.Label.TextPattern = "{V:" + definition.Xformat + "}";
                }
                else
                {
                    if (XList[0].GetType() == typeof(DateTime))
                        diagram.AxisX.Label.TextPattern = "{A:dd.MM.yyyy}";
                    else if (XList[0].GetType() == typeof(int) || XList[0].GetType() == typeof(double) || XList[0].GetType() == typeof(decimal)
                        || XList[0].GetType() == typeof(float) || XList[0].GetType() == typeof(float))
                        diagram.AxisX.Label.TextPattern = "{V:N2}";
                }

                if (!string.IsNullOrEmpty(definition.Yformat))
                {
                    if (YList[0].GetType() == typeof(DateTime))
                        diagram.AxisY.Label.TextPattern = "{A:" + definition.Yformat + "}";
                    else if (YList[0].GetType() == typeof(int) || YList[0].GetType() == typeof(double) || YList[0].GetType() == typeof(decimal)
                        || YList[0].GetType() == typeof(float) || YList[0].GetType() == typeof(float))
                        diagram.AxisY.Label.TextPattern = "{V:" + definition.Yformat + "}";
                }
                else
                {
                    if (YList[0].GetType() == typeof(DateTime))
                        diagram.AxisY.Label.TextPattern = "{A:dd.MM.yyyy}";
                    else if (YList[0].GetType() == typeof(int) || YList[0].GetType() == typeof(double) || YList[0].GetType() == typeof(decimal)
                        || YList[0].GetType() == typeof(float) || YList[0].GetType() == typeof(float))
                        diagram.AxisY.Label.TextPattern = "{V:N2}";
                }
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ComponentChart() - setAxisLabelFormats()", ex.Message);
            }
        }

        private void EnableScrollingAndZooming()
        {
            diagram.EnableAxisXScrolling = true;
            diagram.EnableAxisYScrolling = true;

            diagram.EnableAxisXZooming = true;
            diagram.EnableAxisYZooming = true;
        }

        private void setAxisXdatetimeProperties()
        {
            try
            {
                diagram.AxisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Manual;
                diagram.AxisX.DateTimeScaleOptions.AutoGrid = false;

                setDateTimeMeasureUnit();
            }
            catch (Exception ex)
            {
                form.Logger.Warn("ComponentChart() - setAxisXdatetimeProperties()", ex.Message);
            }
        }

        private DataSet GetData()
        {
            try
            {
                DataSet dataSet = new DataSet();

                if (!string.IsNullOrEmpty(definition.DataName))
                {
                    bool stopActions = false;
                    Logic.Data.DataDefinition dataDefinition = new Logic.Data.DataDefinition(form, definition);
                    return dataDefinition.GetDataSet("getChartData", out stopActions);
                }
                else
                {
                    form.Logger.Warn("ComponentChart - GetData()" + control.Name, @"atribut 'DataName nije defeniran'");
                    return null;
                }
            }
            catch (Exception ex)
            {
                var controlName = !string.IsNullOrEmpty(definition.Name) ? definition.Name : "";
                form.Logger.Warn("ChartControl: " + controlName + " - GetData()", ex.Message);
                return null;
            }
        }

        public void SetEnabled(bool enabled)
        {
            chartControl.Enabled = enabled;
        }
    }
}