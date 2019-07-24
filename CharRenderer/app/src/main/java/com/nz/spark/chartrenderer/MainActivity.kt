package com.nz.spark.chartrenderer

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import com.nz.spark.charrenderer.R
import kotlinx.android.synthetic.main.activity_main.*

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)
        var ut =  UnitTest()
        ut.UnitTestNoOverlap()
        ut.UnitTestOverlap()
        ut.UnitTestGetColour()
        tvOut.text = "OK!"
    }


}
