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

public class Point {
    public int x;
    public int y;

    public Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    //There are 5 results to compare to points. here only return 3 results.
    pubic int Compare(Point p)
    {
        if( p.x > x && p.y > y )
            return 1;
        else if ( p.x == x && p.y == y)
            return 0;
        return -1;
    }
}

public class Color{
    public int R;
    public int G;
    public int B;

    pubic Color(int r, int g, int b)
    {
        this.R = r;
        this.G = g;
        this.B = b;
    }
}