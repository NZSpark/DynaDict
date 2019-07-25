package com.nz.spark.designpatternsample

class MsmqMessageQueue : OutboundQueue() {
    override fun sendMessage(sIn: String) {
        println("Get Msmq message:$sIn")
    }
}
