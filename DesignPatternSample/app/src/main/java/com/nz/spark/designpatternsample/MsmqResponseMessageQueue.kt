package com.nz.spark.designpatternsample

class MsmqResponseMessageQueue : ReplyQueue() {
    override fun receiveMessage(): String {
        return "Test Message from Msmq"
    }
}