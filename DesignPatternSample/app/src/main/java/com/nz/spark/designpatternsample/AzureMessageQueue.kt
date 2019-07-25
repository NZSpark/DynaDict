package com.nz.spark.designpatternsample

class AzureMessageQueue : OutboundQueue() {
    override fun sendMessage(sIn: String) {
        println("Get Azure message:$sIn")
    }
}
