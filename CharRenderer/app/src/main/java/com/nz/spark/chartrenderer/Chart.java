package com.nz.spark.chartrenderer;

public class Chart {
    private Point pTopLeft;
    private Point pBottomRight;
    private Color cChartColor;

    public Chart (Point tl, Point br, Color cc)
    {
        this.pTopLeft = tl;
        this.pBottomRight = br;
        this.cChartColor = cc;
    }

    public void setpBottomRight(Point pBottomRight) {
        this.pBottomRight = pBottomRight;
    }

    public void setpTopLeft(Point pTopLeft) {
        this.pTopLeft = pTopLeft;
    }

    public Point getpBottomRight() {
        return pBottomRight;
    }

    public Point getpTopLeft() {
        return pTopLeft;
    }

    public void setcChartColor(Color cChartColor) {
        this.cChartColor = cChartColor;
    }

    public Color getcChartColor() {
        return cChartColor;
    }

    boolean isInChart(Point p)
    {
        if( pTopLeft.Compare(p) > -1 && p.Compare(pBottomRight) > -1 )
            return true;
        return false;
    }
}


