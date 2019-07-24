package com.nz.spark.chartrenderer;

public class View {
    private Chart[] chartList = new Chart[2];

    public void AddChart(Chart chart)
    {
        //chartList[0] always stores the first chart, and chartList[1] stores the second chart.
        if(chartList[0] == null) {
            chartList[0] = chart;
            return;
        }

        if(chartList[1] == null) {
            chartList[1] = chart;
            return;
        }

        //if new chart is adding and there were 2 charts stored, the oldest one will be discarded.
        chartList[0] = chartList[1];
        chartList[1] = chart;
    }

    public boolean DoChartsOverlap()
    {
        if(chartList[1] == null)
            return false;

        if(chartList[0].isInChart(chartList[1].getpTopLeft()))
            return true;

        if(chartList[0].isInChart(chartList[1].getpTopRight()))
            return true;

        if(chartList[0].isInChart(chartList[1].getpBottomRight()))
            return true;

        if(chartList[0].isInChart(chartList[1].getpBottomLeft()))
            return true;

        return false;
    }

    public Colour GetColour(int X, int Y)
    {
        if(X < 0 || X > 100 || Y <0 || Y > 100 )
            return null;

        Point p = new Point(X,Y);

        //if chartList[0] is null, chartList[1] should be null too since chartList[0] always stores the first chart, and chartList[1] stores the second chart.
        if(chartList[0] != null && chartList[0].isInChart(p))
        {
            if(chartList[1] != null && chartList[1].isInChart(p) ) {
                //(X,Y) is included in both chart 0 and chart 1 .
                return Colour.MixColour(chartList[0].getcChartColour(),chartList[1].getcChartColour());
            }
            //(X,Y) is included in chart 0.
            return chartList[0].getcChartColour();
        }

        if(chartList[1] != null && chartList[1].isInChart(p))
        {
            //(X,Y) is included in chart 1.
            return chartList[1].getcChartColour();
        }

        return null;
    }
}
