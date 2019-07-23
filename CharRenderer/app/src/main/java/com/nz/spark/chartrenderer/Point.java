package com.nz.spark.chartrenderer;

public class Point {
    public int x;
    public int y;

    Point(int x, int y){
        this.x = x;
        this.y = y;
    }

    //There are 5 results of points comparison. here only return 3 results.
    public int Compare(Point p)
    {
        if( p.x > x && p.y > y )
            return 1;
        else if ( p.x == x && p.y == y)
            return 0;
        return -1;
    }
}