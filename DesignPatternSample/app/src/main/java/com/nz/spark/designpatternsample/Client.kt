package com.nz.spark.designpatternsample

class Client(private val factory: AbstractFactory_MessageQueue)// The factory creates message queues either for Azure or MSMQ.
// The client does not know which technology is used.
{

    fun sendMessage() {
        //The client doesn't know whether the OutboundQueue is Azure or MSMQ.
        val out = factory.createProductA()
        out.sendMessage("Hello Abstract Factory!")
    }

    fun receiveMessage(): String {
        //The client doesn't know whether the ReplyQueue is Azure or MSMQ.
        val `in` = factory.createProductB()
        return `in`.receiveMessage()
    }
}












