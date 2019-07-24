package com.nz.spark.chartrenderer;

public class UnitTest {

    //3 cases tested.
    public void UnitTestNoOverlap()
    {
        View v1 = new View();
        //case 1: Not chart
        if(v1.DoChartsOverlap())
            System.out.println("No chart overlap check error.");
        else
            System.out.println("No chart overlap check pass.");

        //case 2: One chart
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

        //case 3: Two charts
        v1.AddChart(chart1);

        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check error.");
        else
            System.out.println("Two charts overlap check pass.");
    }

    //4 cases tested.
    public void UnitTestOverlap()
    {
        //case 1: overlap with topleft
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
            System.out.println("Two charts overlap check pass when the second chart overlap with topleft.");
        else
            System.out.println("Two charts overlap check error when the second chart overlap with topleft..");

        //case 2: overlap with bottomleft
        /*
        change p3=(40,5), p4=(80,40)
        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check pass when the second chart overlap with bottomleft.");
        else
            System.out.println("Two charts overlap check error when the second chart overlap with bottomleft.");
        */

        //case 3: overlap with bottomright
        /*
        change p3=(5,5), p4=(40,40)
        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check pass when the second chart overlap with bottomright.");
        else
            System.out.println("Two charts overlap check error when the second chart overlap with bottomright.");
        */

        //case 4: overlap with topright
        /*
        change p3=(5,40), p4=(40,80)

        if(v1.DoChartsOverlap())
            System.out.println("Two charts overlap check pass when the second chart overlap with topright.");
        else
            System.out.println("Two charts overlap check error when the second chart overlap with topright.");
        */
    }

    //3 cases tested.
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

        //case 1: point is not in any chart
        if(v1.GetColour( 5,5) != null)
            System.out.println("GetColour check error when point is not in any chart.");
        else
            System.out.println("GetColour check pass when point is not in any chart.");

        //case 2: point is in chart 0
        if(!v1.GetColour( 15,15).equals(colour1))
            System.out.println("GetColour check error when point is in chart 0.");
        else
            System.out.println("GetColour check pass when point is in chart 0.");

        //case 3: point is in chart 1
        if(!v1.GetColour( 55,55).equals(colour2))
            System.out.println("GetColour check error when point is in chart 1.");
        else
            System.out.println("GetColour check pass when point is in chart 1.");

        //case 4: point is in both chart 0 and chart 1
        if(!v1.GetColour( 45,45).equals(Colour.MixColour(colour1,colour2)))
            System.out.println("GetColour check error when point is in both chart 0 and chart 1.");
        else
            System.out.println("GetColour check pass when point is in both chart 0 and chart 1.");
    }
}
