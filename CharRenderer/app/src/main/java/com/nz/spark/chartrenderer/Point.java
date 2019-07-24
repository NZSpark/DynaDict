package com.nz.spark.chartrenderer;

public class Point {
    public int x;
    public int y;

    Point(int x, int y){
        //check point value is in range {0,100]
        if(x < 0)
            x =0;
        if(x > 100)
            x =100;
        if(y < 0)
            y =0;
        if(y > 100)
            y =100;

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