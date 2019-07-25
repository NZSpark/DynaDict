package com.nz.spark.designpatternsample

open class ReplyQueue {
    open fun receiveMessage(): String {
        return "Test Message!"
    }
}