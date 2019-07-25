package com.nz.spark.designpatternsample

class AzureResponseMessageQueue : ReplyQueue() {
    override fun receiveMessage(): String {
        return "Test Message from Azure"
    }
}
