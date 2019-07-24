package com.nz.spark.chartrenderer;

public class Chart {
    private Point pTopLeft;
    private Point pTopRight;
    private Point pBottomRight;
    private Point pBottomLeft;
    private Colour cChartColour;

    public Chart (Point tl, Point br, Colour cc)
    {
        pTopLeft = tl;
        pBottomRight = br;
        cChartColour = cc;
        pTopRight = new Point(pBottomRight.x ,pTopLeft.y);
        pBottomLeft = new Point(pTopLeft.x, pBottomRight.y);
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

    public Point getpBottomLeft() {
        return pBottomLeft;
    }

    public Point getpTopRight() {
        return pTopRight;
    }

    public void setcChartColour(Colour cChartColour) {
        this.cChartColour = cChartColour;
    }

    public Colour getcChartColour() {
        return cChartColour;
    }

    boolean isInChart(Point p)
    {
        if( pTopLeft.Compare(p) > -1 && p.Compare(pBottomRight) > -1 )
            return true;
        return false;
    }
}


