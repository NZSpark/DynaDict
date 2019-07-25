package com.nz.spark.designpatternsample

open class OutboundQueue {
    open fun sendMessage(sIn: String) {
        println(sIn)
    }
}