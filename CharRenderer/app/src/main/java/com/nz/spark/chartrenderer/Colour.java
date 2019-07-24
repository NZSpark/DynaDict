package com.nz.spark.chartrenderer;


public class Colour{
    public int R;
    public int G;
    public int B;

    Colour(int r, int g, int b)
    {
        //check colour value is in range [0,255].
        if(r<0)
            r = 0;
        if(r>255)
            r =255;
        if(g<0)
            g = 0;
        if(g>255)
            g =255;
        if(b<0)
            b = 0;
        if(b>255)
            b =255;

        this.R = r;
        this.G = g;
        this.B = b;
    }

    public static Colour MixColour(Colour c1, Colour c2)
    {
        return new Colour((c1.R+c2.R)/2,(c1.G+c2.G)/2,(c1.B+c2.B)/2);
    }


    public boolean equals(Colour colour) {
        if(colour != null)
            if(R == colour.R && G == colour.G && B == colour.B)
                return true;
        return false;
    }
}