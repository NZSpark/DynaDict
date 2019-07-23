package com.nz.spark.chartrenderer;

public class View {
    private Chart chartList[2] ;

    public AddChart(Chart chart)
    {
        if(chartList[0] == null) {
            chartList[0] = chart;
            return;
        }

        if(chartList[1] == null) {
            chartList[1] = chart;
            return;
        }

        chartList[0] = chartList[1];
        chartList[1] = chart;
    }



}
