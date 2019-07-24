package com.nz.spark.chartrenderer;

public class UnitTest {

    public void UnitTestNoOverlap()
    {
        View v1 = new View();
        //Not chart
        if(v1.DoChartsOverlap())
            System.out.println("No chart overlap check error.");
        else
            System.out.println("No chart overlap check pass.");
        //One chart
        Point p1 = new Point(10,10);
        Point p2 = new Point(40,40);
        Point p3 = new Point(50,50);
        Point p4 = new Point(80,80);

        Colour colour1 = new Colour(50,100,150);
        Colour colour2 = new Colour(150,100,50);
        Chart chart0 = new Chart(p1,p2,colour1);
        Chart chart1 = new Chart(p3,p4,colour2);

        v1.AddChart(chart0);

        if(v1.DoChartsOverlap())
            System.out.println("One chart overlap check error.");
        else
            System.out.println("One chart overlap check pass.");

        //Two charts
        v1.AddChart(chart1);

        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check error.");
        else
            System.out.println("Two charts overlap check pass.");
    }

    public void UnitTestOverlap()
    {
        //overlap with topleft
        View v1 = new View();
        Point p1 = new Point(10,10);
        Point p2 = new Point(50,50);
        Point p3 = new Point(40,40);
        Point p4 = new Point(80,80);

        Colour colour1 = new Colour(50,100,150);
        Colour colour2 = new Colour(150,100,50);
        Chart chart0 = new Chart(p1,p2,colour1);
        Chart chart1 = new Chart(p3,p4,colour2);
        v1.AddChart(chart0);
        v1.AddChart(chart1);

        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check pass.");
        else
            System.out.println("Two charts overlap check error.");

        //overlap with bottomleft
        //change p3=(40,5), p4=(80,40)

        //overlap with bottomright
        //change p3=(5,5), p4=(40,40)

        //overlap with topright
        //change p3=(5,40), p4=(40,80)
    }

    public void UnitTestGetColour()
    {

        View v1 = new View();
        Point p1 = new Point(10,10);
        Point p2 = new Point(50,50);
        Point p3 = new Point(40,40);
        Point p4 = new Point(80,80);

        Colour colour1 = new Colour(50,100,150);
        Colour colour2 = new Colour(150,100,50);
        Chart chart0 = new Chart(p1,p2,colour1);
        Chart chart1 = new Chart(p3,p4,colour2);
        v1.AddChart(chart0);
        v1.AddChart(chart1);

        //point is not in any chart
        if(v1.GetColour( 5,5) != null)
            System.out.println("GetColour check error.");
        else
            System.out.println("GetColour check pass.");

        //point is in chart 0
        if(!v1.GetColour( 15,15).equals(colour1))
            System.out.println("GetColour check error.");
        else
            System.out.println("GetColour check pass.");

        //point is in chart 1
        if(!v1.GetColour( 55,55).equals(colour2))
            System.out.println("GetColour check error.");
        else
            System.out.println("GetColour check pass.");

        //point is in both chart 0 and chart 1
        if(!v1.GetColour( 45,45).equals(Colour.MixColour(colour1,colour2)))
            System.out.println("GetColour check error.");
        else
            System.out.println("GetColour check pass.");
    }
}
